using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSCore;
using HSCore.Model;
using HSCore.Readers;

namespace HSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseReader r = new TempoStormBaseReader();
            
            foreach (Deck deck in r.GetDecks())
            {
                //Console.WriteLine(card.Name + " " + card.Own);
            }

            Console.WriteLine(MyCollection.Cards.Count);
            Console.ReadLine();
        }
    }
}
