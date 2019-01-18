using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;
using BiosensorSimulator.Results;

namespace BiosensorSimulator.Simulations.Simulations1D
{
    public class SingleLayerSimulation1D : BaseSimulation
    {
        public SingleLayerSimulation1D(
            SimulationParameters simulationParameters,
            Biosensor biosensor,
            ISchemeCalculator schemeCalculator,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, schemeCalculator, resultPrinter) { }

        public override void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);
            
            SchemeCalculator.CalculateNextStep(SCur, PCur, SPrev, PPrev);

            foreach (var layer in Biosensor.Layers)
            {
                var index = Biosensor.Layers.IndexOf(layer);

                if (index != 0)
                {
                    SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
                }
            }

            SetBoundaryConditions();
        }

        private void SetBoundaryConditions()
        {
            SCur[SimulationParameters.N] = Biosensor.S0;
            SCur[0] = SCur[1];

            PCur[SimulationParameters.N] = Biosensor.P0;
            PCur[0] = 0;
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
    }
}