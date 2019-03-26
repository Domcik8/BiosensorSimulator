﻿using BiosensorSimulator.Calculators.SchemeParameters;
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
                layer.Product.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Product);

                if (layer.Type == LayerType.SelectiveMembrane)
                    continue;

                layer.Substrate.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Substrate);
            }
        }

        public void CalculateNextStep(double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            foreach (var layer in Biosensor.Layers)
            {
                if (layer.H == 0)
                    continue;

                var index = Biosensor.Layers.IndexOf(layer);

                switch (layer.Type)
                {
                    case LayerType.Enzyme:
                        CalculateSmallEnzymeLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                        break;
                    case LayerType.DiffusionLayer:
                        CalculateSmallDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                        break;
                    case LayerType.EnzymeSmallLayer:
                        CalculateSmallEnzymeLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                        break;
                    case LayerType.DiffusionSmallLayer:
                        CalculateSmallDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                        break;
                    case LayerType.SelectiveMembrane:
                        CalculateDiffusionLayerWithOnlyProductNextStep(layer, pCur, pPrev);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void CalculateDiffusionLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < sCur.GetLength(1) - 1; j++)
                {
                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j));

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j));
                }
            }
        }
        
        public void CalculateSmallDiffusionLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i <= layer.M2; i++)
            {
                for (var j = 1; j < layer.M; j++)
                {
                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j));

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j));
                }
            }

            for (var i = layer.M2 + 1; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < sCur.GetLength(1) - 1; j++)
                {
                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.H)
                                  + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j));

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                  + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j));
                }
            }
        }

        public void CalculateSmallEnzymeLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.M2; i++)
            {
                for (var j = 1; j < sCur.GetLength(1) - 1; j++)
                {
                    var fermentReactionSpeed = SimulationParameters.t * (Biosensor.VMax * sPrev[i, j] / (Biosensor.Km + sPrev[i, j]));

                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.H)
                                  + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j))
                                 - fermentReactionSpeed;

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                             (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                              + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j))
                                             + fermentReactionSpeed;
                }
            }

            for (var i = layer.M2; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < layer.M; j++)
                {
                    var fermentReactionSpeed = SimulationParameters.t * (Biosensor.VMax * sPrev[i, j] / (Biosensor.Km + sPrev[i, j]));

                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.H)
                                  + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j))
                                 - fermentReactionSpeed;

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                             (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                              + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j))
                                             + fermentReactionSpeed;
                }
            }
        }

        private double CalculateDiffusionLayerCoordinateRNextLocation(
            double previous, double current, double next, double step, int j)
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

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < sCur.GetLength(1) - 1; j++)
                {
                    var fermentReactionSpeed = SimulationParameters.t * (Biosensor.VMax * sPrev[i, j] / (Biosensor.Km + sPrev[i, j]));

                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j))
                        - fermentReactionSpeed;

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j))
                        + fermentReactionSpeed;
                }
            }
        }

        public void CalculateDiffusionLayerWithOnlyProductNextStep(Layer layer, double[,] pCur, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < pCur.GetLength(1) - 1; j++)
                {
                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerCoordinateZNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.H)
                                 + CalculateDiffusionLayerCoordinateRNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j));
                }
            }
        }
    }
}