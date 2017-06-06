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
                        return 817;
                    case "Warior":
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
                        return 1066;
                    case "Paladin":
                        return 671;
                    case "Rogue":
                        return 930;
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
            var sb = new StringBuilder();
            sb.AppendLine("### " + (string.IsNullOrEmpty(Name) ? Class + " Deck" : Name));
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(0);
                var bytes = VarintBitConverter.GetVarintBytes(1);
                ms.Write(bytes, 0, bytes.Length);
                bytes = VarintBitConverter.GetVarintBytes(DeckType == DeckType.Standard ? 2 : 1);
                ms.Write(bytes, 0, bytes.Length);
                bytes = VarintBitConverter.GetVarintBytes(1);
                ms.Write(bytes, 0, bytes.Length);
                bytes = VarintBitConverter.GetVarintBytes(ClassID);
                ms.Write(bytes, 0, bytes.Length);

                List<KeyValuePair<Card, int>> cards = Cards.OrderBy(x => x.Value).ToList();
                List<KeyValuePair<Card, int>> singleCopy = cards.Where(x => x.Value == 1).ToList();
                List<KeyValuePair<Card, int>> doubleCopy = cards.Where(x => x.Value == 2).ToList();
                List<KeyValuePair<Card, int>> nCopy = cards.Where(x => x.Value > 2).ToList();

                bytes = VarintBitConverter.GetVarintBytes(singleCopy.Count);
                ms.Write(bytes, 0, bytes.Length);
                foreach(var card in singleCopy)
                {
                    bytes = VarintBitConverter.GetVarintBytes(card.Key.DBId);
                    ms.Write(bytes, 0, bytes.Length);
                }

                bytes = VarintBitConverter.GetVarintBytes(doubleCopy.Count);
                ms.Write(bytes, 0, bytes.Length);
                foreach (var card in doubleCopy)
                {
                    bytes = VarintBitConverter.GetVarintBytes(card.Key.DBId);
                    ms.Write(bytes, 0, bytes.Length);
                }

                bytes = VarintBitConverter.GetVarintBytes(nCopy.Count);
                ms.Write(bytes, 0, bytes.Length);
                foreach (var card in nCopy)
                {
                    bytes = VarintBitConverter.GetVarintBytes(card.Key.DBId);
                    ms.Write(bytes, 0, bytes.Length);
                    bytes = VarintBitConverter.GetVarintBytes(card.Value);
                    ms.Write(bytes, 0, bytes.Length);
                }

                sb.AppendLine(Convert.ToBase64String(ms.ToArray()));
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}