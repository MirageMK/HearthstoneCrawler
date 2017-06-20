using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace HSCore.Extensions
{
    class GoogleSheetsWraper
    {
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string APPLICATION_NAME = "Hearthstone Crawler";

        public static SheetsService GetSheetsService()
        {
            SheetsService service = new SheetsService(new BaseClientService.Initializer
            {
                ApiKey = ConfigurationManager.AppSettings["APIKey"]
            });

            if (ConfigurationManager.AppSettings["Environment"] == "Debug")
            {
                UserCredential credential;

                using (FileStream stream =
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
