using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Simulations;
using System;
using BiosensorSimulator.Simulations.Simulations1D;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // You can choose different starting conditions
            var biosensor = new ZeroOrderSimulation().GetInitiationParameters();
            var simulationParameters = new Simulation1().InitiationParameters(biosensor);
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
