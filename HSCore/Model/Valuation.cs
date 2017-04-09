using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [Serializable]
    public class Valuation
    {
        public Valuation(Card card)
        {
            Card = card;

            Decks = new Dictionary<Deck, int>();
        }

        public Card Card { get; }
        public Dictionary<Deck, int> Decks { get; }

        public double Value => GetValue();
        public double AvgValue => (Value1 + Value2 + Value3 + Value4 + Value5) / 5;

        public double Value1 => GetValue(SourceEnum.HearthstoneTopDecks);
        public double Value2 => GetValue(SourceEnum.TempoStorm);
        public double Value3 => GetValue(SourceEnum.ViciousSyndicate);
        public double Value4 => GetValue(SourceEnum.Metabomb);
        public double Value5 => GetValue(SourceEnum.DisguisedToast);

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
            return source == null
                       ? Card.ValuationFactor * (6 - GetTier()) * GetApperences()
                       : GetInDecks(source) != 0 ? Card.ValuationFactor * (6 - GetTier(source)) * GetApperences(source) : 0;
        }

        public List<object> ToValuationArray()
        {
            List<object> toReturn = new List<object>();
            var normalizedValue = Value / NetDecks.Valuations.Max(x => x.Value);
            toReturn.Add(Card.Name);
            toReturn.Add(normalizedValue);
            toReturn.Add(Card.Own);
            toReturn.Add(Card.Own + Card.Missing * (1 - normalizedValue));
            toReturn.Add(Card.MaxInDeck);

            return toReturn;
        }
    }
}
