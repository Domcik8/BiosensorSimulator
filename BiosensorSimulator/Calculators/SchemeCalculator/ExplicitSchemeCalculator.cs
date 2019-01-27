using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ExplicitSchemeCalculator : ISchemeCalculator
    {
        public BaseBiosensor Biosensor { get; }
        public SimulationParameters SimulationParameters { get; }

        public ExplicitSchemeCalculator(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            Biosensor = biosensor;
            SimulationParameters = simulationParameters;
        }

        public void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            foreach (var layer in Biosensor.Layers)
                CalculateNextStep(layer, sCur, pCur, sPrev, pPrev);
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
                    CalculateDiffusionLayerWithOnlyProductNextStep(layer, pCur, pPrev);
                    break;

                case LayerType.PerforatedMembrane:
                    CalculateDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
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
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                var fermentReactionSpeed = Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);
                var effectiveFermentReactionSpeed = fermentReactionSpeed * Biosensor.EffectiveReactionCoefficent;

                sCur[i] = CalculateReactionDiffusionLayerNextLocation(sPrev[i - 1], sPrev[i], sPrev[i + 1],
                    -effectiveFermentReactionSpeed, layer.Substrate.ExplicitScheme.DiffusionCoefficientOverSpace);

                pCur[i] = CalculateReactionDiffusionLayerNextLocation(pPrev[i - 1], pPrev[i], pPrev[i + 1],
                    effectiveFermentReactionSpeed, layer.Product.ExplicitScheme.DiffusionCoefficientOverSpace);
            }
        }

        private double CalculateReactionDiffusionLayerNextLocation(double previous, double current, double next,
            double fermentReactionSpeed, double diffusionCoefficientOverSpace)
        {
            return current + SimulationParameters.t * (diffusionCoefficientOverSpace * (next - 2 * current + previous) + fermentReactionSpeed);
        }

        public void CalculateDiffusionLayerWithOnlyProductNextStep(Layer layer, double[] pCur, double[] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                pCur[i] = CalculateDiffusionLayerNextLocation(pPrev[i - 1], pPrev[i], pPrev[i + 1], layer.Product.ExplicitScheme.DiffusionCoefficientOverR);
            }
        }
    }
}