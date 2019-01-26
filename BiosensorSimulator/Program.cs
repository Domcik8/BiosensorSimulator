using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Simulations;
using BiosensorSimulator.Simulations.Simulations1D;
using System;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            // You can choose different starting conditions
            var biosensor = new FirstOrderBiosensor().GetInitiationParameters();
            var simulationParameters = new SimulationParametersSuplier1().InitiationParameters(biosensor);
            var schemeCalculator = new ImplicitSchemeCalculator(biosensor, simulationParameters);

            var resultPrinter = new ConsolePrinter();
            //var resultPrinter = new FilePrinter(@"C:\BiosensorSimulations");

            BaseSimulation simulation = new SingleLayerSimulation1D(simulationParameters, biosensor, schemeCalculator, resultPrinter);

            // Analytic model validation
            simulation.PrintParameters();
            simulation.ShowValidationValues();

            resultPrinter.Print("====Results====");

            //simulation.RunStableCurrentSimulation();
            simulation.RunSimulation(10);

            if (resultPrinter is ConsolePrinter)
            {
                Console.ReadKey();
                Console.ReadKey();
                Console.ReadKey();
            }
        }
    }
}