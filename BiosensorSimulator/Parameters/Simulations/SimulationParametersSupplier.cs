using System;
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
            DecayRate = 1e-1;
            F = 96485.33289;
            ZeroIBond = 1e-25;
            t = 9.0E-07;
            //t = 4.166665e-4;
            //t = 4.6295296296296314E-05;

            LayersSteps = new List<KeyValuePair<LayerType, long>>
            {
                new KeyValuePair<LayerType, long>(LayerType.SelectiveMembrane, 150),
               // new KeyValuePair<LayerType, long>(LayerType.PerforatedMembrane, 20),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionLayer, 150),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionSmallLayer, 20),
                new KeyValuePair<LayerType, long>(LayerType.EnzymeSmallLayer, 20),
                new KeyValuePair<LayerType, long>(LayerType.Enzyme, 150)
            };

            var widthSteps = 80;
            M = widthSteps;

            long lastLayerMaxIndex = 0;
            foreach (var layer in biosensor.Layers)
            {
                layer.N = GetLayerSteps(layer.Type);
                N += layer.N;

                layer.LowerBondIndex = lastLayerMaxIndex;

                //if (lastLayerMaxIndex == 0)
                //    lastLayerMaxIndex--;

                lastLayerMaxIndex = layer.UpperBondIndex = lastLayerMaxIndex + layer.N;

                layer.M = widthSteps;
                layer.W = layer.Width / layer.M;

                if (layer.Type == LayerType.DiffusionLayer)
                {
                    var smallSteps = layer.Width * widthSteps / layer.FullWidth;
                    layer.M = (int)smallSteps ;
                    layer.W = layer.FullWidth / widthSteps;

                    var smallSteps2 = layer.Height * layer.N / layer.FullHeight;
                    var round = Math.Round(smallSteps2);
                    layer.M2 = layer.UpperBondIndex - (int) round;
                }

                if (layer.Type == LayerType.Enzyme)
                {
                    var smallSteps = layer.Width * widthSteps / layer.FullWidth;
                    layer.M = (int)smallSteps;
                    layer.W = layer.FullWidth / widthSteps;

                    var smallSteps2 = layer.Height * layer.N / layer.FullHeight;
                    var round = Math.Round(smallSteps2);
                    layer.M2 = layer.LowerBondIndex + (int)round;
                }

                if (layer.N == 0) continue;

                layer.H = layer.FullHeight / layer.N;
                layer.R = t / (layer.H * layer.H);
            }
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}