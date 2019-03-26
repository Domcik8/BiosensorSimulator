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
            var firstLayer = Biosensor.Layers.First();
            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                SCur[SimulationParameters.N, j] = Biosensor.S0;
                SCur[firstLayer.UpperBondIndex, j] = SCur[firstLayer.UpperBondIndex + 1, j];
                PCur[0, j] = 0;
                PCur[SimulationParameters.N, j] = 0;
            }
        }

        public override void CalculateNonLeakageConditions()
        {
            var enzyme = Biosensor.EnzymeLayer;
            var diffusion = Biosensor.DiffusionLayer;
           // var small = Biosensor.SmallLayer;
          //  var enzymeSmall = Biosensor.EnzymeSmallLayer;
           // var diffusionSmall = Biosensor.DiffusionSmallLayer;

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

            //var firstLayer = Biosensor.Layers.First();
           
            //for (int j = 0; j < SCur.GetLength(1); j++)
            //{
            //    SCur[firstLayer.UpperBondIndex, j] = SCur[firstLayer.UpperBondIndex + 1, j];
            //}

            for (int i = 0; i < PCur.GetLength(0); i++)
            {
                SCur[i, 0] = SCur[i, 1];
                SCur[i, SimulationParameters.M] = SCur[i, SimulationParameters.M - 1];
                PCur[i, 0] = PCur[i, 1];
                PCur[i, SimulationParameters.M] = PCur[i, SimulationParameters.M - 1];
            }

            for (long i = enzyme.M2; i <= diffusion.M2; i++)
            {
                SCur[i, diffusion.M] = SCur[i, diffusion.M - 1];
                PCur[i, diffusion.M] = PCur[i, diffusion.M - 1];
            }
         
            //var enzymeSmall = Biosensor.EnzymeSmallLayer;
            //if (enzymeSmall != null)
            //{
            //    SetMatchingConditionsInverse(enzymeSmall);
            //}

            //var diffusionSmallL = Biosensor.DiffusionSmallLayer;
            //if (diffusionSmallL != null)
            //{
            //    SetMatchingConditionsInverse(diffusionSmallL);
            //}

            for (long j = enzyme.M + 1; j < SimulationParameters.M; j++)
            {
                SCur[enzyme.M2, j] = SCur[enzyme.M2 - 1, j];
                PCur[enzyme.M2, j] = PCur[enzyme.M2 - 1, j];

                SCur[diffusion.M2, j] = SCur[diffusion.M2 + 1, j];
                PCur[diffusion.M2, j] = PCur[diffusion.M2 + 1, j];
            }
        }

        private void SetMatchingConditionsInverse(Layer layer)
        {
            for (long i = layer.LowerBondIndex + 1; i < layer.UpperBondIndex; i++)
            {
                SCur[i, layer.M] =
                    (layer.W * layer.Substrate.DiffusionCoefficient * SCur[i, layer.M + 1] + layer.W * layer.Substrate.DiffusionCoefficient *
                     SCur[i, layer.M - 1]) / (layer.W * layer.Substrate.DiffusionCoefficient + layer.W * layer.Substrate.DiffusionCoefficient);

                PCur[i, layer.M] =
                    (layer.W * layer.Product.DiffusionCoefficient * PCur[i, layer.M + 1] + layer.W * layer.Product.DiffusionCoefficient *
                     PCur[i, layer.M - 1]) / (layer.W * layer.Product.DiffusionCoefficient + layer.W * layer.Product.DiffusionCoefficient);
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

                //if (previousLayer.Type == LayerType.DiffusionSmallLayer && layer.Type == LayerType.DiffusionLayer)
                //{
                //    //SetMatchingConditionsSmallDiffusion(layer, previousLayer);
                //    CalculateDiffusionLayersMatchinCondition(layer, previousLayer);
                //    var s = 5;
                //}
                //else if (previousLayer.Type == LayerType.Enzyme && layer.Type == LayerType.EnzymeSmallLayer)
                //{
                //    //SetMatchingConditionsEnzyme(layer, previousLayer);
                //    CalculateReactionDiffusionLayersMatchinCondition(layer, previousLayer);
                //    var s = 5;
                //}
                //else if (previousLayer.Type == LayerType.SelectiveMembrane)
                //{
                //    SetMatchingConditionsWithSelectiveMembrane(layer, previousLayer);
                //}
                //else if (previousLayer.Type == LayerType.Enzyme && layer.Type != LayerType.EnzymeSmallLayer)
                //{
                //    SetMatchingConditionsEnzyme(layer, previousLayer);
                //}
                //else if (previousLayer.Type == LayerType.EnzymeSmallLayer)
                //{
                //    SetMatchingConditionsSmallEnzyme(layer, previousLayer);
                //}
                //else
                //{
                //    SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
                //}

                if (previousLayer.Type == LayerType.SelectiveMembrane)
                {
                    SetMatchingConditionsWithSelectiveMembrane(layer, previousLayer);
                }
 
                else if (previousLayer.Type == LayerType.Enzyme)
                {
                    SetMatchingConditionsSmallEnzyme(layer, previousLayer);
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
            for (int j = 0; j < layer.M; j++)
            {
                SCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Substrate.DiffusionCoefficient *
                     SCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Substrate.DiffusionCoefficient + previousLayer.H * layer.Substrate.DiffusionCoefficient);

                PCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Product.DiffusionCoefficient *
                     PCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
            }
        }

        private void SetMatchingConditionsSmallEnzyme(Layer layer, Layer previousLayer)
        {
            for (int j = 0; j < previousLayer.M; j++)
            {
                SCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Substrate.DiffusionCoefficient *
                     SCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Substrate.DiffusionCoefficient + previousLayer.H * layer.Substrate.DiffusionCoefficient);

                PCur[layer.LowerBondIndex, j] =
                    (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1, j] + layer.H * previousLayer.Product.DiffusionCoefficient *
                     PCur[layer.LowerBondIndex - 1, j]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
            }
        }

        private void SetMatchingConditionsSmallDiffusion(Layer layer, Layer previousLayer)
        {
            for (int j = 0; j < previousLayer.M; j++)
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

        public void CalculateDiffusionLayersMatchinCondition(Layer layer, Layer previousLayer)
        {
            var i = layer.LowerBondIndex;
            for (int j = 1; j < previousLayer.M; j++)
            {
                SCur[i, j] = SPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                             (CalculateDiffusionLayerCoordinateZNextLocation(SPrev[i - 1, j], SPrev[i, j], SPrev[i + 1, j], layer.H)
                              + CalculateDiffusionLayerCoordinateRNextLocation(SPrev[i, j - 1], SPrev[i, j], SPrev[i, j + 1], layer.W, j));

                PCur[i, j] = PPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                             (CalculateDiffusionLayerCoordinateZNextLocation(PPrev[i - 1, j], PPrev[i, j], PPrev[i + 1, j], layer.H)
                              + CalculateDiffusionLayerCoordinateRNextLocation(PPrev[i, j - 1], PPrev[i, j], PPrev[i, j + 1], layer.W, j));
            }
        }

        public void CalculateReactionDiffusionLayersMatchinCondition(Layer layer, Layer previousLayer)
        {
            var i = layer.LowerBondIndex;
            for (int j = 1; j < layer.M; j++)
            {
                var fermentReactionSpeed = SimulationParameters.t * (Biosensor.VMax * SPrev[i, j] / (Biosensor.Km + SPrev[i, j]));

                SCur[i, j] = SPrev[i, j] + layer.Substrate.DiffusionCoefficient * SimulationParameters.t *
                             (CalculateDiffusionLayerCoordinateZNextLocation(SPrev[i - 1, j], SPrev[i, j], SPrev[i + 1, j], layer.H)
                              + CalculateDiffusionLayerCoordinateRNextLocation(SPrev[i, j - 1], SPrev[i, j], SPrev[i, j + 1], layer.W, j))
                             - fermentReactionSpeed;

                PCur[i, j] = PPrev[i, j] + layer.Product.DiffusionCoefficient * SimulationParameters.t *
                                         (CalculateDiffusionLayerCoordinateZNextLocation(PPrev[i - 1, j], PPrev[i, j], PPrev[i + 1, j], layer.H)
                                          + CalculateDiffusionLayerCoordinateRNextLocation(PPrev[i, j - 1], PPrev[i, j], PPrev[i, j + 1], layer.W, j))
                                         + fermentReactionSpeed;
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
    }
}