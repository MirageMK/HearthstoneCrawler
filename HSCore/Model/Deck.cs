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
        public DeckType DeckType => Cards.Any(x => !x.Key.IsStandard) ? DeckType.Wild : DeckType.Standard;

        public SourceEnum Source { get; set; }
        public string Url { get; set; }

        private DateTime _updateDate;
        public DateTime UpdateDate => _updateDate;
        public string UpdateDateString
        {
            set
            {
                if (!DateTime.TryParse(value, out _updateDate))
                {
                    _updateDate = DateTime.Now;
                }
            }
        }

        public string Id => Name + Source + DeckType;
        public int Dust => Cards.Sum(x => x.Value * x.Key.Dust);
        public int MyDust => Cards.Sum(x => (x.Value - x.Key.Own < 0 ? 0 : x.Value - x.Key.Own) * x.Key.Dust);
        public int MissingCardNo => Cards.Sum(x => x.Value - x.Key.Own < 0 ? 0 : x.Value - x.Key.Own);
        public string DuplicateIndicatior => $"{Class};D{Dust};C{Cards.Count};L{Cards.Count(x => x.Key.IsLegendary)};MD{MyDust}";

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

        public override string ToString()
        {
            return Name;
        }
    }
}
