using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
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
                        string deckUrl = deckLink.GetAttributeValue("href", string.Empty);
                        if(deckUrl == "")
                        {
                            log.Warn($"Cannot find deckUrl on {deckLink.OuterHtml}");
                            continue;
                        };

                        Deck deck = GetDeck(deckUrl);
                        deck.Name = WebUtility.HtmlDecode(deckLink.InnerText);
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

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            toReturn.Url = url;
            
            toReturn.Class = doc.DocumentNode.SelectNodes("//*[contains(@class,'deck-info')]/a")[0].InnerText;
            toReturn.UpdateDateString = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'updated')]").GetAttributeValue("datetime", DateTime.Now.ToString());

            foreach(HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'card-frame')]"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.SelectSingleNode("a/*[contains(@class,'card-name')]").InnerText).Replace('’', '\'');
                string cardCount = cardLink.SelectSingleNode("*[contains(@class,'card-count')]").InnerText;

                Card card = MyCollection.Get(cardName);
                toReturn.Cards.Add(card, int.Parse(cardCount));
            }

            return toReturn;
        }
    }
}