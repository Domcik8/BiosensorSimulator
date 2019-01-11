using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.SchemeCalculator;

namespace BiosensorSimulator.Parameters.Simulations
{
    public interface ISimulationParametersSuplier
    {
        SimulationParameters InitiationParameters(BiosensorParameters biosensorParameters);
    }
}
