﻿using System;
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
using HSCore.Model;

namespace HSCore.Writers
{
    public class GoogleSpreedsheetWriter : BaseWriter
    {
        private const string APPLICATION_NAME = "Hearthstone Crawler";
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

        public override int WriteDecks()
        {
            SheetsService service = GetService();

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

        private SheetsService GetService()
        {
            SheetsService service = new SheetsService(new BaseClientService.Initializer
                                                      {
                                                          ApiKey = ConfigurationManager.AppSettings["APIKey"]
                                                      });

            if(ConfigurationManager.AppSettings["Environment"] == "Debug")
            {
                UserCredential credential;

                using(FileStream stream =
                    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(
                                                                Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/HSC");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                                                             GoogleClientSecrets.Load(stream).Secrets,
                                                                             Scopes,
                                                                             "user",
                                                                             CancellationToken.None,
                                                                             new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Sheets API service.
                service = new SheetsService(new BaseClientService.Initializer
                                            {
                                                HttpClientInitializer = credential,
                                                ApplicationName = APPLICATION_NAME
                                            });
            }

            return service;
        }
    }
}