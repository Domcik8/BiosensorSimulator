using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class SimulationParametersSupplier : SimulationParameters
    {
        private List<KeyValuePair<LayerType, long>> LayersSteps { get; set; }

        public SimulationParametersSupplier(BaseBiosensor biosensor)
        {
            ne = 2;
            DecayRate = 1e-2;
            F = 96485.33289;
            ZeroIBond = 1e-25;
            //t = 8.3e-4;
            t = 4.166665e-4;

            LayersSteps = new List<KeyValuePair<LayerType, long>>
            {
                new KeyValuePair<LayerType, long>(LayerType.SelectiveMembrane, 100),
                new KeyValuePair<LayerType, long>(LayerType.PerforatedMembrane, 100),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionLayer, 100),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionSmallLayer, 100),
                new KeyValuePair<LayerType, long>(LayerType.Enzyme, 100)
            };

            var widthSteps = 100;
            M = widthSteps;

            long lastLayerMaxIndex = 0;
            foreach (var layer in biosensor.Layers)
            {
                layer.N = GetLayerSteps(layer.Type);
                N += layer.N;

                layer.LowerBondIndex = lastLayerMaxIndex;

                if (lastLayerMaxIndex == 0)
                    lastLayerMaxIndex--;

                lastLayerMaxIndex = layer.UpperBondIndex = lastLayerMaxIndex + layer.N;

                layer.M = widthSteps;
                layer.W = layer.Width / layer.M;

                if (layer.Type == LayerType.DiffusionSmallLayer)
                {
                    layer.M = widthSteps;
                    layer.W = layer.Width / layer.M;
                }

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