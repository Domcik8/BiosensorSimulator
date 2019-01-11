using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.SchemeCalculator;

namespace BiosensorSimulator
{
    public interface ISimulation
    {
        SimulationParameters SimulationParameters { get; }
        BiosensorParameters BiosensorParameters { get; }
        ISchemeCalculator SchemeCalculator { get; }
        CurrentCalculator CurrentCalculator { get; }
    }
}
