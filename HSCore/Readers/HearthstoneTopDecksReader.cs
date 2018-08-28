using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using HSCore.Helper;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;

namespace HSCore.Readers
{
    public class HearthstoneTopDecksReader : BaseReader
    {
        private const string URL = @"http://www.hearthstonetopdecks.com/";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(URL);


                int tier = 0;
                foreach(HtmlNode tierBox in doc.DocumentNode.SelectNodes("//*[contains(@class,'deck-lists')]"))
                {
                    if(tier < 5)
                        tier++;
                    foreach(HtmlNode deckLink in tierBox.Descendants("a"))
                    {
                        string deckTypeUrl = deckLink.GetAttributeValue("href", string.Empty);
                        if(deckTypeUrl == "")
                        {
                            log.Warn($"Cannot find deckTypeUrl on {deckLink.OuterHtml}");
                            continue;
                        }

                        string deckUrl = GetDeckUrl(deckTypeUrl);
                        if(String.IsNullOrEmpty(deckUrl))
                        {
                            log.Warn($"Cannot find deckUrl on {deckTypeUrl}");
                            continue;
                        }

                        Deck deck = GetDeck(deckUrl);
                        deck.Name = WebUtility.HtmlDecode(deckLink.InnerText).Trim();
                        deck.Source = SourceEnum.HearthstoneTopDecks;
                        deck.Tier = tier;
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

        private string GetDeckUrl(string deckTypeUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(deckTypeUrl + "?sort=top-week");

            HtmlNode topDeckOfTheWeek = doc.DocumentNode.SelectSingleNode(@"//*[@id='deck-list']/tbody/tr[1]/td[2]/h4/a");

            return topDeckOfTheWeek != null ? topDeckOfTheWeek.GetAttributeValue("href", string.Empty) : null;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            toReturn.Url = url;

            toReturn.Class = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'single-deck-header')]").GetAttributeValue("class", "single-deck-header").Split(' ')[1].FirstCharToUpper();
            toReturn.UpdateDateString = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'updated')]").GetAttributeValue("datetime", DateTime.Now.ToString());

            foreach (HtmlNode cardLink in doc.DocumentNode.SelectSingleNode("//*[@id = 'deck-master']").SelectNodes("div/ul/li"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.SelectSingleNode("a/*[contains(@class,'card-name')]").InnerText).Replace('’', '\'');
                string cardCount = cardLink.SelectSingleNode("*[contains(@class,'card-count')]").InnerText;

                Card card = MyCollection.Get(cardName);
                if (card == null)
                {
                    toReturn.IsError = true;
                    continue;
                }
                int cardCountInt;
                if(int.TryParse(cardCount, out cardCountInt))
                {
                    if (toReturn.Cards.ContainsKey(card))
                    {
                        log.Warn($"{card} already exist in the deck. ( {toReturn.Url} )");
                        continue;
                    }
                    toReturn.Cards.Add(card, cardCountInt);
                }
                else
                {
                    log.Warn($"Wrong card count on {cardName} ( {url} )"); ;
                }
            }

            return toReturn;
        }
    }
}