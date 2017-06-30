using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HSCore.Model
{
    [ Serializable ]
    public class Deck : IEquatable<Deck>, IEqualityComparer<Deck>
    {
        private DateTime _updateDate;

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
        public DateTime UpdateDate => _updateDate;

        public string UpdateDateString
        {
            set
            {
                if(!DateTime.TryParse(value, out _updateDate))
                    _updateDate = DateTime.Now;
            }
        }

        public string Id => Name + Source + DeckType;
        public int Dust => Cards.Sum(x => x.Value * x.Key.Dust);
        public int MyDust => Cards.Sum(x => (x.Value - x.Key.Own < 0 ? 0 : x.Value - x.Key.Own) * x.Key.Dust);
        public int MissingCardNo => Cards.Sum(x => x.Value - x.Key.Own < 0 ? 0 : x.Value - x.Key.Own);
        public string DuplicateIndicatior => $"{Class};D{Dust};C{Cards.Count};L{Cards.Count(x => x.Key.IsLegendary)};MD{MyDust}";
        public string DeckCode => ToDeckCode();

        public int ClassID
        {
            get
            {
                switch(Class)
                {
                    case "Priest":
                        return 41887;
                    case "Warrior":
                        return 7;
                    case "Warlock":
                        return 893;
                    case "Mage":
                        return 637;
                    case "Druid":
                        return 274;
                    case "Hunter":
                        return 31;
                    case "Shaman":
                        return 40183;
                    case "Paladin":
                        return 2827;
                    case "Rogue":
                        return 40195;
                    default:
                        return 0;
                }
            }
        }

        public Dictionary<Card, int> Cards { get; set; }

        public bool Equals(Deck x, Deck y)
        {
            return x != null && y != null && x.DuplicateIndicatior == y.DuplicateIndicatior;
        }

        public int GetHashCode(Deck obj)
        {
            return obj.DuplicateIndicatior.GetHashCode();
        }

        public bool Equals(Deck other)
        {
            return other != null && other.DuplicateIndicatior == DuplicateIndicatior;
        }

        public List<IList<object>> ToMatrix()
        {
            List<IList<object>> toReturn = new List<IList<object>>();

            foreach(KeyValuePair<Card, int> keyValuePair in Cards)
            {
                IList<object> row = keyValuePair.Key.ToList();
                row.Add(keyValuePair.Value);
                toReturn.Add(row);
            }

            return toReturn;
        }

        public string ToDeckCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("### " + Name);
            using(MemoryStream ms = new MemoryStream())
            {
                Write(ms, 0);
                Write(ms, 1);
                Write(ms, DeckType == DeckType.Standard ? 2 : 1);
                Write(ms, 1);
                Write(ms, ClassID);

                List<KeyValuePair<Card, int>> cards = Cards.OrderBy(x => x.Value).ToList();
                List<KeyValuePair<Card, int>> singleCopy = cards.Where(x => x.Value == 1).ToList();
                List<KeyValuePair<Card, int>> doubleCopy = cards.Where(x => x.Value == 2).ToList();
                List<KeyValuePair<Card, int>> nCopy = cards.Where(x => x.Value > 2).ToList();

                Write(ms, singleCopy.Count);
                foreach(KeyValuePair<Card, int> card in singleCopy)
                {
                    Write(ms, card.Key.DBId);
                }

                Write(ms, doubleCopy.Count);
                foreach(KeyValuePair<Card, int> card in doubleCopy)
                {
                    Write(ms, card.Key.DBId);
                }

                Write(ms, nCopy.Count);
                foreach(KeyValuePair<Card, int> card in nCopy)
                {
                    Write(ms, card.Key.DBId);
                    Write(ms, card.Value);
                }

                sb.AppendLine(Convert.ToBase64String(ms.ToArray()));
                return sb.ToString();
            }
        }

        private static void Write(MemoryStream ms, int value)
        {
            if(value == 0)
                ms.WriteByte(0);
            else
            {
                byte[] bytes = VarintBitConverter.GetVarintBytes((ulong) value);
                ms.Write(bytes, 0, bytes.Length);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}