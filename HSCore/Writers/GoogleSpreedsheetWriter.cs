using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using HSCore.Model;

namespace HSCore.Writers
{
    public class GoogleSpreedsheetWriter : BaseWriter
    {
        public override int WriteDecks()
        {
            var service = GetService();

            string spreadsheetId = "1bWsfZ3bH0wfnCqN6kJQzDTMXLaCzYLWDlao-KSuZFLY";
            string range = "Test!A1:Z";

            List<IList<object>> toBeExported = new List<IList<object>>();


            foreach (Card card in MyCollection.Cards.OrderBy(x => x.CardSet))
            {
                Valuation firstOrDefault = NetDecks.Valuations.FirstOrDefault(x => x.Card == card);
                if (firstOrDefault != null)
                {
                    toBeExported.Add(firstOrDefault.ToValuationArray());
                }
                else
                {
                    List<object> tempList = new List<object> { card.Name, 0, card.Own, card.Own + card.Missing * (1 - 0), card.MaxInDeck };
                    toBeExported.Add(tempList);
                }
            }

            ValueRange vr = new ValueRange
            {
                Values = toBeExported,
                MajorDimension = "ROWS"
            };

            SpreadsheetsResource.ValuesResource.BatchClearRequest clearRequest =
                service.Spreadsheets.Values.BatchClear(new BatchClearValuesRequest() { Ranges = new List<string>() { range } }, spreadsheetId);

            clearRequest.Execute();

            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                    service.Spreadsheets.Values.Update(vr, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            return request.Execute().UpdatedCells ?? 0;
        }

        private SheetsService GetService()
        {
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Hearthstone Crawler";

            UserCredential credential;

            string credPath = Environment.GetFolderPath(
                Environment.SpecialFolder.Personal);

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = @"910604975065-l7l1l4o1r6b30mot8310vcsckl3k79qt.apps.googleusercontent.com",
                    ClientSecret = @"eugcRbe0V6IlSREi1HTb4xF6"
                },
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;

            // Create Google Sheets API service.
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}
