using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
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
            var biosensor = new ZeroOrderSimulation().GetInitiationParameters();
            var simulationParameters = new SimulationParametersSuplier1().InitiationParameters(biosensor);
            var schemeCalculator = new ExplicitSchemeCalculator(biosensor, simulationParameters);

            BaseSimulation simulation = new SingleLayerSimulation1D(simulationParameters, biosensor, schemeCalculator);

            //Analitic model validation
            simulation.AssertSimulationStability();
            simulation.ShowValidationValues();
            simulation.RunStableCurrentSimulation();

            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }
    }
}
