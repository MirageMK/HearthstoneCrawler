using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSCore;
using HSCore.Model;
using HSCore.Readers;
using HSCore.Writers;

namespace HSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var tempa = MyCollection.Cards;
            NetDecks.DownloadDecks();
            var temp = new GoogleSpreedsheetWriter();
            temp.WriteDecks();

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
