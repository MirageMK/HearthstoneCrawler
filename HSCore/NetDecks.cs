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
            Decks = new List<Deck>();
            BaseReader reader = new TempoStormBaseReader();
            Decks.AddRange(reader.GetDecks());
            reader = new HearthstoneTopDecksBaseReader();
            Decks.AddRange(reader.GetDecks());
            reader = new ViciousSyndicateBaseReader();
            Decks.AddRange(reader.GetDecks());

            _isf.SaveObject(Decks, FILE_NAME);

            return Decks;
        }

        public static List<Deck> Decks { get; set; }

        private static List<Valuation> _valuations;
        public static List<Valuation> Valuations
        {
            get
            {
                if(_valuations != null) return _valuations;

                _valuations = new List<Valuation>();

                foreach(Deck deck in Decks)
                {
                    foreach(KeyValuePair<Card, int> dCard in deck.Cards)
                    {
                        Valuation v = _valuations.Find(x => x.Card == dCard.Key);
                        if(v == null)
                        {
                            v = new Valuation(dCard.Key);
                            _valuations.Add(v);
                        }
                        v.SetInDecks(deck.Source, 1);
                        v.SetApperences(deck.Source, dCard.Value);
                        v.SetTierSum(deck.Source, deck.Tier);
                    }
                }
                return _valuations;
            }
        }
    }
}
