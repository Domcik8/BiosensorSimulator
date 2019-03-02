using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using System.Linq;

namespace BiosensorSimulator.Simulations.Simulations1D
{
    public class LayerSimulation2D : BaseSimulation2D
    {
        private int temp; 
        public LayerSimulation2D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, resultPrinter)
        {
            var doub = (1e-3 - 0.1e-3) * 20 / 1e-3;
            temp = (int) doub;
        }

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
            //SCur[SimulationParameters.N - 1, SimulationParameters.M - 1] = Biosensor.S0;
            //SCur[0] = SCur[1];

            //PCur[SimulationParameters.N - 1] = Biosensor.P0;
            //PCur[0] = 0;
        }

        private void SetBoundaryConditionsWithSelectiveMembrane(Layer selectiveMembraneLayer)
        {
            var enzyme = Biosensor.EnzymeLayer;
            var diffusion = Biosensor.DiffusionLayer;
            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                SCur[SimulationParameters.N - 1, j] = Biosensor.S0;
                SCur[selectiveMembraneLayer.UpperBondIndex, j] = SCur[selectiveMembraneLayer.UpperBondIndex + 1, j];
                PCur[0, j] = 0;
                PCur[SimulationParameters.N - 1, j] = 0;
            }

            //for (int j = 0; j < SCur.GetLength(1); j++)
            //{
            //    SCur[selectiveMembraneLayer.UpperBondIndex, j] = SCur[selectiveMembraneLayer.UpperBondIndex + 1, j];
            //}

            //for (int j = 0; j < PCur.GetLength(1); j++)
            //{
            //    PCur[0, j] = 0;
            //}

            //for (int j = 0; j < PCur.GetLength(1); j++)
            //{
            //    PCur[SimulationParameters.N - 1, j] = 0;
            //}

            for (int i = 0; i < PCur.GetLength(0); i++)
            {
                SCur[i, 0] = SCur[i, 1];
                SCur[i, SimulationParameters.M - 1] = SCur[i, SimulationParameters.M - 2];
                PCur[i, 0] = PCur[i, 1];
                PCur[i, SimulationParameters.M - 1] = PCur[i, SimulationParameters.M - 2];
            }

            //for (int j = 20 - temp; j < PCur.GetLength(1); j++)
            //{
            //    SCur[enzyme.UpperBondIndex, j] = SCur[enzyme.UpperBondIndex - 1, j];
            //    PCur[enzyme.UpperBondIndex, j] = PCur[enzyme.UpperBondIndex - 1, j];

            //    SCur[diffusion.LowerBondIndex, j] = SCur[diffusion.LowerBondIndex + 1, j];
            //    PCur[diffusion.LowerBondIndex, j] = PCur[diffusion.LowerBondIndex + 1, j];
            //}
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

                var previousLayer = Biosensor.Layers[index - 1];

                if (previousLayer.Type == LayerType.DiffusionSmallLayer && layer.Type == LayerType.DiffusionLayer)
                {
                    continue;
                }

                if (previousLayer.Type == LayerType.SelectiveMembrane)
                {
                    SetMatchingConditionsWithSelectiveMembrane(layer, previousLayer);
                }
                else
                {
                    SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
                }
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

        private void SetMatchingConditionsWithSelectiveMembrane(Layer layer, Layer previousLayer)
        {
            for (int j = 0; j < PCur.GetLength(1); j++)
            {
                PCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Product.DiffusionCoefficient *
                     PCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
            }
        }
    }
}