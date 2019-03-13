using System.Linq;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;

namespace BiosensorSimulator.Simulations.Simulations2D
{
    public class MicroreactorSimulation2D : BaseSimulation2D
    {
        public MicroreactorSimulation2D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, resultPrinter) { }

        public override double GetCurrent()
        {
            if (Biosensor is BaseMicroreactorBiosensor)
                return GetCurrentForMicroreactorBiosensor();
            else
                return GetCurrentForBiosensor();
        }

        public double GetCurrentForMicroreactorBiosensor()
        {
            double enzimeAreaCurrent = 0;
            double diffusionAreaCurrent = 0;
            var firstLayer = (LayerWithSubAreas)Biosensor.Layers.First();
            var firstArea = firstLayer.SubAreas.First();
            var secondArea = firstLayer.SubAreas.Last();
            var spaceStepR = firstLayer.W / firstLayer.H * firstLayer.W;

            for (var j = firstArea.LeftBondIndex; j < firstArea.RightBondIndex; j++)
                enzimeAreaCurrent += PCur[1, j] * spaceStepR * (j + 1);

            for (var j = secondArea.LeftBondIndex; j < secondArea.RightBondIndex; j++)
                diffusionAreaCurrent += PCur[1, j] * spaceStepR * (j + 1);

            return CurrentFactor * (enzimeAreaCurrent * firstArea.Product.DiffusionCoefficient
                                    + diffusionAreaCurrent * secondArea.Product.DiffusionCoefficient);
        }

        public double GetCurrentForBiosensor()
        {
            double sum = 0;
            var firstLayer = Biosensor.Layers.First();
            var spaceStepR = firstLayer.W / firstLayer.H * firstLayer.W;

            for (var j = 0; j < SCur.GetLength(1) - 1; j++)
                sum += PCur[1, j] * spaceStepR * (j + 1);

            return sum * CurrentFactor * firstLayer.Product.DiffusionCoefficient;
        }

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
                if (layer.Type == LayerType.NonHomogenousLayer)
                    SetMatchingConditionsVertically((LayerWithSubAreas)layer);

                var index = Biosensor.Layers.IndexOf(layer);
                if (index == 0)
                    continue;

                if (layer.Type == LayerType.NonHomogenousLayer)
                {
                    var layerWithSubAreas = (LayerWithSubAreas)layer;

                    SetMatchingConditions(layerWithSubAreas.SubAreas.First(),
                        Biosensor.Layers[index - 1]);
                    
                    SetMatchingConditions(layerWithSubAreas.SubAreas.Last(),
                            Biosensor.Layers[index - 1]);
                }
                if (Biosensor.Layers[index - 1].Type == LayerType.NonHomogenousLayer)
                {
                    SetMatchingConditionsWithNonHomogenousLayer(layer,
                        ((LayerWithSubAreas)Biosensor.Layers[index - 1]).SubAreas.First());
                    SetMatchingConditionsWithNonHomogenousLayer(layer,
                        ((LayerWithSubAreas)Biosensor.Layers[index - 1]).SubAreas.Last());
                }
                else
                    SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
            }
        }

        private void SetMatchingConditions(Area layer, Area previousLayer)
        {
            for (var j = layer.LeftBondIndex; j < layer.RightBondIndex; j++)
            {
                SCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Substrate.DiffusionCoefficient *
                     SCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Substrate.DiffusionCoefficient + previousLayer.H * layer.Substrate.DiffusionCoefficient);

                PCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Product.DiffusionCoefficient *
                     PCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
            }
        }

        private void SetMatchingConditionsWithNonHomogenousLayer(Area layer, Area previousLayer)
        {
            for (var j = previousLayer.LeftBondIndex; j < previousLayer.RightBondIndex; j++)
            {
                SCur[layer.LowerBondIndex, j] =
                (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1, j]
                + layer.H * previousLayer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex - 1, j])
                /
                (layer.H * previousLayer.Substrate.DiffusionCoefficient
                + previousLayer.H * layer.Substrate.DiffusionCoefficient);

                PCur[layer.LowerBondIndex, j] =
                (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1, j]
                + layer.H * previousLayer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex - 1, j])
                /
                (layer.H * previousLayer.Product.DiffusionCoefficient
                + previousLayer.H * layer.Product.DiffusionCoefficient);
            }
        }

        private void SetMatchingConditionsVertically(LayerWithSubAreas layer)
        {
            var firstArea = layer.SubAreas.First();
            var secondArea = layer.SubAreas.Last();

            if (secondArea.Width == 0)
                return;

            for (var i = layer.LowerBondIndex; i < layer.UpperBondIndex; i++)
            {
                SCur[i, firstArea.RightBondIndex] =
                    (firstArea.W * secondArea.Substrate.DiffusionCoefficient * SCur[i, firstArea.RightBondIndex - 1]
                    + secondArea.W * firstArea.Substrate.DiffusionCoefficient * SCur[i, secondArea.LeftBondIndex + 1])
                    /
                    (secondArea.W * firstArea.Substrate.DiffusionCoefficient
                    + firstArea.H * secondArea.H * firstArea.Substrate.DiffusionCoefficient);

                PCur[i, firstArea.RightBondIndex] =
                    (firstArea.W * secondArea.Product.DiffusionCoefficient * PCur[i, firstArea.RightBondIndex - 1]
                    + secondArea.W * firstArea.Product.DiffusionCoefficient * PCur[i, secondArea.LeftBondIndex + 1])
                    /
                    (secondArea.W * firstArea.Product.DiffusionCoefficient
                    + firstArea.H * secondArea.H * firstArea.Product.DiffusionCoefficient);
            }
        }
    }
}