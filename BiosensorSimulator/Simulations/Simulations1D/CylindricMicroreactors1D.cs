﻿using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using System.Linq;

namespace BiosensorSimulator.Simulations.Simulations1D
{
    public class CylindricMicroreactors1D : BaseSimulation
    {
        public CylindricMicroreactors1D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, resultPrinter) { }

        public override void CalculateBoundaryConditions()
        {
            var firstLayer = Biosensor.Layers.First();

            if (firstLayer.Type == LayerType.SelectiveMembrane)
                SetBoundaryConditionsWithSelectiveMembrane(firstLayer);
            else
                SetBoundaryConditions();
        }

        private void SetBoundaryConditions()
        {
            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            SCur[0] = SCur[1];

            PCur[SimulationParameters.N - 1] = Biosensor.P0;
            PCur[0] = 0;
        }

        private void SetBoundaryConditionsWithSelectiveMembrane(Layer selectiveMembraneLayer)
        {
            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            SCur[selectiveMembraneLayer.UpperBondIndex] = SCur[selectiveMembraneLayer.UpperBondIndex + 1];

            PCur[SimulationParameters.N - 1] = Biosensor.P0;
            PCur[0] = 0;
        }

        public override void CalculateMatchingConditions()
        {
            foreach (var layer in Biosensor.Layers)
            {
                var index = Biosensor.Layers.IndexOf(layer);

                if (index == 0)
                    continue;

                var previousLayer = Biosensor.Layers[index - 1];

                if (previousLayer.Type == LayerType.SelectiveMembrane)
                    SetMatchingConditionsWithSelectiveMembrane(layer, previousLayer);
                else
                    SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
            }
        }

        private void SetMatchingConditions(Layer layer, Layer previousLayer)
        {

            SCur[layer.LowerBondIndex] =
                (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1] + layer.H * previousLayer.Substrate.DiffusionCoefficient *
                 SCur[layer.LowerBondIndex - 1]) / (layer.H * previousLayer.Substrate.DiffusionCoefficient + previousLayer.H * layer.Substrate.DiffusionCoefficient);

            PCur[layer.LowerBondIndex] =
                (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1] + layer.H * previousLayer.Product.DiffusionCoefficient *
                 PCur[layer.LowerBondIndex - 1]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
        }

        private void SetMatchingConditionsWithSelectiveMembrane(Layer layer, Layer previousLayer)
        {
            PCur[layer.LowerBondIndex] =
                (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1] + layer.H * previousLayer.Product.DiffusionCoefficient *
                 PCur[layer.LowerBondIndex - 1]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
        }
    }
}