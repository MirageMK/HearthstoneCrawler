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

        public double Value => Card.ValuationFactor * (6 - GetTier()) * GetApperences();
        public double AvgValue => (Value1 + Value2 + Value3 + Value4) / 4;

        public int InDecks1 => GetInDecks(SourceEnum.HearthstoneTopDecks);
        public int Apperences1 => (int)GetApperences(SourceEnum.HearthstoneTopDecks);
        public double TierSum1 => GetTier(SourceEnum.HearthstoneTopDecks);
        public double Value1 => GetInDecks(SourceEnum.HearthstoneTopDecks) != 0 ? Card.ValuationFactor * (6 - GetTier(SourceEnum.HearthstoneTopDecks)) * GetApperences(SourceEnum.HearthstoneTopDecks) : 0;
        public int InDecks2 => GetInDecks(SourceEnum.TempoStorm);
        public int Apperences2 => (int)GetApperences(SourceEnum.TempoStorm);
        public double TierSum2 => GetTier(SourceEnum.TempoStorm);
        public double Value2 => GetInDecks(SourceEnum.TempoStorm) != 0 ? Card.ValuationFactor * (6 - GetTier(SourceEnum.TempoStorm)) * GetApperences(SourceEnum.TempoStorm) : 0;
        public int InDecks3 => GetInDecks(SourceEnum.ViciousSyndicate);
        public int Apperences3 => (int)GetApperences(SourceEnum.ViciousSyndicate);
        public double TierSum3 => GetTier(SourceEnum.ViciousSyndicate);
        public double Value3 => GetInDecks(SourceEnum.ViciousSyndicate) != 0 ? Card.ValuationFactor * (6 - GetTier(SourceEnum.ViciousSyndicate)) * GetApperences(SourceEnum.ViciousSyndicate) : 0;
        public int InDecks4 => GetInDecks(SourceEnum.Metabomb);
        public int Apperences4 => (int)GetApperences(SourceEnum.Metabomb);
        public double TierSum4 => GetTier(SourceEnum.Metabomb);
        public double Value4 => GetInDecks(SourceEnum.Metabomb) != 0 ? Card.ValuationFactor * (6 - GetTier(SourceEnum.Metabomb)) * GetApperences(SourceEnum.Metabomb) : 0;

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
    }
}
