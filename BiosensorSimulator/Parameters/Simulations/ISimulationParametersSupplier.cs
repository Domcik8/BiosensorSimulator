using BiosensorSimulator.Parameters.Biosensors.Base;

namespace BiosensorSimulator.Parameters.Simulations
{
    public interface ISimulationParametersSupplier
    {
        SimulationParameters InitiationParameters(Biosensor biosensor);
    }
}