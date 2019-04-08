using System;
using System.Globalization;
using System.IO;

namespace BiosensorSimulator.Results
{
    public class ConsoleFilePrinter : IResultPrinter
    {
        private const string FileName = "Simulation_result_";
        private readonly string _path;
        private StreamWriter _streamWriter;

        public ConsoleFilePrinter(string path)
        {
            _path = Path.Combine(path, FileName + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".txt");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            OpenStream();

            CultureInfo.CurrentCulture = new CultureInfo("lt-LT", false);
        }

        public void Print(string message)
        {
            PrintToStream(message);
        }

        public void OpenStream()
        {
            _streamWriter = new StreamWriter(_path);
        }

        public void PrintToStream(string message)
        {
            Console.WriteLine(message);
            _streamWriter.WriteLine(message);
        }

        public void CloseStream()
        {
            _streamWriter.Close();
        }
    }
}