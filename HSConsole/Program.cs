using System;
using System.Collections.Generic;
using System.Linq;
using HSCore;
using HSCore.Model;
using HSCore.Readers;

namespace HSConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var decks = new HSReplayReader();

            var t1Decks = decks.GetDecks();

            List<Card> cards = new List<Card>();
            foreach(Deck t1Deck in t1Decks)
            {
                cards.AddRange(t1Deck.Cards.Keys.Where(x => x.Missing > 0));
            }

            var uniqueCards = cards.Distinct();


            /*var temp = new HearthstoneTopDecksReader();
            var x = temp.GetDecks();*/

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}