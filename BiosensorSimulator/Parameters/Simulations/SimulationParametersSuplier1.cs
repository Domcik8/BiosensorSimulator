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
            DecayRate = 1e-2;
            F = 96485.33289;
            ZeroIBond = 1e-25;
            t = 7.5e-8;

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

                if (layer.N == 0) continue;

                layer.H = layer.Height / layer.N;
                layer.R = t / (layer.H * layer.H);
            }
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}