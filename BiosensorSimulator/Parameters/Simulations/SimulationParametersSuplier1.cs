using BiosensorSimulator.Calculators.SchemeParameters;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class SimulationParametersSuplier1 : SimulationParameters
    {
        private List<KeyValuePair<LayerType, long>> LayersSteps { get; set; }

        public SimulationParametersSuplier1(BaseBiosensor biosensor)
        {
            ne = 2;
            DecayRate = 1e-6;
            F = 96485.33289;
            ZeroIBond = 1e-25;
            t = 7.5e-6;

            LayersSteps = new List<KeyValuePair<LayerType, long>>
            {
                new KeyValuePair<LayerType, long>(LayerType.SelectiveMembrane, 100),
                new KeyValuePair<LayerType, long>(LayerType.PerforatedMembrane, 100),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionLayer, 100),
                new KeyValuePair<LayerType, long>(LayerType.Enzyme, 100)
            };

            long lastLayerMaxIndex = 0;
            foreach (var layer in biosensor.Layers)
            {
                layer.N = GetLayerSteps(layer.Type);
                N += layer.N;

                layer.LowerBondIndex = lastLayerMaxIndex;

                if (lastLayerMaxIndex == 0)
                    lastLayerMaxIndex--;

                lastLayerMaxIndex = layer.UpperBondIndex = lastLayerMaxIndex + layer.N;

                if (layer.N != 0)
                {
                    layer.H = layer.Height / layer.N;
                    layer.R = t / (layer.H * layer.H);
                }

                layer.Product.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Product);
                layer.Product.ImplicitScheme = new ImplicitSchemeParameters(biosensor, layer, layer.Product);

                if (layer.Type == LayerType.SelectiveMembrane)
                {
                    continue;
                }

                layer.Substrate.ExplicitScheme = new ExplicitSchemeParameters(layer, layer.Substrate);
                layer.Substrate.ImplicitScheme = new ImplicitSchemeParameters(biosensor, layer, layer.Substrate);
            }
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}