using System;
using System.IO;

namespace BiosensorSimulator.Results
{
    public class FilePrinter : IResultPrinter
    {
        private const string FileName = "Simulation_result_";
        private readonly string _path;
        private StreamWriter _streamWriter;

        public FilePrinter(string path)
        {
            this._path = Path.Combine(path, FileName + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".txt");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            OpenStream();
        }

        public void Print(string message)
        {
            PrintToStream(message);
            //File.AppendAllText(_path, message + Environment.NewLine);
        }

        public void OpenStream()
        {
            _streamWriter = new StreamWriter(_path);
        }

        public void PrintToStream(string message)
        {
            _streamWriter.WriteLine(message);
        }

        public void CloseStream()
        {
            _streamWriter.Close();
        }
    }
}