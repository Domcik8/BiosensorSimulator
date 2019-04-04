using BiosensorSimulator.Calculators.SchemeParameters;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Schemes.Calculators2D
{
    public class ExplicitSchemeCalculator2D : ISchemeCalculator2D
    {
        public BaseBiosensor Biosensor { get; }
        public SimulationParameters SimulationParameters { get; }

        public ExplicitSchemeCalculator2D(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            Biosensor = biosensor;
            SimulationParameters = simulationParameters;

            foreach (var layer in biosensor.Layers)
            {
                if (layer.Type == LayerType.NonHomogenousLayer)
                {
                    foreach (var area in ((LayerWithSubAreas) layer).SubAreas)
                    {
                        area.Product.ExplicitScheme = new ExplicitSchemeParameters(area, area.Product);
                        area.Product.ExplicitScheme = new ExplicitSchemeParameters(area, area.Substrate);
                    }
                    continue;
                }

                layer.Product.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Product);

                if (layer.Type == LayerType.SelectiveMembrane)
                    continue;

                layer.Substrate.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Substrate);
            }
        }

        public void CalculateNextStep(double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            foreach (var layer in Biosensor.Layers)
                CalculateNextStepBasedOnLayerType(layer, sCur, pCur, sPrev, pPrev);
        }

        private void CalculateNextStepBasedOnLayerType(
            Area area, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            switch (area.Type)
            {
                case LayerType.Enzyme:
                    CalculateReactionDiffusionLayerNextStep(area, sCur, pCur, sPrev, pPrev);
                    break;
                case LayerType.DiffusionLayer:
                    CalculateDiffusionLayerNextStep(area, sCur, pCur, sPrev, pPrev);
                    break;
                case LayerType.DiffusionSmallLayer:
                    CalculateSmallDiffusionLayerNextStep(area, sCur, pCur, sPrev, pPrev);
                    break;
                case LayerType.SelectiveMembrane:
                    CalculateDiffusionLayerWithOnlyProductNextStep(area, pCur, pPrev);
                    break;
                case LayerType.NonHomogenousLayer:
                    CalculateNonHomogenousLayerNextStep((LayerWithSubAreas)area, sCur, pCur, sPrev, pPrev);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CalculateNonHomogenousLayerNextStep(
            LayerWithSubAreas layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            foreach (var area in layer.SubAreas)
                CalculateNextStepBasedOnLayerType(area, sCur, pCur, sPrev, pPrev);
        }

        public void CalculateDiffusionLayerNextStep(
            Area area, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            var substrateDiffusionCoefficientT = area.Substrate.DiffusionCoefficient * SimulationParameters.t;
            var productDiffusionCoefficientT = area.Product.DiffusionCoefficient * SimulationParameters.t;

            for (var i = area.LowerBondIndex + 1; i < area.UpperBondIndex; i++)
            {
                for (var j = area.LeftBondIndex + 1; j < area.RightBondIndex; j++)
                {
                    sCur[i, j] = sPrev[i, j] + substrateDiffusionCoefficientT *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], area.W, j));

                    pCur[i, j] = pPrev[i, j] + productDiffusionCoefficientT *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], area.W, j));
                }
            }
        }
        
        public void CalculateSmallDiffusionLayerNextStep(
            Area area, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            var substrateDiffusionCoefficientT = area.Substrate.DiffusionCoefficient * SimulationParameters.t;
            var productDiffusionCoefficientT = area.Product.DiffusionCoefficient * SimulationParameters.t;

            for (var i = area.LowerBondIndex + 1; i < area.UpperBondIndex + 1; i++)
            {
                for (var j = area.LeftBondIndex + 1; j < area.RightBondIndex; j++)
                {
                    sCur[i, j] = sPrev[i, j] + substrateDiffusionCoefficientT *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], area.W, j));

                    pCur[i, j] = pPrev[i, j] + productDiffusionCoefficientT *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], area.W, j));
                }
            }
        }

        private double CalculateDiffusionLayerCoordinateRNextLocation(
            double previous, double current, double next, double step, long j)
        {
            var firstStep = (j + 0.5) * step;
            var first = (next - current) / step;
            var secondStep = (j - 0.5) * step;
            var second = (current - previous) / step;
            var division = step * step * j;

            return (firstStep * first - secondStep * second) / division;
        }

        private double CalculateDiffusionLayerCoordinateZNextLocation(
            double previous, double current, double next, double step)
        {
            var first = (next - current) / step;
            var second = (current - previous) / step;

            return (first - second) / step;
        }

        public void CalculateReactionDiffusionLayerNextStep(
            Area area, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            var substrateDiffusionCoefficientT = area.Substrate.DiffusionCoefficient * SimulationParameters.t;
            var productDiffusionCoefficientT = area.Product.DiffusionCoefficient * SimulationParameters.t;

            for (var i = area.LowerBondIndex + 1; i < area.UpperBondIndex; i++)
            {
                for (var j = area.LeftBondIndex + 1; j < area.RightBondIndex; j++)
                {
                    var fermentReactionSpeed = SimulationParameters.t * (Biosensor.VMax * sPrev[i, j] / (Biosensor.Km + sPrev[i, j]));

                    sCur[i, j] = sPrev[i, j] + substrateDiffusionCoefficientT *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], area.W, j))
                        - fermentReactionSpeed;

                    pCur[i, j] = pPrev[i, j] + productDiffusionCoefficientT *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], area.W, j))
                        + fermentReactionSpeed;
                }
            }
        }

        public void CalculateDiffusionLayerWithOnlyProductNextStep(
            Area area, double[,] pCur, double[,] pPrev)
        {
            var productDiffusionCoefficientT = area.Product.DiffusionCoefficient * SimulationParameters.t;

            for (var i = area.LowerBondIndex + 1; i < area.UpperBondIndex; i++)
            {
                for (var j = area.LeftBondIndex + 1; j < area.RightBondIndex; j++)
                {
                    pCur[i, j] = pPrev[i, j] + productDiffusionCoefficientT  *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], area.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], area.W, j));
                }
            }
        }
    }
}