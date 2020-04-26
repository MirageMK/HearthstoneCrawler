using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using HSCore.Extensions;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;

namespace HSCore.Readers
{
    public class ViciousSyndicateReader : BaseReader
    {
        private const string LIBRARY_URL = "http://www.vicioussyndicate.com/deck-library/";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(LIBRARY_URL);

                foreach (HtmlNode deckLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'menu-deck-library-menu-container')]/ul/li/a"))
                {
                    string deckClass = deckLink.InnerHtml;

                    string deckUrl = deckLink.GetAttributeValue("href", string.Empty);

                    Deck deck = GetDeck(deckUrl);
                    if(deck == null) continue;
                    deck.Source = SourceEnum.ViciousSyndicate;
                    deck.Name = deckLink.InnerHtml;
                    deck.Tier = 1;//NOT GOOD;
                    deck.UpdateDateString = DateTime.Now.ToLongDateString();//NOT GOOD
                    deck.Class = deckClass;

                    toReturn.Add(deck);
                }
            }
            catch (Exception ex)
            {
                log.Error("Problem", ex);
            }

            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;

            try
            {
                doc = web.Load(url);
            }
            catch(Exception)
            {
                log.Warn($"Cannot pull deck from url: {url}");
                return null;
            }

            toReturn.Url = url;
            HtmlNode deckLink = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'article-content')]/*/a/img") ??
                                doc.DocumentNode.SelectSingleNode("//*[contains(@class,'entry-content')]/*/a/img") ??
                                doc.DocumentNode.SelectSingleNode("//*[contains(@class,'entry-content')]/*/*/a/img");

            if(deckLink == null)
            {
                log.Warn($"Cannot find deck on {url}");
                return null;
            }
            string deckUrl = deckLink.ParentNode.GetAttributeValue("href", string.Empty);
            doc = web.Load(deckUrl);

            HtmlNode cardsMeta = doc.DocumentNode.SelectSingleNode("//meta[@property='x-hearthstone:deck:cards']");

            if(cardsMeta != null)
            {
                string cardsString = cardsMeta.GetAttributeValue("content", string.Empty);
                string[] cardArray = cardsString.Split(',');
                foreach(string cardID in cardArray)
                {
                    Card card = MyCollection.GetByID(cardID);
                    if(toReturn.Cards.ContainsKey(card))
                        toReturn.Cards[card]++;
                    else
                        toReturn.Cards.Add(card, 1);
                }
            }
            else
            {
                string cardsString = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'entry-content')]/p").InnerHtml;
                string[] cardArray = cardsString.Split(new[] { "<br>\n" }, StringSplitOptions.RemoveEmptyEntries);
                for(int i = Array.IndexOf(cardArray, "#") + 1;; i++)
                {
                    if(cardArray[i] == "#") break;
                    string[] cardData = cardArray[i].Split(new[] { "x (" }, StringSplitOptions.RemoveEmptyEntries);
                    string cardName = cardData[1].Substring(3).Trim();
                    cardName = WebUtility.HtmlDecode(cardName);
                    cardName = cardName.Replace('’', '\'');
                    Card card = MyCollection.Get(cardName);
                    if(card == null)
                    {
                        toReturn.IsError = true;
                        continue;
                    }
                    toReturn.Cards.Add(card, int.Parse(cardData[0].Substring(2)));
                }
            }

            return toReturn;
        }
    }
}