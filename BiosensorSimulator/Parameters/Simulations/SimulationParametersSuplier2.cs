using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class SimulationParametersSupplier2 : SimulationParameters
    {
        private List<KeyValuePair<LayerType, long>> LayersSteps { get; set; }

        public SimulationParametersSupplier2(BaseBiosensor biosensor)
        {
            ne = 2;
            DecayRate = 1e-2;
            F = 96485.33289;
            ZeroIBond = 1e-25;
            t = 2.5e-7;

            LayersSteps = new List<KeyValuePair<LayerType, long>>
            {
                new KeyValuePair<LayerType, long>(LayerType.SelectiveMembrane, 40),
                new KeyValuePair<LayerType, long>(LayerType.PerforatedMembrane, 40),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionLayer, 40),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionSmallLayer, 40),
                new KeyValuePair<LayerType, long>(LayerType.Enzyme, 40)
            };

            var widthSteps = 40;
            M = widthSteps;

            long lastLayerMaxIndex = 0;
            foreach (var layer in biosensor.Layers)
            {
                if (layer.Height == 0)
                {
                    continue;
                }

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
                    var smallSteps = layer.Width * widthSteps / layer.FullWidth;
                    layer.M = (int)smallSteps;
                    layer.W = layer.FullWidth / widthSteps;
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