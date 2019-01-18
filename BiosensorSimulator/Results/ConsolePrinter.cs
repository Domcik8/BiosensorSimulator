using System;

namespace BiosensorSimulator.Results
{
    public class ConsolePrinter : IResultPrinter
    {
        public void Print(string message)
        {
            Console.WriteLine(message);
        }

        public void OpenStream() { }

        public void PrintToStream(string message)
        {
            Console.WriteLine(message);
        }

        public void CloseStream() { }
    }
}