using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using HSCore.Model;
using HtmlAgilityPack;

namespace HSCore.Readers
{
    public class ViciousSyndicateReader : BaseReader
    {
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string APPLICATION_NAME = "Hearthstone Crawler";

        private const string DECK_URL = "http://vicioussyndicate.s3-us-west-1.amazonaws.com/datareaper/radar/{deckName}/index.html";


        private List<Deck> GetDeckRanks()
        {
            List<Deck> toReturn = new List<Deck>();

            UserCredential credential;

            using(var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(
                                                            Environment.SpecialFolder.Personal);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                                                         GoogleClientSecrets.Load(stream).Secrets,
                                                                         Scopes,
                                                                         "user",
                                                                         CancellationToken.None,
                                                                         new FileDataStore(credPath, true)).Result;
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
                                            {
                                                HttpClientInitializer = credential,
                                                ApplicationName = APPLICATION_NAME,
                                            });

            // Define request parameters.
            string spreadsheetId = "1osCVci8-7ttXp_CjWORzEUYf5VQlGWN_ZsOUrbCX0AI";
            string range = "Top Archetype Matchups!A1:AZ";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;
            if(values != null && values.Count > 0)
            {
                Dictionary<Deck, double> decks = new Dictionary<Deck, double>();

                int i = 0;
                while(true)
                {
                    if (values[0][i].ToString() == "") { 
                        break;
                    }
                    i++;
                }
                i++;
                foreach(IList<object> row in values.Skip(1))
                {
                    double deckWinPercent;
                    if(row.Count > i && double.TryParse(row[i].ToString(), out deckWinPercent))
                    {
                        Deck deck = new Deck();
                        deck.Name = row[0].ToString();
                        deck.UpdateDateString = values[0][0].ToString();

                        if (deckWinPercent >= 0.55)
                        {
                            deck.Tier = 1;
                        }
                        else if(deckWinPercent >= 0.50) {
                            deck.Tier = 2;
                        }
                        else if (deckWinPercent >= 0.45)
                        {
                            deck.Tier = 3;
                        }
                        else if (deckWinPercent >= 0.40)
                        {
                            deck.Tier = 4;
                        }
                        else
                        {
                            deck.Tier = 5;
                        }
                        decks.Add(deck, deckWinPercent);
                    }
                    else
                    {
                        toReturn = decks.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
                        break;
                    }
                }
            }
            return toReturn;
        }

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();

            foreach(Deck tempDeck in GetDeckRanks())
            {
                string deckClass = tempDeck.Name.Split(' ').Last();

                string deckUrl = DECK_URL.Replace("{deckName}", tempDeck.Name);

                Deck deck = GetDeck(deckUrl);
                if(deck == null) continue;
                deck.Source = SourceEnum.ViciousSyndicate;
                deck.Name = tempDeck.Name;
                deck.Tier = tempDeck.Tier;
                deck.Class = deckClass;

                toReturn.Add(deck);
            }

            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            HtmlNode graph = doc.DocumentNode.SelectSingleNode("//*[@id=\"graph\"]/script");
            if(graph == null) return null;
            string script = graph.InnerHtml;

            dynamic cardJson = Json.Decode(script.Split(new[] { "var n = " }, StringSplitOptions.None)[1].Split(';')[0]);

            foreach (KeyValuePair<string, dynamic> cardObj in cardJson)
            {
                Card card = MyCollection.Get(cardObj.Key);
                if(card != null)
                {
                    toReturn.Cards.Add(card, card.Own + card.Missing);
                }
            }

            return toReturn;
        }
    }
}
