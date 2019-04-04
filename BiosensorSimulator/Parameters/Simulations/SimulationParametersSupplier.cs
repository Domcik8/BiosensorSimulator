using System;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using System.Collections.Generic;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;

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
            var LayerHight = 100;
            M = 100;

            LayersSteps = new List<KeyValuePair<LayerType, long>>
            {
                new KeyValuePair<LayerType, long>(LayerType.SelectiveMembrane, LayerHight),
                new KeyValuePair<LayerType, long>(LayerType.PerforatedMembrane, LayerHight),
                new KeyValuePair<LayerType, long>(LayerType.DiffusionLayer, LayerHight),
                new KeyValuePair<LayerType, long>(
                    LayerType.DiffusionSmallLayer, LayerHight),
                new KeyValuePair<LayerType, long>(LayerType.Enzyme, LayerHight),
                new KeyValuePair<LayerType, long>(LayerType.NonHomogenousLayer, LayerHight)
            };

            long lastLayerMaxIndex = 0;
            foreach (var layer in biosensor.Layers)
            {
                layer.LeftBondIndex = 0;
                layer.RightBondIndex = M;

                layer.N = GetLayerSteps(layer.Type);
                N += layer.N;

                layer.LowerBondIndex = lastLayerMaxIndex;

                lastLayerMaxIndex = layer.UpperBondIndex = lastLayerMaxIndex + layer.N;

                layer.M = M;
                layer.W = layer.Width / layer.M;

                if (layer.Type == LayerType.DiffusionSmallLayer)
                {
                    layer.M = M;
                    layer.W = layer.Width / layer.M;
                }

                if (layer.N == 0) continue;

                layer.H = layer.Height / layer.N;
                layer.R = t / (layer.H * layer.H);

                if (layer.Type == LayerType.NonHomogenousLayer)
                {
                    var microreactiorBiosensor = (BaseMicroreactorBiosensor)biosensor;
                    var enzymeArea = ((LayerWithSubAreas)layer).SubAreas.First();
                    var diffusionArea = ((LayerWithSubAreas)layer).SubAreas.Last();

                    enzymeArea.M = (long)(microreactiorBiosensor.MicroReactorRadius / layer.Width * layer.M);
                    diffusionArea.M = layer.M - enzymeArea.M;

                    enzymeArea.LeftBondIndex = 0;
                    enzymeArea.RightBondIndex = enzymeArea.M;
                    diffusionArea.LeftBondIndex = enzymeArea.RightBondIndex;
                    diffusionArea.RightBondIndex = layer.M;

                    enzymeArea.R = diffusionArea.R = layer.R;
                    enzymeArea.N = diffusionArea.N = layer.N;
                    enzymeArea.H = diffusionArea.H = layer.H;
                    enzymeArea.W = diffusionArea.W = layer.W;
                    enzymeArea.Height = diffusionArea.Height = layer.Height;
                    enzymeArea.LowerBondIndex = diffusionArea.LowerBondIndex = layer.LowerBondIndex;
                    enzymeArea.UpperBondIndex = diffusionArea.UpperBondIndex = layer.UpperBondIndex;
                }
            }

            t = GetMinimalTimestep(biosensor);
        }

        private static double GetMinimalTimestep(BaseBiosensor biosensor)
        {
            var minH = biosensor.Layers.Aggregate((curMin, x) => curMin == null || x.H < curMin.H ? x : curMin).H;
            var maxDiffusionCoefficient = 6.00E-04;

            var diffusionTime = 0.25 * minH * minH / maxDiffusionCoefficient;
            var reactionTime = 0.5 * biosensor.Km / biosensor.VMax;

            return Math.Min(diffusionTime, reactionTime);
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}