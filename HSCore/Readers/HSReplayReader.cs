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

        private const string SNAPSHOT_END_POINT = @"analytics/query/trending_decks_by_popularity/";
        //private const string SNAPSHOT_END_POINT = @"analytics/query/list_decks_by_win_rate/?GameType=RANKED_STANDARD";

        private const string ARCHETYPE_END_POINT = @"api/v1/archetypes/";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                RestClient client = new RestClient(URL);
                RestRequest archetypeRequest = new RestRequest(ARCHETYPE_END_POINT, Method.GET);
                IRestResponse archtypeResponse = client.Execute(archetypeRequest);
                dynamic archtype = Json.Decode(archtypeResponse.Content);

                Dictionary<int, string> archtypeMapper = new Dictionary<int, string>();
                for(int i = 0; i < archtype.Length; i++)
                {
                    archtypeMapper.Add(archtype[i].id, archtype[i].name);
                }

                RestRequest deckRequest = new RestRequest(SNAPSHOT_END_POINT, Method.GET);

                IRestResponse response = client.Execute(deckRequest);
                dynamic snapshot = Json.Decode(response.Content);

                foreach(dynamic classObj in snapshot.series.data)
                {
                    if (classObj.Value.Length == 0) continue;
                    for(int i = 0; i < classObj.Value.Length; i++)
                    {
                        var deckObj = classObj.Value[i];
                        if(deckObj.archetype_id == null)
                        {
                            log.Warn($"No archetype for class {classObj.Key} deck {i}");
                            continue;
                        }
                        Deck deck = GetDeck(deckObj.deck_list.ToString());
                        deck.Name = archtypeMapper[deckObj.archetype_id];
                        string playerClass = classObj.Key.ToLower();
                        char[] a = playerClass.ToCharArray();
                        a[0] = char.ToUpper(a[0]);
                        deck.Class = new string(a);
                        deck.Tier = 5;
                        if(deckObj.win_rate >= (decimal) 54)
                            deck.Tier = 1;
                        else if(deckObj.win_rate >= (decimal) 50)
                            deck.Tier = 2;
                        else if(deckObj.win_rate >= (decimal) 46)
                            deck.Tier = 3;
                        else if(deckObj.win_rate >= (decimal) 40)
                            deck.Tier = 4;
                        else
                            deck.Tier = 5;
                        deck.Source = SourceEnum.HSReplay;
                        deck.UpdateDateString = snapshot.as_of;
                        deck.Url = $"https://hsreplay.net/decks/{deckObj.shortid}/";
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

        private Deck GetDeck(string jsonDeck)
        {
            Deck toReturn = new Deck();

            string[][] res = jsonDeck.Trim('[').Trim(']').Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split(',')).ToArray();

            foreach(string[] strings in res)
            {
                Card card = MyCollection.GetByID(Int32.Parse(strings[0]));
                if (toReturn.Cards.ContainsKey(card))
                {
                    log.Warn($"{card} already exist in the deck. ( {toReturn.Url} )");
                    continue;
                }
                toReturn.Cards.Add(card, int.Parse(strings[1]));
            }

            return toReturn;
        }
    }
}