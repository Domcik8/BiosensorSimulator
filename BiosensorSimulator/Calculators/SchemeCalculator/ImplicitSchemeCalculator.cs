using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public void CalculateNextStep(
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            Biosensor biosensor, SimulationParameters simulationParameters)
        {
            /*Copy(SPrev, SCur);
            Copy(PPrev, PCur);

            double DsOverhh = Biosensor.Ds / (Biosensor.hd * Biosensor.hd);
            double DpOverhh = Biosensor.Dp / (Biosensor.hd * Biosensor.hd);

            for (int i = 1; i < Biosensor.N; i++) // 29
            {
                double commonMember = Biosensor.Vmax * SPrev[i] / (Biosensor.Km + SPrev[i]);
                SCur[i] = Biosensor.t * (DsOverhh * (SPrev[i + 1] - 2 * SPrev[i] + SPrev[i - 1]) - commonMember) + SPrev[i];
                PCur[i] = Biosensor.t * (DpOverhh * (PPrev[i + 1] - 2 * PPrev[i] + PPrev[i - 1]) + commonMember) + PPrev[i];
            }
            


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
