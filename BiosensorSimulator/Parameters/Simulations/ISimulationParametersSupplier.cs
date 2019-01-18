using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Simulations
{
    public interface ISimulationParametersSupplier
    {
        SimulationParameters InitiationParameters(Biosensor biosensor);
    }
}