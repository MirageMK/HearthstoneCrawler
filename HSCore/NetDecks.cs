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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public static bool IsDownloading { get; private set; }

        public static List<Deck> DownloadDecks()
        {
            if (_isf == null)
            {
                _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            }

            _decks = new List<Deck>();

            if(IsDownloading) return _decks;
            log.Info("START");
            IsDownloading = true;

            try
            {
                BaseReader reader = new TempoStormReader();
                List<Deck> decks = reader.GetDecks();
                log.Info($"TempoStormReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count()}");
                _decks.AddRange(decks);
                reader = new HearthstoneTopDecksReader();
                decks = reader.GetDecks();
                log.Info($"HearthstoneTopDecksReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count()}");
                _decks.AddRange(decks);
                reader = new ViciousSyndicateReader();
                decks = reader.GetDecks();
                log.Info($"ViciousSyndicateReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count()}");
                _decks.AddRange(decks);
                reader = new MetabombReader();
                decks = reader.GetDecks();
                log.Info($"MetabombReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count()}");
                _decks.AddRange(decks);
                reader = new DisgusedToastReader();
                decks = reader.GetDecks();
                log.Info($"DisgusedToastReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count()}");
                _decks.AddRange(decks);
                reader = new HSReplayReader();
                decks = reader.GetDecks();
                log.Info($"HSReplayReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count()}");
                _decks.AddRange(decks);

                _isf.SaveObject(_decks, FILE_NAME);

                RecalculateValuations();
                IsDownloading = false;
            }
            catch(Exception ex)
            {
                log.Error("Problem", ex);
            }
            log.Info("FINISH");
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
                if (_valuations == null)
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
