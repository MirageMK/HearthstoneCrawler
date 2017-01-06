using System;
using System.Collections.Generic;
using System.Linq;
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
                if(response != null) toReturn.AddRange(response.Data.Where(x => x.Type != "Hero"));
            }
            Cards = toReturn;
        }

        public static Card Get(string name)
        {
            Card newCard = Cards.Find(x => x.Name == name);

            if (newCard == null)
                throw new Exception("Cannot find card with name:" + name);

            return newCard;
        }
    }
}
