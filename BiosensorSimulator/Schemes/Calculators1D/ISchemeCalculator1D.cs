namespace BiosensorSimulator.Schemes.Calculators1D
{
    public interface ISchemeCalculator1D
    {
        void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);
    }
}
