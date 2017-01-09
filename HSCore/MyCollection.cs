using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using HSCore.Model;

namespace HSCore
{
    public static class MyCollection
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "Hearthstone Crawler";
        internal static readonly string[] NonColectable = { "Roaring Torch", "Tank Up!", "Kazakus Potion", "The Storm Guardian" };

        public static List<Card> Cards { get; }
        public static int CardCount => Cards.Sum(card => card.Own);

        static MyCollection()
        {
            List<Card> toReturn = new List<Card>();

            UserCredential credential;

            using (var stream =
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
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            string spreadsheetId = "1bWsfZ3bH0wfnCqN6kJQzDTMXLaCzYLWDlao-KSuZFLY";
            //string spreadsheetId = "1mBHQp3sd8390K8Nh-sJuTUNA1m114SrLpYqXZREZAEA"; //Kanga
            string range = "HiddenData!B7:P";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (IList<object> row in values)
                {
                    int cardCount;
                    if (row.Count > 14 && int.TryParse(row[14].ToString(), out cardCount))
                    {
                        Card card = HeartstoneDB.Get(row[0].ToString());
                        card.Own = cardCount;
                        toReturn.Add(card);
                    }
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
                default:
                    return name;
            }
        }
    }
}
