using System;
using System.Collections.Generic;
using HSCore;
using HSCore.Model;
using HSCore.Readers;
using HSCore.Writers;

namespace HSConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var temp = new HSReplayReader();
            var x = temp.GetDecks();

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}