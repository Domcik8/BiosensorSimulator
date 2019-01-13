using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public void CalculateNextStep(
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
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
