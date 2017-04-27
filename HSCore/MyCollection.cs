using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HSCore.Model;
using HtmlAgilityPack;
using HSCore.Extensions;

namespace HSCore
{
    public static class MyCollection
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        //static readonly string ApplicationName = "Hearthstone Crawler";
        internal static readonly string[] NonColectable = { "Roaring Torch", "Tank Up!", "Kazakus Potion", "The Storm Guardian" };
        private const string USER_NAME = "Miragemk";
        //private const string USER_NAME = "GlAdIaToR_kanga";
        //private const string USER_NAME = "NikoRuso";
        
        public static List<Card> Cards { get; }
        public static int CardCount => Cards.Sum(card => card.Own);

        static MyCollection()
        {
            List<Card> toReturn = new List<Card>();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"http://www.hearthpwn.com/members/{USER_NAME}/collection");

            foreach (HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'card-image-item')]"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.GetAttributeValue("data-card-name", string.Empty));
                int cardCount = int.Parse(cardLink.SelectSingleNode("a/span[contains(@class,'inline-card-count')]").GetAttributeValue("data-card-count", string.Empty));

                Card tempCard = toReturn.FirstOrDefault(x => x.Name == cardName);

                if (tempCard == null)
                {
                    Card card = HeartstoneDB.Get(cardName);
                    card.Own = (cardCount > 2 ? 2 : cardCount);
                    toReturn.Add(card);
                }
                else
                {
                    tempCard.Own += cardCount;
                    if (tempCard.Own >= 2) tempCard.Own = 2;
                }
            }

            Cards = toReturn;
            log.Info($"Cards in collection: {Cards.Count}");
        }

        public static Card Get(string name)
        {
            if (NonColectable.Contains(name))
            {
                return null;
            }

            Card newCard = Cards.Find(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (newCard != null) return newCard;
            
            newCard = Cards.Find(x => string.Equals(x.Name, Mapper(name), StringComparison.CurrentCultureIgnoreCase));
            if (newCard == null) throw new Exception("MY - Cannot find card with name:" + name);

            log.Warn($"My Card: {name} replaced with {newCard.Name}");
            return newCard;
        }

        public static Card GetByID(string id)
        {
            Card newCard = Cards.Find(x => string.Equals(x.CardId, id, StringComparison.CurrentCultureIgnoreCase));
            if (newCard != null) return newCard;

            throw new Exception("MY - Cannot find card with Id:" + id);
        }

        private static string Mapper(string name)
        {
            List<int> matchList = Cards.Select(card => Algorithms.LevenshteinDistance(card.Name, name)).ToList();

            return Cards.ElementAt(matchList.IndexOf(matchList.Min())).Name;
        }
    }
}
