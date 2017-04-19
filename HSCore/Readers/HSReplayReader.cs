using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using HSCore.Model;
using HtmlAgilityPack;
using RestSharp;

namespace HSCore.Readers
{
    public class HSReplayReader : BaseReader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string URL = "https://hsreplay.net/";
        private const string SNAPSHOT_END_POINT = @"analytics/query/list_decks_by_win_rate?GameType=RANKED_{mode}&TimeRange=LAST_30_DAYS";
        private const string CARD_END_POINT = @"cards/";

        private readonly Dictionary<string,string> cardMaper = new Dictionary<string, string>();

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            try
            {
                foreach (DeckType dType in Enum.GetValues(typeof(DeckType)))
                {
                    if (dType == DeckType.Undefined) continue;

                    string deckTypeDescription = Enums.GetEnumDescription(dType);

                    RestClient client = new RestClient(URL);
                    RestRequest request = new RestRequest(SNAPSHOT_END_POINT, Method.GET);
                    request.AddUrlSegment("mode", deckTypeDescription.ToUpper());

                    IRestResponse response = client.Execute(request);
                    dynamic snapshot = Json.Decode(response.Content);

                    foreach (dynamic classObj in snapshot.series.data)
                    {
                        Deck deck = GetDeck(classObj.Value[0].deck_list.ToString());
                        deck.Name = classObj.Key;
                        string playerClass = classObj.Key.ToLower();
                        char[] a = playerClass.ToCharArray();
                        a[0] = char.ToUpper(a[0]);
                        deck.Class = new string(a);
                        deck.Tier = 1;
                        deck.Source = SourceEnum.HSReplay;
                        deck.UpdateDateString = snapshot.as_of;
                        toReturn.Add(deck);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Problem", ex);
            }

            return toReturn;
        }

        private Deck GetDeck(string jsonDeck)
        {
            Deck toReturn = new Deck();

            string[][] res = jsonDeck.Trim('[').Trim(']').Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split(',')).ToArray();

            HtmlWeb web = new HtmlWeb();
            foreach (string[] strings in res)
            {
                string cardName;
                if(cardMaper.ContainsKey(strings[0]))
                {
                    cardName = cardMaper[strings[0]];
                }
                else
                {
                    HtmlDocument doc = web.Load(URL + CARD_END_POINT + strings[0]);
                    cardName = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                    cardName = WebUtility.HtmlDecode(cardName.Split(new[] { " - HSReplay.net" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    cardMaper.Add(strings[0], cardName);
                }

                Card card = MyCollection.Get(cardName);
                toReturn.Cards.Add(card, int.Parse(strings[1]));
            }

            return toReturn;
        }
    }
}
