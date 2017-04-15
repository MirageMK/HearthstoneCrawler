using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [Serializable]
    public class Card : IEquatable<Card>, IEqualityComparer<Card>
    {
        public string CardId { get; set; }

        public string Name { get; set; }
        public string CardSet { get; set; }
        public SetEnum CardSetEnum => Enums.GetValueFromDescription<SetEnum>(CardSet);
        public string Type { get; set; }
        public string Faction { get; set; }
        public string Rarity { get; set; }
        public int Cost { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public string Text { get; set; }
        public string Flavor { get; set; }
        public string Artist { get; set; }
        public bool Collectible { get; set; }
        public bool Elite { get; set; }
        public string HowToGet { get; set; }
        public string HowToGetGold { get; set; }
        public string PlayerClass { get; set; }
        public string Race { get; set; }
        public string Img { get; set; }
        public string ImgGold { get; set; }
        public string Locale { get; set; }
        public List<Mechanic> Mechanics { get; set; }
        public int Dust
        {
            get
            {
                switch (Rarity)
                {
                    case "Free":
                        return 0;
                    case "Common":
                        return 40;
                    case "Rare":
                        return 100;
                    case "Epic":
                        return 400;
                    case "Legendary":
                        return 1600;
                    default:
                        return 0;
                }
            }
        }
        public int Own { get; set; }
        public double OwnW
        {
            get
            {
                Valuation firstOrDefault = NetDecks.Valuations.FirstOrDefault(x => x.Card.Name == Name);
                if (firstOrDefault != null) return Own + Missing * (1 - firstOrDefault.NormalizedValue);
                else return MaxInDeck;
            }
        }

        public int Missing => MaxInDeck - Own;
        public int MaxInDeck => IsLegendary ? 1 : 2;
        public bool IsLegendary => Rarity == "Legendary";
        public bool IsStandard => CardSetEnum < SetEnum.HoF || CardSetEnum > SetEnum.LoE;

        public double ValuationFactor
        {
            get
            {
                double factor = 1;

                if (!IsStandard) factor -= 0.5;
                if (CardSetEnum > SetEnum.LoE && CardSetEnum < SetEnum.JtU)
                {
                    factor -= (((DateTime.Now.Month + 8) % 12) + 1) * 0.5 / 12;
                }

                if (IsLegendary) factor += 0.25;

                switch (Rarity)
                {
                    case "Epic":
                        factor -= 0.1;
                        break;
                    case "Rare":
                        factor -= 0.2;
                        break;
                    case "Common":
                        factor -= 0.3;
                        break;
                    case "Free":
                        factor -= 0.4;
                        break;
                }

                return factor;
            }
        }

        public bool Equals(Card other)
        {
            return other != null && other.Name == Name;
        }

        public bool Equals(Card x, Card y)
        {
            return x != null && y != null && x.Name == y.Name;
        }

        public int GetHashCode(Card obj)
        {
            return obj.Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public IList<object> ToList()
        {
            return new List<object> { Name, ValuationFactor };
        }
    }

    [Serializable]
    public class Mechanic
    {
        public string Name { get; set; }
    }
}
