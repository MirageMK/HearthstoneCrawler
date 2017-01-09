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
            BaseReader br = new MetabombReader();
            br.GetDecks();
            BaseWriter bw = new GoogleSpreedsheetWriter();

            bw.WriteDecks();

            Console.WriteLine(MyCollection.Cards.Count);
            Console.ReadLine();
        }
    }
}
