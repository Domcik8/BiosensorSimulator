using BiosensorSimulator.Calculators;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Simulations
{
    public interface ISimulation
    {
        SimulationParameters SimulationParameters { get; }
        BiosensorParameters BiosensorParameters { get; }
        ISchemeCalculator SchemeCalculator { get; }
        CurrentCalculator CurrentCalculator { get; }

        // Run simulation for x s
        void RunSimulation();

        // Get simulation stable surrent 
        void RunStableCurrentSimulation();

        // Show ZeroCondition and First condition
        void ShowValidationValues();

        // Assert that simulation steps are correct
        void AssertSimulation();
    }
}
