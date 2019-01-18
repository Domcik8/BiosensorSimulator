namespace BiosensorSimulator.Results
{
    public interface IResultPrinter
    {
        void Print(string message);

        void OpenStream();

        void PrintToStream(string message);

        void CloseStream();
    }
}