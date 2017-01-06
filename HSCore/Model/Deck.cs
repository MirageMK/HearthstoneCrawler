using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [Serializable]
    public class Deck : IEquatable<Deck>, IEqualityComparer<Deck>
    {
        public Deck()
        {
            Cards = new Dictionary<Card, int>();
        }

        public string Name { get; set; }
        public string Class { get; set; }
        public int Tier { get; set; }
        private DeckType _deckType;

        public DeckType DeckType
        {
            get
            {
                if (_deckType != DeckType.Undefined) return _deckType;
                return Cards.Any(x => !x.Key.IsStandard) ? DeckType.Wild : DeckType.Standard;
            }
            set { _deckType = value; }
        }

        public SourceEnum Source { get; set; }
        public string Id => Name + Source + DeckType;
        public int Dust => Cards.Sum(x => x.Value * x.Key.Dust);
        public int MyDust => Cards.Sum(x => (x.Value - x.Key.Own < 0 ? 0 : x.Value - x.Key.Own) * x.Key.Dust);

        public Dictionary<Card, int> Cards { get; set; }
        public bool Equals(Deck other)
        {
            return other != null && other.Name == Name;
        }

        public bool Equals(Deck x, Deck y)
        {
            return x != null && y != null && x.Name == y.Name;
        }

        public int GetHashCode(Deck obj)
        {
            return obj.Name.GetHashCode();
        }

        public List<IList<object>> ToMatrix()
        {
            List<IList<object>> toReturn = new List<IList<object>>();

            foreach (KeyValuePair<Card, int> keyValuePair in Cards)
            {
                IList<object> row = keyValuePair.Key.ToList();
                row.Add(keyValuePair.Value);
                toReturn.Add(row);
            }

            return toReturn;
        }
    }
}
