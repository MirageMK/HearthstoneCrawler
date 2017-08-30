using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;

namespace HSCore.Readers
{
    public class DisgusedToastReader : BaseReader
    {
        private const string BASE_URL = @"https://disguisedtoast.com";
        private const string META_URL = @"/meta_deck_rankings";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(BASE_URL + META_URL);

                HtmlNode tierSection = doc.DocumentNode.SelectSingleNode("/html/body/main/section[3]");
                if(tierSection == null) return toReturn;
                int tier = 0;
                foreach(HtmlNode tierNode in tierSection.SelectNodes("section"))
                {
                    tier++;
                    if (tierNode.SelectNodes("*/*/div[contains(@class,'dt-meta-deck-well')]/a") == null) continue;

                    foreach(HtmlNode deckLink in tierNode.SelectNodes("*/*/div[contains(@class,'dt-meta-deck-well')]/a"))
                    {
                        string deckUrl = deckLink.GetAttributeValue("href", string.Empty);
                        if(deckUrl == "")
                        {
                            log.Warn($"Cannot find deckUrl on {deckLink.OuterHtml}");
                            continue;
                        }
                        Deck deck = GetDeck(deckUrl);
                        if(deck == null) continue;
                        deck.Source = SourceEnum.DisguisedToast;
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
            HtmlDocument doc = web.Load(BASE_URL + url);

            HtmlNode deckLink = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'dt-col-decklist-name')]/a");
            if(deckLink == null)
            {
                log.Warn($"Cannot find deck on {BASE_URL + url}");
                return null;
            }

            string deckUrl = deckLink.GetAttributeValue("href", string.Empty);
            toReturn.UpdateDateString = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'dt-tight')]/*[contains(@class,'visible-xs-inline-block')]").InnerText;
            int isPlayer = string.IsNullOrEmpty(deckLink.ParentNode.NextSibling.NextSibling.InnerText.Trim('\n')) ? 0 : 1;

            doc = web.Load(BASE_URL + deckUrl);
            toReturn.Url = BASE_URL + url;
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'dt-decklist-metadata')]/dt");
            toReturn.Class = nodes[isPlayer + 1].SelectSingleNode("a").InnerText;
            toReturn.Name = WebUtility.HtmlDecode(nodes[isPlayer + 2].SelectSingleNode("a").InnerText);

            foreach(HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'dt-cardlist')]/li"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.SelectSingleNode("*[contains(@class,'dt-card-name')]").InnerText).Trim();
                string cardCount = cardLink.SelectSingleNode("*[contains(@class,'dt-card-quantity')]").InnerText;

                Card card = MyCollection.Get(cardName);
                if(card == null)
                {
                    toReturn.IsError = true;
                    continue;
                }
                if (toReturn.Cards.ContainsKey(card))
                {
                    log.Warn($"{card} already exist in the deck. ( {toReturn.Url} )");
                    continue;
                }
                toReturn.Cards.Add(card, int.Parse(cardCount));
            }

            return toReturn;
        }
    }
}