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
            //t = 4.166666666666665E-06;
            t = 1.6665E-05;
            //t = 8.3e-4;
            //t = 4.166665e-4;
            //t = 4.6295296296296314E-05;
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
                layer.RightBondIndex = M - 1;

                layer.N = GetLayerSteps(layer.Type);
                N += layer.N;

                layer.LowerBondIndex = lastLayerMaxIndex;

                if (lastLayerMaxIndex == 0)
                    lastLayerMaxIndex--;

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
                    var microreactiorBiosensor = (BaseMicroreactorBiosensor) biosensor;
                    var enzymeArea = ((LayerWithSubAreas)layer).SubAreas.First();
                    var diffusionArea = ((LayerWithSubAreas)layer).SubAreas.Last();

                    enzymeArea.M = (long) (microreactiorBiosensor.MicroReactorRadius / layer.Width * layer.M);
                    diffusionArea.M = layer.M - enzymeArea.M;

                    enzymeArea.LeftBondIndex = 0;
                    enzymeArea.RightBondIndex = enzymeArea.M - 1;
                    diffusionArea.LeftBondIndex = enzymeArea.RightBondIndex;
                    diffusionArea.RightBondIndex = layer.M - 1;

                    enzymeArea.R = diffusionArea.R = layer.R;
                    enzymeArea.N = diffusionArea.N = layer.N;
                    enzymeArea.H = diffusionArea.H = layer.H;
                    enzymeArea.W = diffusionArea.W = layer.W;
                    enzymeArea.Height = diffusionArea.Height = layer.Height;
                    enzymeArea.LowerBondIndex = diffusionArea.LowerBondIndex = layer.LowerBondIndex;
                    enzymeArea.UpperBondIndex = diffusionArea.UpperBondIndex = layer.UpperBondIndex;
                }
            }
        }

        private long GetLayerSteps(LayerType layerType)
        {
            return LayersSteps.First(s => s.Key == layerType).Value;
        }
    }
}