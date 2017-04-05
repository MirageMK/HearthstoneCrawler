using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using HSCore.Extensions;
using HSCore.Model;
using HSCore.Readers;

namespace HSCore
{
    public static class NetDecks
    {
        private static IsolatedStorageFile _isf;
        private const string FILE_NAME = "HSDecks.dat";

        static NetDecks()
        {
            if (Decks == null)
            {
                LoadDecks();
            }

        }

        private static void LoadDecks()
        {
            _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
          IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
          typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));

            DateTimeOffset lastChange = _isf.GetLastWriteTime(FILE_NAME);
            Decks = _isf.LoadObject<List<Deck>>(FILE_NAME);

            if (Decks == null || !lastChange.Date.Equals(DateTime.Now.Date))
            {
                DownloadDecks();
            }
        }

        public static List<Deck> DownloadDecks()
        {
            if (_isf == null)
            {
                _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            }

            Decks = new List<Deck>();

            BaseReader reader = new TempoStormReader();
            Decks.AddRange(reader.GetDecks());
            reader = new HearthstoneTopDecksReader();
            Decks.AddRange(reader.GetDecks());
            reader = new ViciousSyndicateReader();
            Decks.AddRange(reader.GetDecks());
            reader = new MetabombReader();
            Decks.AddRange(reader.GetDecks());
            reader = new DisgusedToastReader();
            Decks.AddRange(reader.GetDecks());

            _isf.SaveObject(Decks, FILE_NAME);

            RecalculateValuations();

            return Decks;
        }

        public static List<Deck> Decks { get; set; }

        private static List<Valuation> _valuations;
        public static List<Valuation> Valuations
        {
            get
            {
                if (_valuations == null)
                    RecalculateValuations();

                return _valuations;
            }
        }

        private static void RecalculateValuations()
        {
            _valuations = new List<Valuation>();
            foreach (Deck deck in Decks)
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
    }
}
