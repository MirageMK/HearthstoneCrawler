using System;
using System.Collections.Generic;
using System.Linq;

namespace HSCore.Model
{
    [ Serializable ]
    public class Pack
    {
        private static readonly double[] P = { 0.7159, 0.2291, 0.0439, 0.0111 };

        private Dictionary<string, double> _splitedValueN;

        private Dictionary<string, double> _splitedValueW;

        public Pack(SetEnum set)
        {
            Set = set;
            Cards = MyCollection.Cards.Where(x => x.CardSetEnum == set).ToList();
        }

        public SetEnum Set { get; }
        private List<Card> Cards { get; }
        public bool CanBuy => Set != SetEnum.Naxx && 
                              Set != SetEnum.BRM && 
                              Set != SetEnum.LoE && 
                              Set != SetEnum.Kara && 
                              Set != SetEnum.Basic && 
                              Set != SetEnum.HoF &&
                              Set != SetEnum.GA &&
                              Set != SetEnum.DHI ;

        private Dictionary<string, double> SplitedValueW
        {
            get
            {
                if(_splitedValueW != null) return _splitedValueW;

                _splitedValueW = new Dictionary<string, double>();
                foreach(Card card in Cards)
                    if(_splitedValueW.ContainsKey(card.Rarity))
                        _splitedValueW[card.Rarity] += card.MissingW;
                    else
                        _splitedValueW.Add(card.Rarity, card.MissingW);

                return _splitedValueW;
            }
        }

        private Dictionary<string, double> SplitedValueN
        {
            get
            {
                if(_splitedValueN != null) return _splitedValueN;

                _splitedValueN = new Dictionary<string, double>();
                foreach(Card card in Cards)
                    if(_splitedValueN.ContainsKey(card.Rarity))
                        _splitedValueN[card.Rarity] += card.Missing;
                    else
                        _splitedValueN.Add(card.Rarity, card.Missing);

                return _splitedValueN;
            }
        }

        public double ValueW
        {
            get
            {
                double[] e =
                {
                    GetValueForRarity("Common", true),
                    GetValueForRarity("Rare", true),
                    GetValueForRarity("Epic", true),
                    GetValueForRarity("Legendary", true)
                };
                return SumProduct(e, P) * 5;
            }
        }

        public double ValueN
        {
            get
            {
                double[] e =
                {
                    GetValueForRarity("Common", false),
                    GetValueForRarity("Rare", false),
                    GetValueForRarity("Epic", false),
                    GetValueForRarity("Legendary", false)
                };
                return SumProduct(e, P) * 5;
            }
        }

        public double ProbabilityW
        {
            get
            {
                double[] e =
                {
                    GetProbabilityForNewCard("Common", true),
                    GetProbabilityForNewCard("Rare", true),
                    GetProbabilityForNewCard("Epic", true),
                    GetProbabilityForNewCard("Legendary", true)
                };

                return (1 - Math.Pow(1 - SumProduct(e, P), 5)) * 100;
            }
        }

        public double ProbabilityN
        {
            get
            {
                double[] e =
                {
                    GetProbabilityForNewCard("Common", false),
                    GetProbabilityForNewCard("Rare", false),
                    GetProbabilityForNewCard("Epic", false),
                    GetProbabilityForNewCard("Legendary", false)
                };
                return (1 - Math.Pow(1 - SumProduct(e, P), 5)) * 100;
            }
        }

        private double GetProbabilityForNewCard(string rarity, bool isW)
        {
            if(isW)
            {
                if(!SplitedValueW.ContainsKey(rarity)) return 0;
            }
            else
            {
                if (!SplitedValueN.ContainsKey(rarity)) return 0;
            }

            if(rarity == "Legendary") //cannot open duplicate legendary
            {
                int missingCards = Cards.Where(x => x.Rarity == rarity && x.Missing > 0).Sum(x => x.MaxInDeck);
                if(missingCards != 0)
                {
                    return (isW ? SplitedValueW[rarity] : SplitedValueN[rarity]) / missingCards;
                }
                return (isW ? SplitedValueW[rarity] : SplitedValueN[rarity]) / Cards.Where(x => x.Rarity == rarity).Sum(x => x.MaxInDeck);
            }
            return (isW ? SplitedValueW[rarity] : SplitedValueN[rarity]) / Cards.Where(x => x.Rarity == rarity).Sum(x => x.MaxInDeck);
        }

        private double GetValueForRarity(string rarity, bool isW)
        {
            double x = GetProbabilityForNewCard(rarity, isW);
            switch(rarity)
            {
                case "Common":
                    return x * 40 + (1 - x) * 5;
                case "Rare":
                    return x * 100 + (1 - x) * 20;
                case "Epic":
                    return x * 400 + (1 - x) * 100;
                case "Legendary":
                    return x * 1600 + (1 - x) * 400;
            }
            return x * 40 + (1 - x) * 5;
        }

        private double SumProduct(double[] arrayA, double[] arrayB)
        {
            double result = 0;
            for(int i = 0; i < arrayA.Count(); i++)
                result += arrayA[i] * arrayB[i];
            return result;
        }
    }
}