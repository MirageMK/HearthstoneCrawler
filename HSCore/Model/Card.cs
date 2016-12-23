using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [Serializable]
    public class Card : IEquatable<Card>, IEqualityComparer<Card>
    {
        public string CardId { get; set; }

        private string _name;

        public string Name
        {
            get
            {
                switch (_name)
                {
                    case "King Krush":
                        return "King Crush";
                    case "One-eyed Cheat":
                        return "One-Eyed Cheat";
                    default:
                        return _name;
                }
            }
            set { _name = value; }
        }

        public string CardSet { get; set; }
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
        public int Missing => Rarity == "Legendary" ? 1 - Own : 2 - Own;

        public bool Equals(Card other)
        {
            return other != null && other.Name == this.Name;
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
    }

    [Serializable]
    public class Mechanic
    {
        public string Name { get; set; }
    }
}
