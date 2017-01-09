using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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

            // Get the root and file portions of the search string.
            string fileString = Path.GetFileName("*");

            List<String> fileList = new List<String>(_isf.GetFileNames("*"));

            if (Decks == null || !lastChange.Date.Equals(DateTime.Now.Date))
            {
                DownloadDecks();
            }
        }

        public static List<Deck> DownloadDecks()
        {
            if(_isf == null)
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
                    v.SetInDecks(deck.Source, 1);
                    v.SetApperences(deck.Source, dCard.Value);
                    v.SetTierSum(deck.Source, deck.Tier);
                }
            }
        }
    }
}
