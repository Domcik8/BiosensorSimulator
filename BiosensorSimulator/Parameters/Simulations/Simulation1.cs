using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class Simulation1 : ISimulationParametersSuplier
    {
        public SimulationParameters InitiationParameters(BiosensorParameters biosensorParameters)
        {
            var simulationParameters = new SimulationParameters()
            {
                ne = 2,
                DecayRate = 1e-5,
                F = 96485.3329,
                ZeroIBond = 1e-25,
                Nd = 100,
                Nf = 100,
                N = 100,
                t = 0.0000000000075
            };
            
            //simulationParameters.hf = biosensorParameters.c / simulationParameters.Nf;
            //simulationParameters.hd = biosensorParameters.n / simulationParameters.Nd;

            foreach (var layer in biosensorParameters.Layers)
            {
                if (layer.Type == LayerType.Enzyme)
                {
                    simulationParameters.hf = layer.Height / simulationParameters.Nf;
                    layer.N = simulationParameters.Nf;
                    layer.H = simulationParameters.hf;
                }

                if (layer.Type == LayerType.DiffusionLayer)
                {
                    simulationParameters.hf = layer.Height / simulationParameters.Nd;
                    layer.N = simulationParameters.Nd;
                    layer.H = simulationParameters.hd;
                }
            }

            return simulationParameters;
        }
    }
}
