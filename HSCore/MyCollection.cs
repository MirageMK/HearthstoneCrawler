using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HSCore.Model;
using HtmlAgilityPack;

namespace HSCore
{
    public static class MyCollection
    {
        //static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        //static readonly string ApplicationName = "Hearthstone Crawler";
        internal static readonly string[] NonColectable = { "Roaring Torch", "Tank Up!", "Kazakus Potion", "The Storm Guardian" };
        private const string USER_NAME = "Miragemk";

        public static List<Card> Cards { get; }
        public static int CardCount => Cards.Sum(card => card.Own);

        static MyCollection()
        {
            List<Card> toReturn = new List<Card>();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"http://www.hearthpwn.com/members/{USER_NAME}/collection");

            foreach(HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'card-image-item')]"))
            {
                string cardName = WebUtility.HtmlDecode(cardLink.GetAttributeValue("data-card-name", string.Empty));
                int cardCount = Int32.Parse(cardLink.SelectSingleNode("a/span[contains(@class,'inline-card-count')]").GetAttributeValue("data-card-count", string.Empty));

                Card tempCard = toReturn.FirstOrDefault(x => x.Name == cardName);

                if(tempCard == null)
                {
                    Card card = HeartstoneDB.Get(cardName);
                    card.Own = cardCount;
                    toReturn.Add(card);
                }
                else
                {
                    tempCard.Own += cardCount;
                    if(tempCard.Own >= 2) tempCard.Own = 2;
                }
            }

            Cards = toReturn;
        }

        public static Card Get(string name)
        {
            name = Mapper(name);
            Card newCard = Cards.Find(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (newCard != null) return newCard;

            if (NonColectable.Contains(name))
            {
                return null;
            }

            throw new Exception("MY - Cannot find card with name:" + name);
        }

        private static string Mapper(string name)
        {
            switch(name)
            {
                case "Upgrade":
                    return "Upgrade!";
                case "Dopplegangster":
                    return "Doppelgangster";
                case "Argent Hunter":
                    return "Argent Squire";
                case "Smuggler's  Run":
                    return "Smuggler's Run";
                case "Argent's Lance":
                    return "Argent Lance";
                default:
                    return name;
            }
        }
    }
}
