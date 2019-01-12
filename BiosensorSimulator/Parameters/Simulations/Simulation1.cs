using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class Simulation1 : ISimulationParametersSuplier
    {
        public SimulationParameters InitiationParameters(
            BiosensorParameters biosensorParameters)
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
                t = 7.5e-12
            };
            
            //simulationParameters.hf = biosensorParameters.c / simulationParameters.Nf;
            //simulationParameters.hd = biosensorParameters.n / simulationParameters.Nd;

            foreach (var layer in biosensorParameters.Layers)
            {
                simulationParameters.hf = layer.Height / simulationParameters.Nf;
                layer.N = simulationParameters.Nf;
                layer.H = simulationParameters.hf;

                layer.R = simulationParameters.t / (layer.H * layer.H);

                foreach (Substance s in layer.Substances)
                {
                    s.DiffusionCoefficientOverR = s.DiffusionCoefficient * layer.R;
                    s.DiffusionCoefficientOverSpace = s.DiffusionCoefficient / (layer.H * layer.H);
                }
            }

            return simulationParameters;
        }
    }
}
