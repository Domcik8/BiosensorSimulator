using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ExplicitSchemeCalculator : ISchemeCalculator
    {
        public BiosensorParameters BiosensorParameters { get; }
        public SimulationParameters SimulationParameters { get; }
        public double R { get; }
        public double DSdr { get; }
        public double DPdr { get; }

        public ExplicitSchemeCalculator(
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            BiosensorParameters = biosensorParameters;
            SimulationParameters = simulationParameters;
        }

        public void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            if (layer.Type == LayerType.Enzyme)
            {
                CalculateReactionDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
            }

            if (layer.Type == LayerType.DiffusionLayer)
            {
                CalculateDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
            }
        }

        public void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            double R = SimulationParameters.t / (layer.H * layer.H);
            double DSdr = layer.Substances.First(s => s.Type == SubstanceType.Substrate).DiffusionCoefficient * R;
            double DPdr = layer.Substances.First(s => s.Type == SubstanceType.Product).DiffusionCoefficient * R;

            for (int i = 1; i < SimulationParameters.Nd; i++)
            {
                sCur[i] = sPrev[i] + DSdr * (sPrev[i + 1] - 2 * sPrev[i] + sPrev[i - 1]);
                pCur[i] = pPrev[i] + DPdr * (pPrev[i + 1] - 2 * pPrev[i] + pPrev[i - 1]);
            }
        }

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            double dSfOverhh = layer.Substances.First(s => s.Type == SubstanceType.Substrate).DiffusionCoefficient / (layer.H * layer.H);
            double dPfOverhh = layer.Substances.First(s => s.Type == SubstanceType.Product).DiffusionCoefficient / (layer.H * layer.H);

            for (int i = 1; i < SimulationParameters.Nf; i++)
            {
                double fermentReactionSpeed = BiosensorParameters.VMax * sPrev[i] /
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
