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

        public double TEST => GetTierSum(SourceEnum.TempoStorm) / (float) GetInDecks(SourceEnum.TempoStorm);

        public double InDecks => GetInDecks() / (float)_inDecks.Count;
        public double Apperences => GetApperences() / (float) _apperences.Count;
        public double TierSum => GetTierSum() / (float) _tierSum.Count;
        //public double AvgApperences => Apperences / (float)InDecks;
        //public double AvgTier => TierSum / (float)InDecks;

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
            if (source != null) return _inDecks.ContainsKey(source.Value) ? _apperences[source.Value] : 0;

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
