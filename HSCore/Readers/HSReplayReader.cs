using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web.Helpers;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;
using RestSharp;

namespace HSCore.Readers
{
    public class HSReplayReader : BaseReader
    {
        private const string URL = "https://hsreplay.net/";

        //private const string SNAPSHOT_END_POINT = @"analytics/query/list_decks_by_win_rate?GameType=RANKED_{mode}&TimeRange=LAST_30_DAYS";
        private const string SNAPSHOT_END_POINT = @"analytics/query/trending_decks_by_popularity/";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            try
            {
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest(SNAPSHOT_END_POINT, Method.GET);

                IRestResponse response = client.Execute(request);
                dynamic snapshot = Json.Decode(response.Content);

                foreach(dynamic classObj in snapshot.series.data)
                {
                    if (classObj.Value.Length == 0) continue;
                    Deck deck = GetDeck(classObj.Value[0].deck_list.ToString());
                    deck.Name = classObj.Key;
                    string playerClass = classObj.Key.ToLower();
                    char[] a = playerClass.ToCharArray();
                    a[0] = char.ToUpper(a[0]);
                    deck.Class = new string(a);
                    deck.Tier = 5;
                    if(classObj.Value[0].win_rate >= (decimal) 55)
                        deck.Tier = 1;
                    else if(classObj.Value[0].win_rate >= (decimal) 50)
                        deck.Tier = 2;
                    else if(classObj.Value[0].win_rate >= (decimal) 45)
                        deck.Tier = 3;
                    else if(classObj.Value[0].win_rate >= (decimal) 40)
                        deck.Tier = 4;
                    else
                        deck.Tier = 5;
                    deck.Source = SourceEnum.HSReplay;
                    deck.UpdateDateString = snapshot.as_of;
                    deck.Url = $"https://hsreplay.net/decks/{classObj.Value[0].shortid}/";
                    toReturn.Add(deck);
                }
            }
            catch(Exception ex)
            {
                log.Error("Problem", ex);
            }

            return toReturn;
        }

        private Deck GetDeck(string jsonDeck)
        {
            Deck toReturn = new Deck();

            string[][] res = jsonDeck.Trim('[').Trim(']').Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split(',')).ToArray();

            foreach(string[] strings in res)
            {
                Card card = MyCollection.GetByID(Int32.Parse(strings[0]));
                if (toReturn.Cards.ContainsKey(card))
                {
                    log.Warn($"{card} already exist in the deck.");
                    continue;
                }
                toReturn.Cards.Add(card, int.Parse(strings[1]));
            }

            return toReturn;
        }
    }
}