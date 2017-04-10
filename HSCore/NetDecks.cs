using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSCore.Extensions;
using HSCore.Model;
using HSCore.Readers;

namespace HSCore
{
    public static class NetDecks
    {
        private static IsolatedStorageFile _isf;
        private const string FILE_NAME = "HSDecks.dat";

        private static List<Deck> LoadDecks()
        {
            _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
          IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
          typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));

            DateTimeOffset lastChange = _isf.GetLastWriteTime(FILE_NAME);
            _decks = _isf.LoadObject<List<Deck>>(FILE_NAME);

            if(_decks == null || !lastChange.Date.Equals(DateTime.Now.Date))
            {
                return DownloadDecks();
            }
            else
            {
                return _decks;
            }
        }

        private static bool isDownloading = false;

        public static List<Deck> DownloadDecks()
        {
            if (_isf == null)
            {
                _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            }

            _decks = new List<Deck>();

            if(isDownloading) return _decks;

            isDownloading = true;
            BaseReader reader = new TempoStormReader();
            _decks.AddRange(reader.GetDecks());
            reader = new HearthstoneTopDecksReader();
            _decks.AddRange(reader.GetDecks());
            reader = new ViciousSyndicateReader();
            _decks.AddRange(reader.GetDecks());
            reader = new MetabombReader();
            _decks.AddRange(reader.GetDecks());
            reader = new DisgusedToastReader();
            _decks.AddRange(reader.GetDecks());

            _isf.SaveObject(_decks, FILE_NAME);

            RecalculateValuations();
            isDownloading = false;

            return _decks;
        }

        private static List<Deck> _decks;
        public static List<Deck> Decks => _decks ?? LoadDecks();

        private static List<Valuation> _valuations;
        public static List<Valuation> Valuations
        {
            get
            {
                if(_decks == null)
                    LoadDecks();
                else if (_valuations == null)
                    RecalculateValuations();

                return _valuations;
            }
        }

        private static void RecalculateValuations()
        {
            _valuations = new List<Valuation>();
            foreach (Deck deck in _decks)
            {
                foreach (KeyValuePair<Card, int> dCard in deck.Cards)
                {
                    Valuation v = _valuations.Find(x => x.Card == dCard.Key);
                    if (v == null)
                    {
                        v = new Valuation(dCard.Key);
                        _valuations.Add(v);
                    }
                    v.Decks.Add(deck, dCard.Value);
                }
            }
        }

        public static string GetWeightedFeed()
        {
            StringBuilder toBeReturned = new StringBuilder();
            toBeReturned.AppendLine("PACT_WF 1.3");
            double maxValuationValue = Valuations.Max(x => x.Value);
            foreach (Card card in MyCollection.Cards)
            {
                double scaledValue = 0;
                Valuation firstOrDefault = Valuations.FirstOrDefault(x => x.Card == card);
                if (firstOrDefault != null)
                {
                    scaledValue = firstOrDefault.Value / maxValuationValue;
                }

                toBeReturned.AppendLine($"{card.Name}; {scaledValue}; {scaledValue}");
            }
            return toBeReturned.ToString();
        }

        public static async void DownloadDecksAsync()
        {
            await Task.Run(() => DownloadDecks());
        }
    }
}
