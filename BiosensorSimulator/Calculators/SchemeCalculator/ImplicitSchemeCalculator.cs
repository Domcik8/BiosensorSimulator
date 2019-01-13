using BiosensorSimulator.Parameters.Simulations;
using System;
using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public void CalculateNextStep(
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            Biosensor biosensor, SimulationParameters simulationParameters)
        {
            throw new NotImplementedException();
        }

        public void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }

        public void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }
    }
}
