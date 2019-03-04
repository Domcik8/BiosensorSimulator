using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;

namespace BiosensorSimulator.Simulations.Simulations2D
{
    public class SimpleSimulation2D : BaseSimulation2D
    {
        public SimpleSimulation2D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, resultPrinter) { }

        public override void CalculateBoundaryConditions()
        {
            SetBoundaryConditions();
        }

        private void SetBoundaryConditions()
        {
            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                SCur[SimulationParameters.N - 1, j] = Biosensor.S0;
                SCur[0, j] = SCur[1, j];
                PCur[0, j] = 0;
                PCur[SimulationParameters.N - 1, j] = 0;
            }
        }

        public override void CalculateNonLeakageConditions()
        {
            for (int i = 0; i < PCur.GetLength(0); i++)
            {
                SCur[i, 0] = SCur[i, 1];
                SCur[i, SimulationParameters.M - 1] = SCur[i, SimulationParameters.M - 2];
                PCur[i, 0] = PCur[i, 1];
                PCur[i, SimulationParameters.M - 1] = PCur[i, SimulationParameters.M - 2];
            }
        }

        public override void CalculateMatchingConditions()
        {
            foreach (var layer in Biosensor.Layers)
            {
                var index = Biosensor.Layers.IndexOf(layer);

                if (index == 0)
                {
                    continue;
                }

                SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
            }
        }

        private void SetMatchingConditions(Layer layer, Layer previousLayer)
        {
            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                SCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Substrate.DiffusionCoefficient *
                     SCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Substrate.DiffusionCoefficient + previousLayer.H * layer.Substrate.DiffusionCoefficient);

                PCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Product.DiffusionCoefficient *
                     PCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
            }
        }
    }
}