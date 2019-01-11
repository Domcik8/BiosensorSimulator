using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public void CalculateNextStep(
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            Array.Copy(sCur, sPrev, sCur.Length);
            Array.Copy(pCur, pPrev, pCur.Length);

            /*CalculateNextStepDiffusionLayer(sCur, pCur, sPrev, pPrev,
                biosensorParameters, simulationParameters);*/
                
            // Main ferment layer equations
            CalculateNextStepReactionDiffusionLayer(sCur, pCur, sPrev, pPrev,
                biosensorParameters, simulationParameters);

            // Lets say, that we got only ferment layer
            sCur[simulationParameters.N] = biosensorParameters.S0;
            sCur[0] = sCur[1];
            pCur[simulationParameters.N] = biosensorParameters.P0;
            pCur[0] = 0;
        }

        private static void CalculateNextStepDiffusionLayer(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev, BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            double r = simulationParameters.t / (simulationParameters.hd * simulationParameters.hd);
            double dSdr = biosensorParameters.DSd * r;
            double dPdr = biosensorParameters.DPd * r;

            for (int i = 1; i < simulationParameters.Nd; i++)
            {
                sCur[i] = sPrev[i] + dSdr * (sPrev[i + 1] - 2 * sPrev[i] + sPrev[i - 1]);
                pCur[i] = pPrev[i] + dPdr * (pPrev[i + 1] - 2 * pPrev[i] + pPrev[i - 1]);
            }
        }

        private static void CalculateNextStepReactionDiffusionLayer(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev, BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            double dSfOverhh = biosensorParameters.DSf / (simulationParameters.hf * simulationParameters.hf);
            double dPfOverhh = biosensorParameters.DPf / (simulationParameters.hf * simulationParameters.hf);

            for (int i = 1; i < simulationParameters.Nf; i++)
            {
                double fermentReactionSpeed = biosensorParameters.Vmax * sPrev[i] / 
                    (biosensorParameters.Km + sPrev[i]);
                
                sCur[i] = sPrev[i] + simulationParameters.t * (dSfOverhh *
                    (sPrev[i + 1] - 2 * sPrev[i] + sPrev[i - 1])
                    - fermentReactionSpeed);

                pCur[i] = pPrev[i] + simulationParameters.t * (dPfOverhh *
                    (pPrev[i + 1] - 2 * pPrev[i] + pPrev[i - 1])
                    + fermentReactionSpeed);
            }
        }
    }
}
