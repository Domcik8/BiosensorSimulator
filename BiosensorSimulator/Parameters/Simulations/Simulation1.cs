using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.SchemeCalculator;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class Simulation1 : ISimulationParametersSuplier
    {
        public SimulationParameters InitiationParameters(BiosensorParameters biosensorParameters)
        {
            SimulationParameters simulationParameters = new SimulationParameters()
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
            
            simulationParameters.hf = biosensorParameters.c / simulationParameters.Nf;
            simulationParameters.hd = biosensorParameters.n / simulationParameters.Nd;

            return simulationParameters;
        }
    }
}
