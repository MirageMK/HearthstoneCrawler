using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using HSCore.Extensions;
using HSCore.Model;

namespace HSCore.Writers
{
    public class GoogleSpreedsheetWriter : BaseWriter
    {
        public override int WriteDecks()
        {
            SheetsService service = GoogleSheetsWraper.GetSheetsService();

            string spreadsheetId = "1bWsfZ3bH0wfnCqN6kJQzDTMXLaCzYLWDlao-KSuZFLY";
            string range = "Test!A1:Z";

            List<IList<object>> toBeExported = new List<IList<object>>();


            foreach(Card card in MyCollection.Cards.OrderBy(x => x.CardSet))
            {
                Valuation firstOrDefault = NetDecks.Valuations.FirstOrDefault(x => x.Card == card);
                if(firstOrDefault != null)
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
                service.Spreadsheets.Values.BatchClear(new BatchClearValuesRequest { Ranges = new List<string> { range } }, spreadsheetId);

            clearRequest.Execute();

            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                service.Spreadsheets.Values.Update(vr, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            return request.Execute().UpdatedCells ?? 0;
        }
    }
}