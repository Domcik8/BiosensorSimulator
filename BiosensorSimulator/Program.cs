using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Simulations;
using BiosensorSimulator.Simulations.Simulations1D;
using BiosensorSimulator.Results;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            // You can choose different starting conditions
            var biosensor = new FirstOrderSimulation().GetInitiationParameters();
            var simulationParameters = new SimulationParametersSuplier1().InitiationParameters(biosensor);
            var schemeCalculator = new ExplicitSchemeCalculator(biosensor, simulationParameters);
            var resultPrinter = new FilePrinter(@"C:\BiosensorSimulations");

            BaseSimulation simulation = new SingleLayerSimulation1D(simulationParameters, biosensor, schemeCalculator, resultPrinter);

            // Analytic model validation
            simulation.AssertSimulationStability();
            simulation.PrintParameters();
            simulation.ShowValidationValues();
            simulation.RunStableCurrentSimulation();
        }
    }
}