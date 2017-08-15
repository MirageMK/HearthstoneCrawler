using System;
using HSCore.Readers;

namespace HSConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var temp = new DisgusedToastReader();
            var x = temp.GetDecks();

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}