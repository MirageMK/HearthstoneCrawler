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

            _inDecks = new Dictionary<SourceEnum, int>();
            _apperences = new Dictionary<SourceEnum, int>();
            _tierSum = new Dictionary<SourceEnum, int>();
        }

        public Card Card { get; }

        public double Value => Card.ValuationFactor * (6 - GetTierSum() / (float) GetInDecks()) * GetApperences();
        public double AvgValue => (Value1 + Value2 + Value3) / 3;

        public int InDecks1 => GetInDecks(SourceEnum.HearthstoneTopDecks);
        public int Apperences1 => GetApperences(SourceEnum.HearthstoneTopDecks);
        public double TierSum1 => GetTierSum(SourceEnum.HearthstoneTopDecks) / (float) GetInDecks(SourceEnum.HearthstoneTopDecks);
        public double Value1 => GetInDecks(SourceEnum.HearthstoneTopDecks) != 0 ? (Card.ValuationFactor * (6 - GetTierSum(SourceEnum.HearthstoneTopDecks) / (float)GetInDecks(SourceEnum.HearthstoneTopDecks)) * GetApperences(SourceEnum.HearthstoneTopDecks)) : 0;
        public int InDecks2 => GetInDecks(SourceEnum.TempoStorm);
        public int Apperences2 => GetApperences(SourceEnum.TempoStorm);
        public double TierSum2 => GetTierSum(SourceEnum.TempoStorm) / (float)GetInDecks(SourceEnum.TempoStorm);
        public double Value2 => GetInDecks(SourceEnum.TempoStorm) != 0 ? (Card.ValuationFactor * (6 - GetTierSum(SourceEnum.TempoStorm) / (float)GetInDecks(SourceEnum.TempoStorm)) * GetApperences(SourceEnum.TempoStorm)) : 0;
        public int InDecks3 => GetInDecks(SourceEnum.ViciousSyndicate);
        public int Apperences3 => GetApperences(SourceEnum.ViciousSyndicate);
        public double TierSum3 => GetTierSum(SourceEnum.ViciousSyndicate) / (float)GetInDecks(SourceEnum.ViciousSyndicate);
        public double Value3 => GetInDecks(SourceEnum.ViciousSyndicate) != 0 ? (Card.ValuationFactor * (6 - GetTierSum(SourceEnum.ViciousSyndicate) / (float)GetInDecks(SourceEnum.ViciousSyndicate)) * GetApperences(SourceEnum.ViciousSyndicate)) : 0;

        private readonly Dictionary<SourceEnum, int> _inDecks;
        public void SetInDecks(SourceEnum source, int value)
        {
            if (_inDecks.ContainsKey(source))
            {
                _inDecks[source] += value;
            }
            else
            {
                _inDecks.Add(source, value);
            }
        }
        public int GetInDecks(SourceEnum? source = null)
        {
            if (source != null) return _inDecks.ContainsKey(source.Value) ? _inDecks[source.Value] : 0;

            return _inDecks.Aggregate(0, (current, keyValuePair) => current + keyValuePair.Value);
        }

        private readonly Dictionary<SourceEnum, int> _apperences;
        public void SetApperences(SourceEnum source, int value)
        {
            if (_apperences.ContainsKey(source))
            {
                _apperences[source] += value;
            }
            else
            {
                _apperences.Add(source, value);
            }
        }

        public int GetApperences(SourceEnum? source = null)
        {
            if(source != null) return _apperences.ContainsKey(source.Value) ? _apperences[source.Value] : 0;

            return _apperences.Aggregate(0, (current, keyValuePair) => current + keyValuePair.Value);
        }

        private readonly Dictionary<SourceEnum, int> _tierSum;
        public void SetTierSum(SourceEnum source, int value)
        {
            if (_tierSum.ContainsKey(source))
            {
                _tierSum[source] += value;
            }
            else
            {
                _tierSum.Add(source, value);
            }
        }
        public int GetTierSum(SourceEnum? source = null)
        {
            if (source != null) return _tierSum.ContainsKey(source.Value) ? _tierSum[source.Value] : 0;

            return _tierSum.Aggregate(0, (current, keyValuePair) => current + keyValuePair.Value);
        }
    }
}
