using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Helpers;
using HSCore.Extensions;
using HSCore.Model;
using log4net;
using RestSharp;

namespace HSCore
{
    public static class HeartstoneDB
    {
        private const string X_MASHAPE_KEY = "97ivM51w5HmshhjJQhVH0MuyOMA2p1ecDlQjsn1mQyqgCor9NN";
        private const string DB_URL = "https://api.hearthstonejson.com/v1/19506/enUS/cards.collectible.json";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        static HeartstoneDB()
        {
            List<Card> toReturn = new List<Card>();
            Dictionary<string, int> dbfMaper = new Dictionary<string, int>();
            
            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(DB_URL);
                dynamic cards = Json.Decode(json);
                foreach (dynamic card in cards)
                {
                    if(!dbfMaper.ContainsKey(card.id))
                    {
                        dbfMaper.Add(card.id, card.dbfId);
                    }
                }
            }
            
            foreach (SetEnum sType in Enum.GetValues(typeof(SetEnum)))
            {
                string setDescription = Enums.GetEnumDescription(sType);

                RestClient client = new RestClient { BaseUrl = new Uri("https://omgvamp-hearthstone-v1.p.mashape.com/cards/sets/" + setDescription + "?collectible=1") };

                RestRequest request = new RestRequest();
                request.AddHeader("X-Mashape-Key", X_MASHAPE_KEY);

                RestResponse<List<Card>> response = client.Execute<List<Card>>(request) as RestResponse<List<Card>>;
                if(response == null) continue;

                response.Data.ForEach(x => x.DBId = dbfMaper[x.CardId]);

                toReturn.AddRange(response.Data);
            }
            
            Cards = toReturn.Where(x => x.Type != "Hero").ToList();

            log.Info($"Cards in database: {Cards.Count}");
        }

        public static List<Card> Cards { get; }

        public static Card Get(string name)
        {
            Card newCard = Cards.Find(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (newCard != null) return newCard;

            newCard = Cards.Find(x => string.Equals(x.Name, Mapper(name), StringComparison.CurrentCultureIgnoreCase));
            if (newCard == null) throw new Exception("DB - Cannot find card with name:" + name);

            log.Warn($"DB Card: {name} replaced with {newCard.Name}");
            return newCard;
        }

        private static string Mapper(string name)
        {
            List<int> matchList = Cards.Select(card => Algorithms.LevenshteinDistance(card.Name, name)).ToList();

            return Cards.ElementAt(matchList.IndexOf(matchList.Min())).Name;
        }
    }
}