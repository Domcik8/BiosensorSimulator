using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ExplicitSchemeCalculator : ISchemeCalculator
    {
        public Biosensor Biosensor { get; }
        public SimulationParameters SimulationParameters { get; }

        public ExplicitSchemeCalculator(Biosensor biosensor, SimulationParameters simulationParameters)
        {
            Biosensor = biosensor;
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
            for (var i = 1; i < layer.N; i++)
            {
                sCur[i] = CalculateDiffusionLayerNextLocation(sPrev[i - 1], sPrev[i], sPrev[i + 1], layer.Substrate.ExplicitScheme.DiffusionCoefficientOverR);
                pCur[i] = CalculateDiffusionLayerNextLocation(pPrev[i - 1], pPrev[i], pPrev[i + 1], layer.Product.ExplicitScheme.DiffusionCoefficientOverR);
            }
        }

        private double CalculateDiffusionLayerNextLocation(double previous, double current, double next, double diffusionCoefficientOverR)
        {
            return current + diffusionCoefficientOverR * (next - 2 * current + previous);
        }

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            for (var i = 1; i < layer.N; i++)
            {
                var fermentReactionSpeed = Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);

                sCur[i] = CalculateReactionDiffusionLayerNextLocation(sPrev[i - 1], sPrev[i], sPrev[i + 1],
                    -fermentReactionSpeed, layer.Substrate.ExplicitScheme.DiffusionCoefficientOverSpace);

                pCur[i] = CalculateReactionDiffusionLayerNextLocation(pPrev[i - 1], pPrev[i], pPrev[i + 1],
                    fermentReactionSpeed, layer.Product.ExplicitScheme.DiffusionCoefficientOverSpace);
            }
        }

        private double CalculateReactionDiffusionLayerNextLocation(double previous, double current, double next,
            double fermentReactionSpeed, double diffusionCoefficientOverSpace)
        {
            return current + SimulationParameters.t * (diffusionCoefficientOverSpace * (next - 2 * current + previous) + fermentReactionSpeed);
        }
    }
}