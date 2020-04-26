using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using HSCore.Extensions;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;

namespace HSCore
{
    public static class MyCollection
    {
        private const string USER_NAME = "Miragemk";
        //private const string USER_NAME = "GlAdIaToR_kanga";
        //private const string USER_NAME = "NikoRuso";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        static MyCollection()
        {
            List<Card> toReturn = new List<Card>();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"https://www.hearthpwn.com/members/{USER_NAME}/collection");

            foreach(HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'card-image-item')]"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.GetAttributeValue("data-card-name", string.Empty));
                int cardCount = int.Parse(cardLink.SelectSingleNode("a/span[contains(@class,'inline-card-count')]").GetAttributeValue("data-card-count", string.Empty));

                Card tempCard = toReturn.FirstOrDefault(x => x.Name == cardName);

                if(tempCard == null)
                {
                    Card card = HeartstoneDB.Get(cardName);
                    if(card == null)
                    {
                        continue;
                    }
                    card.Own = cardCount > 2 ? 2 : cardCount;
                    toReturn.Add(card);
                }
                else
                {
                    tempCard.Own += cardCount;
                    if(tempCard.Own >= 2) tempCard.Own = 2;
                }
            }

            Cards = toReturn;
            log.Info($"Cards in collection: {Cards.Count}");
        }

        public static List<Card> Cards { get; }
        public static int CardCount => Cards.Sum(card => card.Own);

        public static Card Get(string name)
        {
            Card newCard = Cards.Find(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if(newCard != null) return newCard;

            string newCardName = Mapper(name);
            if(Algorithms.LevenshteinDistance(name, newCardName) > 3)
            {
                log.Error($"MY - Cannot find card with name: {name}");
                return null;
            }

            newCard = Cards.Find(x => string.Equals(x.Name, newCardName, StringComparison.CurrentCultureIgnoreCase));
            log.Warn($"My Card: {name} replaced with {newCard.Name}");
            return newCard;
        }

        public static Card GetByID(string id)
        {
            Card newCard = Cards.Find(x => string.Equals(x.CardId, id, StringComparison.CurrentCultureIgnoreCase));
            if(newCard != null) return newCard;

            throw new Exception("MY - Cannot find card with Id:" + id);
        }

        public static Card GetByID(int id)
        {
            Card newCard = Cards.Find(x => x.DbfId == id);
            if (newCard != null) return newCard;

            throw new Exception("MY - Cannot find card with DbfId:" + id);
        }

        private static string Mapper(string name)
        {
            List<int> matchList = Cards.Select(card => Algorithms.LevenshteinDistance(card.Name, name)).ToList();

            return Cards.ElementAt(matchList.IndexOf(matchList.Min())).Name;
        }
    }
}