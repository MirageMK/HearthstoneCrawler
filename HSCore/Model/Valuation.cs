using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [ Serializable ]
    public class Valuation
    {
        private double? _avgValue;

        private double? _value;

        public Valuation(Card card)
        {
            Card = card;

            Decks = new Dictionary<Deck, int>();
        }

        public Card Card { get; }

        public Dictionary<Deck, int> Decks { get; }

        public int ApperanceInT1Decks => Decks.Where(x => x.Key.Tier == 1).GroupBy(x => x.Key.DuplicateIndicatior).Count();

        public double Value
        {
            get
            {
                if(_value == null)
                    _value = GetValue();
                return _value.Value;
            }
        }

        public double NormalizedValue => GetValue() / NetDecks.Valuations.Max(x => x.Value);

        public double NormalizedValueAdjusted
        {
            get
            {
                double value = GetValue() / NetDecks.Valuations.Where(x => x.Card.Missing > 0).Max(x => x.Value);
                if(value > 1) value = 1;
                return value;
            }
        }

        public double AvgValue
        {
            get
            {
                if(_avgValue == null)
                    _avgValue = (Value1 + Value2 + Value3 + Value4 + Value5 + Value6) / 6;
                return _avgValue.Value;
            }
        }

        public double Value1 => GetValue(SourceEnum.HearthstoneTopDecks);
        public double Value2 => GetValue(SourceEnum.TempoStorm);
        public double Value3 => GetValue(SourceEnum.ViciousSyndicate);
        public double Value4 => GetValue(SourceEnum.Metabomb);
        public double Value5 => GetValue(SourceEnum.DisguisedToast);
        public double Value6 => GetValue(SourceEnum.HSReplay);

        public int GetInDecks(SourceEnum? source = null)
        {
            return source == null
                       ? Decks.GroupBy(x => x.Key.DuplicateIndicatior).Count()
                       : Decks.Count(x => x.Key.Source == source.Value);
        }

        public double GetApperences(SourceEnum? source = null)
        {
            return source == null
                       ? Decks.GroupBy(x => x.Key.DuplicateIndicatior).Sum(x => x.Average(y => y.Value))
                       : Decks.Where(x => x.Key.Source == source.Value).Sum(x => x.Value);
        }

        public double GetTier(SourceEnum? source = null)
        {
            return source == null
                       ? Decks.GroupBy(x => x.Key.DuplicateIndicatior).Sum(x => x.Average(y => y.Key.Tier)) / GetInDecks()
                       : Decks.Where(x => x.Key.Source == source.Value).Sum(x => x.Key.Tier) / (double) GetInDecks(source);
        }

        public double GetValue(SourceEnum? source = null)
        {
            int wildCount = source == null
                                ? Decks.Where(x => x.Key.DeckType != DeckType.Standard).GroupBy(x => x.Key.DuplicateIndicatior).Count()
                                : Decks.Count(x => x.Key.Source == source.Value && x.Key.DeckType != DeckType.Standard);

            double wildReduceFactor = 1 - wildCount / (GetInDecks(source) * 2.0);

            return source == null
                       ? Card.ValuationFactor * (6 - GetTier()) * GetApperences() * wildReduceFactor
                       : GetInDecks(source) != 0
                           ? Card.ValuationFactor * (6 - GetTier(source)) * GetApperences(source) * wildReduceFactor
                           : 0;
        }

        public List<object> ToValuationArray()
        {
            List<object> toReturn = new List<object>();
            toReturn.Add(Card.Name);
            toReturn.Add(NormalizedValue);
            toReturn.Add(Card.Own);
            toReturn.Add(Card.Own + Card.Missing * (1 - NormalizedValue));
            toReturn.Add(Card.MaxInDeck);

            return toReturn;
        }
    }
}