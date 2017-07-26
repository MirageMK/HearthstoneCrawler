using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HSCore.Extensions;
using HSCore.Model;
using HSCore.Readers;
using log4net;

namespace HSCore
{
    public static class NetDecks
    {
        private const string FILE_NAME = "HSDecks.dat";

        private static readonly ILog log = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        private static IsolatedStorageFile _isf;

        private static List<Deck> _decks;

        private static List<Valuation> _valuations;

        public static bool IsDownloading { get; private set; }

        public static event Action<string> ProgressChanged;

        public static List<Deck> Decks => _decks ?? LoadDecks();

        public static List<Valuation> Valuations
        {
            get
            {
                if(_decks == null)
                    LoadDecks();
                if(_valuations == null)
                    RecalculateValuations();

                return _valuations;
            }
        }

        private static List<Deck> LoadDecks()
        {
            _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
                                                IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
                                                typeof(Url),
                                                typeof(Url));

            DateTimeOffset lastChange = _isf.GetLastWriteTime(FILE_NAME);
            _decks = _isf.LoadObject<List<Deck>>(FILE_NAME);
            
            if (_decks == null || lastChange.AddMinutes(5) < DateTime.Now)
                return DownloadDecks();
            return _decks;
        }

        public static List<Deck> DownloadDecks()
        {
            if (ProgressChanged == null) ProgressChanged = s => { };
            if (_isf == null)
                _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
                                                    IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
                                                    typeof(Url),
                                                    typeof(Url));

            _decks = new List<Deck>();
            if (IsDownloading) return _decks;
            log.Info("START");
            IsDownloading = true;
            ProgressChanged($"Downloading Decks from 6 sources.");
            BaseReader reader = new TempoStormReader();
            List<Deck> decks = reader.GetDecks();
            string message = $"TempoStormReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count}";
            log.Info(message);
            _decks.AddRange(decks);
            ProgressChanged($"(1/6) Downloading Decks...     {decks.Count} added. Total:{_decks.Count}");
            reader = new HearthstoneTopDecksReader();
            decks = reader.GetDecks();
            message = $"HearthstoneTopDecksReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count}";
            log.Info(message);
            _decks.AddRange(decks);
            ProgressChanged($"(2/6) Downloading Decks...     {decks.Count} added. Total:{_decks.Count}");
            reader = new ViciousSyndicateReader();
            decks = reader.GetDecks();
            message = $"ViciousSyndicateReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count}";
            log.Info(message);
            _decks.AddRange(decks);
            ProgressChanged($"(3/6) Downloading Decks...     {decks.Count} added. Total:{_decks.Count}");
            reader = new MetabombReader();
            decks = reader.GetDecks();
            message = $"MetabombReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count}";
            log.Info(message);
            _decks.AddRange(decks);
            ProgressChanged($"(4/6) Downloading Decks...     {decks.Count} added. Total:{_decks.Count}");
            reader = new DisgusedToastReader();
            decks = reader.GetDecks();
            message = $"DisgusedToastReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count}";
            log.Info(message);
            _decks.AddRange(decks);
            ProgressChanged($"(5/6) Downloading Decks...     {decks.Count} added. Total:{_decks.Count}");
            reader = new HSReplayReader();
            decks = reader.GetDecks();
            message = $"HSReplayReader - Standard:{decks.Count(x => x.DeckType == DeckType.Standard)} - Wild:{decks.Count(x => x.DeckType == DeckType.Wild)} - Total:{decks.Count}";
            log.Info(message);
            _decks.AddRange(decks);
            ProgressChanged($"(6/6) Downloading Decks...     {decks.Count} added. Total:{_decks.Count}");

            _isf.SaveObject(_decks, FILE_NAME);

            ProgressChanged($"Recalculating Valuations...");
            RecalculateValuations();
            IsDownloading = false;

            log.Info("FINISH");
            return _decks;
        }

        private static void RecalculateValuations()
        {
            _valuations = new List<Valuation>();
            foreach(Deck deck in _decks)
            foreach(KeyValuePair<Card, int> dCard in deck.Cards)
            {
                Valuation v = _valuations.Find(x => x.Card == dCard.Key);
                if(v == null)
                {
                    v = new Valuation(dCard.Key);
                    _valuations.Add(v);
                }
                v.Decks.Add(deck, dCard.Value);
            }
        }

        public static string GetWeightedFeed()
        {
            StringBuilder toBeReturned = new StringBuilder();
            toBeReturned.AppendLine("PACT_WF 1.3");
            double maxValuationValue = Valuations.Max(x => x.Value);
            foreach(Card card in MyCollection.Cards)
            {
                double scaledValue = 0;
                Valuation firstOrDefault = Valuations.FirstOrDefault(x => x.Card == card);
                if(firstOrDefault != null)
                    scaledValue = firstOrDefault.Value / maxValuationValue;

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