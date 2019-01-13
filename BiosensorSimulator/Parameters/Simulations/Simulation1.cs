using System.Collections.Generic;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class Simulation1 : ISimulationParametersSuplier
    {
        private List<KeyValuePair<LayerType, long>> LayersSteps { get; set; }

        public SimulationParameters InitiationParameters(Biosensor biosensor)
        {
            var simulationParameters = new SimulationParameters
            {
                ne = 2,
                DecayRate = 1e-5,
                F = 96485.3329,
                ZeroIBond = 1e-25,         
                t = 7.5e-12,
                N = 100
            };

            LayersSteps = new List<KeyValuePair<LayerType, long>>
            {
                new KeyValuePair<LayerType, long>(LayerType.SelectiveMembrane, 100),
                new KeyValuePair<LayerType, long>(LayerType.PerforatedMembrane, 100),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionLayer, 100),
                new KeyValuePair<LayerType, long>(LayerType.Enzyme, 100)
            };

            foreach (var layer in biosensor.Layers)
            {
                layer.N = GetLayerSteps(layer.Type);
                layer.H = layer.Height / layer.N;
                layer.R = simulationParameters.t / (layer.H * layer.H);

                layer.Product.DiffusionCoefficientOverR = GetSubstanceDiffusionCoefficientOverR(layer.Product, layer.R);
                layer.Substrate.DiffusionCoefficientOverR = GetSubstanceDiffusionCoefficientOverR(layer.Substrate, layer.R);

                layer.Product.DiffusionCoefficientOverSpace = GetSubstanceDiffusionCoefficientOverSpace(layer.Product, layer.H);
                layer.Substrate.DiffusionCoefficientOverSpace = GetSubstanceDiffusionCoefficientOverSpace(layer.Substrate, layer.H);
            }

            return simulationParameters;
        }

        private static double GetSubstanceDiffusionCoefficientOverR(Substance substance, double R)
        {
            return substance.DiffusionCoefficient * R;
        }

        private static double GetSubstanceDiffusionCoefficientOverSpace(Substance substance, double H)
        {
            return substance.DiffusionCoefficient / (H * H);
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}