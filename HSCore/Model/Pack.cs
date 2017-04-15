using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [Serializable]
    public class Pack
    {
        public Pack(SetEnum set)
        {
            Set = set;
            Cards = MyCollection.Cards.Where(x => x.CardSetEnum == set).ToList();
            SplitedValueW = new Dictionary<string, double>();
            SplitedValueN = new Dictionary<string, double>();
        }

        public SetEnum Set { get;}
        public bool CanBuy => Set < SetEnum.HoF || Set > SetEnum.LoE;

        private Dictionary<string, double> SplitedValueW;
        private Dictionary<string, double> SplitedValueN;

        private List<Card> Cards { get; }

        public double ValueW
        {
            get
            {
                foreach(Card card in Cards)
                {
                    if(SplitedValueW.ContainsKey(card.Rarity))
                    {
                        SplitedValueW[card.Rarity] += card.OwnW;
                    }
                    else
                    {
                        SplitedValueW.Add(card.Rarity, card.OwnW);
                    }
                }
                return 0;
            }
        }

        public double ValueN
        {
            get
            {
                foreach (Card card in Cards)
                {
                    if (SplitedValueN.ContainsKey(card.Rarity))
                    {
                        SplitedValueN[card.Rarity] += card.Own;
                    }
                    else
                    {
                        SplitedValueN.Add(card.Rarity, card.Own);
                    }
                }
                return 0;
            }
        }
    }
}
