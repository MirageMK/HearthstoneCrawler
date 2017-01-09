using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            foreach(Deck deck in NetDecks.Decks)
            {
                if(deck.Source == SourceEnum.TempoStorm)
                {
                    toBeExported.AddRange(deck.ToMatrix());
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
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}
