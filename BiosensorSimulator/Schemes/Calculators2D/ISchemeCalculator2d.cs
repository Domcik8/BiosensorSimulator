namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public interface ISchemeCalculator2D
    {
        void CalculateNextStep(double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev);
    }
}
