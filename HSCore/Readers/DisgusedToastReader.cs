using System;
using System.Collections.Generic;
using System.Net;
using HSCore.Model;
using HtmlAgilityPack;

namespace HSCore.Readers
{
    public class DisgusedToastReader : BaseReader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string BASE_URL = @"https://disguisedtoast.com";
        private const string META_URL = @"/meta_deck_rankings";

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(BASE_URL + META_URL);

                HtmlNode tierSection = doc.DocumentNode.SelectSingleNode("/html/body/main/section[3]");
                if (tierSection == null) return toReturn;
                int tier = 0;
                foreach (HtmlNode tierNode in tierSection.SelectNodes("section"))
                {
                    tier++;
                    foreach (HtmlNode deckLink in tierNode.SelectNodes("*/*/div[contains(@class,'dt-meta-deck-well')]/a"))
                    {
                        string deckUrl = deckLink.GetAttributeValue("href", string.Empty);
                        if (deckUrl == "") continue;
                        Deck deck = GetDeck(deckUrl);
                        if (deck == null) continue;
                        deck.Source = SourceEnum.DisguisedToast;
                        deck.Tier = tier;
                        toReturn.Add(deck);
                    }
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
            HtmlDocument doc = web.Load(BASE_URL + url);

            var deckLink = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'dt-col-decklist-name')]/a");
            if (deckLink == null) return null;
            string deckUrl = deckLink.GetAttributeValue("href", string.Empty);

            doc = web.Load(BASE_URL + deckUrl);
            toReturn.Url = BASE_URL + url;
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'dt-decklist-metadata')]/dt");
            toReturn.Class = nodes[1].SelectSingleNode("a").InnerText;
            toReturn.Name = WebUtility.HtmlDecode(nodes[2].SelectSingleNode("a").InnerText);
            toReturn.UpdateDateString = nodes[nodes.Count - 1].InnerText;

            foreach (HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'dt-cardlist')]/li"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.SelectSingleNode("*[contains(@class,'dt-card-name')]").InnerText).Trim();
                string cardCount = cardLink.SelectSingleNode("*[contains(@class,'dt-card-quantity')]").InnerText;

                Card card = MyCollection.Get(cardName);
                toReturn.Cards.Add(card, int.Parse(cardCount));
            }

            return toReturn;
        }
    }
}
