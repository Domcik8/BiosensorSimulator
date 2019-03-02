namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public interface ISchemeCalculator
    {
        void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);
    }
}
