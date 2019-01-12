using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public interface ISchemeCalculator
    {
        void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);

        void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);

        void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev);
    }
}
