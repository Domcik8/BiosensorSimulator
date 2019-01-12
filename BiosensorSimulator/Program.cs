using BiosensorSimulator.Calculators;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Simulations;
using System;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // You can choose different starting conditions
            var biosensorParameters = new FirstOrderSimulation().GetInitiationParameters();
            var simulationParameters = new Simulation1().InitiationParameters(biosensorParameters);
            var schemeCalculator = new ImplicitSchemeCalculator(biosensorParameters, simulationParameters);

            var currentCalculator = new CurrentCalculator(simulationParameters, biosensorParameters, schemeCalculator);

            ISimulation simulation = new Simulation1D(simulationParameters, biosensorParameters, schemeCalculator, currentCalculator);

            //Analitic model validation

            
            // AssertSimulation
            simulation.AssertSimulation();
            
            simulation.RunStableCurrentSimulation();

            simulation.ShowValidationValues();

            Console.ReadKey();

        }
    }
}
