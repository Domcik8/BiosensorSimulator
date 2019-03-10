using System.Linq;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;

namespace BiosensorSimulator.Simulations.Simulations2D
{
    public class PerforatedMembrane2D : BaseSimulation2D
    {
        public PerforatedMembrane2D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, resultPrinter) { }

        public override void CalculateBoundaryConditions()
        {
            var firstLayer = Biosensor.Layers.First();
            SetBoundaryConditionsWithSelectiveMembrane(firstLayer);
        }

        private void SetBoundaryConditionsWithSelectiveMembrane(Layer selectiveMembraneLayer)
        {
            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                SCur[SimulationParameters.N - 1, j] = Biosensor.S0;
                SCur[selectiveMembraneLayer.UpperBondIndex, j] = SCur[selectiveMembraneLayer.UpperBondIndex + 1, j];
                PCur[0, j] = 0;
                PCur[SimulationParameters.N - 1, j] = 0;
            }
        }

        public override void CalculateNonLeakageConditions()
        {
            var enzyme = Biosensor.EnzymeLayer;
            var diffusion = Biosensor.DiffusionLayer;
            var diffusionSmall = Biosensor.DiffusionSmallLayer;

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

            for (long i = enzyme.UpperBondIndex + 1; i < diffusion.LowerBondIndex; i++)
            {
                SCur[i, diffusionSmall.M] = SCur[i, diffusionSmall.M - 1];
                PCur[i, diffusionSmall.M] = PCur[i, diffusionSmall.M - 1];
            }

            for (long j = diffusionSmall.M + 1; j < SimulationParameters.M; j++)
            {
                SCur[enzyme.UpperBondIndex, j] = SCur[enzyme.UpperBondIndex - 1, j];
                PCur[enzyme.UpperBondIndex, j] = PCur[enzyme.UpperBondIndex - 1, j];

                SCur[diffusion.LowerBondIndex, j] = SCur[diffusion.LowerBondIndex + 1, j];
                PCur[diffusion.LowerBondIndex, j] = PCur[diffusion.LowerBondIndex + 1, j];
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

                var previousLayer = Biosensor.Layers[index - 1];

                if (previousLayer.Type == LayerType.DiffusionSmallLayer && layer.Type == LayerType.DiffusionLayer)
                {
                    continue;
                }

                if (previousLayer.Type == LayerType.SelectiveMembrane)
                {
                    SetMatchingConditionsWithSelectiveMembrane(layer, previousLayer);
                }
                else if (previousLayer.Type == LayerType.Enzyme)
                {
                    SetMatchingConditionsEnzyme(layer, previousLayer);
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

        private void SetMatchingConditionsEnzyme(Layer layer, Layer previousLayer)
        {
            for (int j = 0; j <= layer.M; j++)
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