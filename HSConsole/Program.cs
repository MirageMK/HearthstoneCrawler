using System;
using System.Collections.Generic;
using HSCore;
using HSCore.Model;
using HSCore.Writers;

namespace HSConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<Card> tempa = MyCollection.Cards;
            NetDecks.DownloadDecks();
            GoogleSpreedsheetWriter temp = new GoogleSpreedsheetWriter();
            temp.WriteDecks();

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}