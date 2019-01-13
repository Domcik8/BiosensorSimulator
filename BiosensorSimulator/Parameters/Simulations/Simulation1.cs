using System.Collections.Generic;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Scheme;

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

                layer.Product.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Product);
                layer.Substrate.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Substrate);

                layer.Product.ImplicitScheme = new ImplicitSchemeParameters(layer, layer.Product);
                layer.Substrate.ImplicitScheme = new ImplicitSchemeParameters(layer, layer.Substrate);
            }

            return simulationParameters;
        }

        private static ExplicitSchemeParameters GetExplicitSchemeParameters(Layer layer, Substance substance)
        {
            return new ExplicitSchemeParameters
            {
                DiffusionCoefficientOverR = substance.DiffusionCoefficient * layer.R,
                DiffusionCoefficientOverSpace = substance.DiffusionCoefficient / (layer.H * layer.H)
            };
        }

        private static ImplicitSchemeParameters GetImplicitSchemeParameters(Layer layer, Substance substance)
        {
            return new ImplicitSchemeParameters();
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}