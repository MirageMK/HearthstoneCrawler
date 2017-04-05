using System;
using System.Collections.Generic;
using System.Linq;
using HSCore.Extensions;
using HSCore.Model;
using RestSharp;

namespace HSCore
{
    public static class HeartstoneDB
    {
        private const string X_MASHAPE_KEY = "97ivM51w5HmshhjJQhVH0MuyOMA2p1ecDlQjsn1mQyqgCor9NN";
        public static List<Card> Cards { get; }
        static HeartstoneDB()
        {
            List<Card> toReturn = new List<Card>();
            foreach (SetEnum sType in Enum.GetValues(typeof(SetEnum)))
            {
                string setDescription = Enums.GetEnumDescription(sType);

                RestClient client = new RestClient { BaseUrl = new Uri("https://omgvamp-hearthstone-v1.p.mashape.com/cards/sets/" + setDescription + "?collectible=1") };

                RestRequest request = new RestRequest();
                request.AddHeader("X-Mashape-Key", X_MASHAPE_KEY);

                RestResponse<List<Card>> response = client.Execute<List<Card>>(request) as RestResponse<List<Card>>;
                if (response != null) toReturn.AddRange(response.Data.Where(x => x.Type != "Hero"));
            }
            Cards = toReturn;
        }

        public static Card Get(string name)
        {
            Card newCard = Cards.Find(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (newCard != null) return newCard;

            newCard = Cards.Find(x => string.Equals(x.Name, Mapper(name), StringComparison.CurrentCultureIgnoreCase));
            if (newCard != null) return newCard;

            throw new Exception("DB - Cannot find card with name:" + name);
        }

        private static string Mapper(string name)
        {
            List<int> matchList = Cards.Select(card => Algorithms.LevenshteinDistance(card.Name, name)).ToList();

            return Cards.ElementAt(matchList.IndexOf(matchList.Min())).Name;
        }
    }
}
