using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using HSCore.Extensions;
using HSCore.Model;
using HtmlAgilityPack;
using log4net;

namespace HSCore.Readers
{
    public class ViciousSyndicateReader : BaseReader
    {
        private const string DECK_URL = "http://www.vicioussyndicate.com/deck-library/{class}-decks/{deckName}/";
        private const string spreadsheetId = "1osCVci8-7ttXp_CjWORzEUYf5VQlGWN_ZsOUrbCX0AI";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        private List<Deck> GetDeckRanks()
        {
            List<Deck> toReturn = new List<Deck>();

            SheetsService service = GoogleSheetsWraper.GetSheetsService();

            // Define request parameters.
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
                    if(values[0][i].ToString() == "")
                        break;
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

                        if(deckWinPercent >= 0.55)
                            deck.Tier = 1;
                        else if(deckWinPercent >= 0.50)
                            deck.Tier = 2;
                        else if(deckWinPercent >= 0.45)
                            deck.Tier = 3;
                        else if(deckWinPercent >= 0.40)
                            deck.Tier = 4;
                        else
                            deck.Tier = 5;
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

            try
            {
                foreach(Deck tempDeck in GetDeckRanks())
                {
                    string deckClass = tempDeck.Name.Split(' ').Last();

                    string deckUrl = DECK_URL.Replace("{class}", deckClass).Replace("{deckName}", tempDeck.Name);

                    Deck deck = GetDeck(deckUrl);
                    if(deck == null) continue;
                    deck.Source = SourceEnum.ViciousSyndicate;
                    deck.Name = tempDeck.Name;
                    deck.Tier = tempDeck.Tier;
                    deck.UpdateDateString = tempDeck.UpdateDate.ToString();
                    deck.Class = deckClass;

                    toReturn.Add(deck);
                }
            }
            catch(Exception ex)
            {
                log.Error("Problem", ex);
            }

            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            toReturn.Url = url;
            HtmlNode deckLink = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'article-content')]/p/a/img");
            if(deckLink == null) deckLink = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'entry-content')]/p/a/img");
            if(deckLink == null)
            {
                log.Warn($"Cannot find deck on {url}");
                return null;
            }
            string temp = deckLink.ParentNode.GetAttributeValue("href", string.Empty);
            doc = web.Load(temp);

            HtmlNode cardsMeta = doc.DocumentNode.SelectSingleNode("//meta[@property='x-hearthstone:deck:cards']");
            if(cardsMeta == null)
            {
                log.Warn($"Cannot find cards on {temp}");
                return null;
            }
            string cardsString = cardsMeta.GetAttributeValue("content", string.Empty);

            foreach(string cardID in cardsString.Split(','))
            {
                Card card = MyCollection.GetByID(cardID);
                if(toReturn.Cards.ContainsKey(card))
                    toReturn.Cards[card]++;
                else
                    toReturn.Cards.Add(card, 1);
            }

            return toReturn;
        }

        public Object GetBestDeck()
        {
            SheetsService service = GoogleSheetsWraper.GetSheetsService();

            // Define request parameters.
            string range = "'Classes/Rank Overview'!N1:X22";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;
            return values;
        }
    }
}