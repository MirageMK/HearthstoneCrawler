using System;
using System.Collections.Generic;
using HSCore.Model;
using HtmlAgilityPack;

namespace HSCore.Readers
{
    public class MetabombReader : BaseReader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
 (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string URL = "http://hearthstone.metabomb.net";

        private string GetUrl()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(URL + "/game-guides");

            var xPath = "//main/div[1]/ul/li";
            var link = "";
            foreach (HtmlNode selectNode in doc.DocumentNode.SelectNodes(xPath))
            {

                link = selectNode.FirstChild.Attributes["href"].Value;
                if (link.ToLower().Contains("tier")) break;
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
                    if(deckNode == null) continue;
                    foreach(HtmlNode deckLink in deckNode)
                    {
                        if(deckLink.ChildNodes[0].InnerHtml != "" && tier < 5) tier++;

                        if(deckLink.SelectSingleNode("td/a") == null) continue;
                        string deckUrl = deckLink.SelectSingleNode("td/a").GetAttributeValue("href", string.Empty);
                        if(deckUrl == "") continue;
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
            if (doc.DocumentNode.SelectNodes("//*[contains(@class,'article-content')]") == null) return null;

            toReturn.UpdateDateString = doc.DocumentNode.SelectSingleNode("//*[contains(@itemprop,'datePublished')]").GetAttributeValue("content","");

            foreach (HtmlNode cardSection in doc.DocumentNode.SelectNodes("//*[contains(@class,'article-content')]/section"))
            {
                HtmlNode table = cardSection.SelectSingleNode("table");
                if (table == null) continue;

                toReturn.Class = table.SelectSingleNode("thead/tr/th[1]").InnerText;
                
                foreach (HtmlNode cardNode in table.SelectNodes("tbody/tr/td"))
                {
                    string cardName = cardNode.InnerText;
                    if (cardName == "") continue;
                    cardName = cardName.Substring(3).Trim();

                    int cardCount;
                    if (cardNode.InnerText.Contains("2 x"))
                    {
                        cardCount = 2;
                    }
                    else if (cardNode.InnerText.Contains("1 x"))
                    {
                        cardCount = 1;
                    }
                    else
                    {
                        continue;
                    }

                    Card card = MyCollection.Get(cardName);
                    if(card == null) continue;
                    toReturn.Cards.Add(card, cardCount);
                }

                break;
            }

            return toReturn;
        }
    }
}
