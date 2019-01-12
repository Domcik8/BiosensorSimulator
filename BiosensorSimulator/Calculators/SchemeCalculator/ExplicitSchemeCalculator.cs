using System;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ExplicitSchemeCalculator : ISchemeCalculator
    {
        public BiosensorParameters BiosensorParameters { get; }
        public SimulationParameters SimulationParameters { get; }

        public ExplicitSchemeCalculator(
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            BiosensorParameters = biosensorParameters;
            SimulationParameters = simulationParameters;
        }

        public void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            switch (layer.Type)
            {
                case LayerType.Enzyme:
                    CalculateReactionDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                    break;

                case LayerType.DiffusionLayer:
                    CalculateDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                    break;

                case LayerType.SelectiveMembrane:
                    throw new NotImplementedException();
                    break;

                case LayerType.PerforatedMembrane:
                    throw new NotImplementedException();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            for (var i = 1; i < SimulationParameters.Nd; i++)
            {
                sCur[i] =  CalculateDiffusionLayerNextLocation(sPrev[i - 1], sPrev[i], sPrev[i + 1],
                    layer.Substances.First(x => x.Type == SubstanceType.Substrate).DiffusionCoefficientOverR);

                pCur[i] = CalculateDiffusionLayerNextLocation(pPrev[i - 1], pPrev[i], pPrev[i + 1],
                    layer.Substances.First(x => x.Type == SubstanceType.Product).DiffusionCoefficientOverR);
            }
        }

        private double CalculateDiffusionLayerNextLocation(
            double previous, double current, double next, double diffusionCoefficientOverR)
        {
            return current + diffusionCoefficientOverR * (next - 2 * current + previous);
        }

        public void CalculateReactionDiffusionLayerNextStep(
            Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            for (var i = 1; i < SimulationParameters.Nf; i++)
            {
                var fermentReactionSpeed = BiosensorParameters.VMax * sPrev[i] /
                    (BiosensorParameters.Km + sPrev[i]);

                sCur[i] = CalculateReactionDiffusionLayerNextLocation(sPrev[i - 1], sPrev[i], sPrev[i + 1], -fermentReactionSpeed,
                    layer.Substances.First(x => x.Type == SubstanceType.Substrate).DiffusionCoefficientOverSpace);

                pCur[i] = CalculateReactionDiffusionLayerNextLocation(pPrev[i - 1], pPrev[i], pPrev[i + 1], fermentReactionSpeed, layer.Substances.First(x => x.Type == SubstanceType.Product).DiffusionCoefficientOverSpace);
            }
        }
        
        private double CalculateReactionDiffusionLayerNextLocation(double previous, double current, double next,
            double fermentReactionSpeed, double diffusionCoefficientOverSpace)
        {
            return current + SimulationParameters.t * (diffusionCoefficientOverSpace *
                (next - 2 * current + previous) + fermentReactionSpeed);
        }
    }
}
