namespace BiosensorSimulator.Schemes.Calculators1D
{
    public interface ISchemeCalculator
    {
        void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);
    }
}
