using System;
using System.Collections.Generic;
using System.Reflection;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;

namespace HSCore.Readers
{
    public class MetabombReader : BaseReader
    {
        private const string URL = "http://www.metabomb.net";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        private string GetUrl()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(URL + "/hearthstone/game-guides");

            string link = "";
            bool found = false;
            foreach(HtmlNode selectNode in doc.DocumentNode.SelectNodes("//*[contains(@class,'spotlight')]/div/div"))
            {
                link = selectNode.SelectSingleNode("div/a").Attributes["href"].Value;
                if(link.ToLower().Contains("tier"))
                {
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                foreach(HtmlNode selectNode in doc.DocumentNode.SelectNodes("//*[contains(@class,'latest')]/ul/li"))
                {
                    link = selectNode.SelectSingleNode("div/a").Attributes["href"].Value;
                    if(link.ToLower().Contains("tier")) break;
                }
            }
            return URL + link;
        }

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(GetUrl());

                int tier = 0;

                foreach(HtmlNode deckSection in doc.DocumentNode.SelectNodes("//*[contains(@class,'article-content')]/section"))
                {
                    HtmlNodeCollection deckNode = deckSection.SelectNodes("table/tbody/tr");
                    if(deckNode == null)
                    {
                        deckNode = deckSection.SelectNodes("*//table/tbody/tr");
                        if (deckNode == null)
                        {
                            continue;
                        }
                    }
                    foreach(HtmlNode deckLink in deckNode)
                    {
                        if(deckLink.ChildNodes[0].InnerHtml != "" && tier < 5) tier++;

                        if(deckLink.SelectSingleNode("td/a") == null)
                            continue;
                        string deckUrl = deckLink.SelectSingleNode("td/a").GetAttributeValue("href", string.Empty);
                        if(deckUrl == "")
                        {
                            log.Warn($"Cannot find deckUrl on {deckLink.OuterHtml}");
                            continue;
                        }
                        ;
                        Deck deck = GetDeck(deckUrl);
                        if(deck == null) continue;
                        deck.Tier = tier;
                        deck.Name = deckLink.ChildNodes[1].InnerText;
                        deck.Source = SourceEnum.Metabomb;
                        toReturn.Add(deck);
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("Problem", ex);
            }
            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            toReturn.Url = url;
            if(doc.DocumentNode.SelectNodes("//*[contains(@class,'article-content')]") == null)
            {
                log.Warn($"Cannot find decks on {url}");
                return null;
            }

            toReturn.UpdateDateString = doc.DocumentNode.SelectSingleNode("//*[contains(@itemprop,'datePublished')]").GetAttributeValue("content", "");

            foreach(HtmlNode cardSection in doc.DocumentNode.SelectNodes("//*[contains(@class,'article-content')]/section"))
            {
                HtmlNode table = cardSection.SelectSingleNode("table");
                if(table == null) continue;

                toReturn.Class = table.SelectSingleNode("thead/tr/th[1]").InnerText;

                foreach(HtmlNode cardNode in table.SelectNodes("tbody/tr/td"))
                {
                    string cardName = cardNode.InnerText;
                    if(cardName == "") continue;
                    cardName = cardName.Substring(3).Trim();

                    int cardCount;
                    if(cardNode.InnerText.Contains("2 x"))
                        cardCount = 2;
                    else if(cardNode.InnerText.Contains("1 x"))
                        cardCount = 1;
                    else
                        continue;

                    Card card = MyCollection.Get(cardName);
                    if (card == null)
                    {
                        toReturn.IsError = true;
                        continue;
                    }
                    if (toReturn.Cards.ContainsKey(card))
                    {
                        log.Warn($"{card} already exist in the deck.");
                        continue;
                    }
                    toReturn.Cards.Add(card, cardCount);
                }

                break;
            }

            return toReturn;
        }
    }
}