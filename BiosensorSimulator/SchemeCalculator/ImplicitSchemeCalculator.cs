using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public BiosensorParameters BiosensorParameters { get; }
        public SimulationParameters SimulationParameters { get; }
        public double R { get; }
        public double DSdr { get; }
        public double DPdr { get; }

        public ImplicitSchemeCalculator(
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            BiosensorParameters = biosensorParameters;
            SimulationParameters = simulationParameters;

            R = simulationParameters.t / (simulationParameters.hd * simulationParameters.hd);
            DSdr = biosensorParameters.DSd * R;
            DPdr = biosensorParameters.DPd * R;
        }

        public void CalculateDiffusionLayerNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            for (int i = 1; i < SimulationParameters.Nd; i++)
            {
                sCur[i] = sPrev[i] + DSdr * (sPrev[i + 1] - 2 * sPrev[i] + sPrev[i - 1]);
                pCur[i] = pPrev[i] + DPdr * (pPrev[i + 1] - 2 * pPrev[i] + pPrev[i - 1]);
            }
        }

        public void CalculateReactionDiffusionLayerNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            double dSfOverhh = BiosensorParameters.DSf / (SimulationParameters.hf * SimulationParameters.hf);
            double dPfOverhh = BiosensorParameters.DPf / (SimulationParameters.hf * SimulationParameters.hf);

            for (int i = 1; i < SimulationParameters.Nf; i++)
            {
                double fermentReactionSpeed = BiosensorParameters.Vmax * sPrev[i] /
                    (BiosensorParameters.Km + sPrev[i]);

                sCur[i] = sPrev[i] + SimulationParameters.t * (dSfOverhh *
                    (sPrev[i + 1] - 2 * sPrev[i] + sPrev[i - 1])
                    - fermentReactionSpeed);

                pCur[i] = pPrev[i] + SimulationParameters.t * (dPfOverhh *
                    (pPrev[i + 1] - 2 * pPrev[i] + pPrev[i - 1])
                    + fermentReactionSpeed);
            }
        }
    }
}
