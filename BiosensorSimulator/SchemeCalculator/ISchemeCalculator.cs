namespace BiosensorSimulator.SchemeCalculator
{
    public interface ISchemeCalculator
    {
        void CalculateDiffusionLayerNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);

        void CalculateReactionDiffusionLayerNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);
    }
}
