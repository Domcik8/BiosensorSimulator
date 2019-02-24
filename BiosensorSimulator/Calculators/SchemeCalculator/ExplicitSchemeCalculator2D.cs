﻿using BiosensorSimulator.Calculators.SchemeParameters;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Calculators.SchemeCalculator
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
                {
                    continue;
                }

                var index = Biosensor.Layers.IndexOf(layer);

                switch (layer.Type)
                {
                    case LayerType.Enzyme:
                        CalculateReactionDiffusionLayerNextStep(layer, Biosensor.Layers[index - 1], sCur, pCur, sPrev, pPrev);
                        break;
                    case LayerType.DiffusionLayer:
                        CalculateDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                        break;
                    case LayerType.DiffusionSmallLayer:
                        CalculateFirstDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
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
                                 (CalculateDiffusionLayerNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j) +
                                  CalculateDiffusionLayerNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.Substrate.ExplicitScheme.H2));

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j) +
                                  CalculateDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.Product.ExplicitScheme.H2));
                }
            }
        }

        public void CalculateFirstDiffusionLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex + 1; i++)
            {
                for (var j = 1; j < sCur.GetLength(1) - 1; j++)
                {
                    sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j) +
                                  CalculateDiffusionLayerNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.Substrate.ExplicitScheme.H2));

                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j) +
                                  CalculateDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.Product.ExplicitScheme.H2));
                }
            }
        }

        private double CalculateDiffusionLayerNextLocation(double previous, double current, double next, double step, int j)
        {
            return (j + 0.5) * step * ((next - current) / step) / (step * (step * j)) - (j - 0.5) * step * ((current - previous) / step) / (step * (step * j));
        }

        //public void CalculateDiffusionLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        //{
        //    for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
        //    {
        //        for (var j = 1; j < sCur.GetLength(1) - 1; j++)
        //        {
        //            sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
        //                         (CalculateDiffusionLayerNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j],
        //                              layer.Substrate.ExplicitScheme.H2) +
        //                          CalculateDiffusionLayerNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1],
        //                              layer.Substrate.ExplicitScheme.W2));

        //            pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
        //                         (CalculateDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j],
        //                              layer.Product.ExplicitScheme.H2) +
        //                          CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1],
        //                              layer.Product.ExplicitScheme.W2));
        //        }
        //    }
        //}

        //public void CalculateFirstDiffusionLayerNextStep(Layer layer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        //{
        //    for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex + 1; i++)
        //    {
        //        for (var j = 1; j < sCur.GetLength(1) - 1; j++)
        //        {
        //            sCur[i, j] = sPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
        //                         (CalculateDiffusionLayerNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j],
        //                             layer.Substrate.ExplicitScheme.H2) +
        //                          CalculateDiffusionLayerNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1],
        //                              layer.Substrate.ExplicitScheme.W2));

        //            pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
        //                         (CalculateDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j],
        //                              layer.Product.ExplicitScheme.H2) +
        //                          CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1],
        //                              layer.Product.ExplicitScheme.W2));
        //        }
        //    }
        //}

        private double CalculateDiffusionLayerNextLocation(double previous, double current, double next, double step)
        {
            return (next - 2 * current + previous) / step;
        }

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, Layer prevLayer, double[,] sCur, double[,] pCur, double[,] sPrev, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < sCur.GetLength(1) - 1; j++)
                {
                    var fermentReactionSpeed = SimulationParameters.t * (Biosensor.VMax * sPrev[i, j] / (Biosensor.Km + sPrev[i, j]));

                    sCur[i, j] = sPrev[i, j] + SimulationParameters.t * layer.Substrate.DiffusionCoefficient *
                                 (CalculateDiffusionLayerNextLocation(sPrev[i, j - 1], sPrev[i, j], sPrev[i, j + 1], layer.W, j) +
                                  CalculateReactionDiffusionLayerNextLocation(sPrev[i - 1, j], sPrev[i, j], sPrev[i + 1, j], layer.Substrate.ExplicitScheme.H2)) - fermentReactionSpeed;

                    pCur[i, j] = pPrev[i, j] + SimulationParameters.t * layer.Product.DiffusionCoefficient *
                                 (CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j) +
                                  CalculateReactionDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.Product.ExplicitScheme.H2)) + fermentReactionSpeed;
                }
            }
        }

        private double CalculateReactionDiffusionLayerNextLocation(double previous, double current, double next, double step)
        {
            return (next - 2 * current + previous) / step;
        }

        //public void CalculateDiffusionLayerWithOnlyProductNextStep(Layer layer, double[,] pCur, double[,] pPrev)
        //{
        //    for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
        //    {
        //        for (var j = 1; j < pCur.GetLength(1) - 1; j++)
        //        {
        //            pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
        //                         (CalculateDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j],
        //                              layer.Product.ExplicitScheme.H2) +
        //                          CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1],
        //                              layer.Product.ExplicitScheme.W2));
        //        }
        //    }
        //}

        public void CalculateDiffusionLayerWithOnlyProductNextStep(Layer layer, double[,] pCur, double[,] pPrev)
        {
            for (var i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                for (var j = 1; j < pCur.GetLength(1) - 1; j++)
                {
                    pCur[i, j] = pPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                 (CalculateDiffusionLayerNextLocation(pPrev[i, j - 1], pPrev[i, j], pPrev[i, j + 1], layer.W, j) +
                                 CalculateDiffusionLayerNextLocation(pPrev[i - 1, j], pPrev[i, j], pPrev[i + 1, j], layer.Product.ExplicitScheme.H2));
                }
            }
        }
    }
}