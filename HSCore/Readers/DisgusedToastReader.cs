using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HSCore.Model;
using HtmlAgilityPack;

namespace HSCore.Readers
{
    public class DisgusedToastReader : BaseReader
    {
        private const string BASE_URL = @"https://disguisedtoast.com";
        private const string META_URL = @"/meta_deck_rankings";

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(BASE_URL + META_URL);
            
            HtmlNode tierSection = doc.DocumentNode.SelectSingleNode("/html/body/main/section[3]");
            if (tierSection == null) return toReturn;
            int tier = 0;
            foreach (HtmlNode tierNode in tierSection.SelectNodes("section"))
            {
                tier++;
                foreach(HtmlNode deckLink in tierNode.SelectNodes("*/*/div[contains(@class,'dt-meta-deck-well')]/a"))
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

            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(BASE_URL + url);

            var deckLink = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'dt-col-decklist-name')]/a");
            if(deckLink == null) return null;
            string deckUrl = deckLink.GetAttributeValue("href", string.Empty);
            toReturn.Name = WebUtility.HtmlDecode(deckLink.InnerText);
            toReturn.UpdateDateString = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'visible-xs-inline-block')]").InnerText;

            doc = web.Load(BASE_URL + deckUrl);
            var nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'dt-decklist-metadata')]/dt");
            toReturn.Class = nodes[nodes.Count - 4].SelectSingleNode("a").InnerText;

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
