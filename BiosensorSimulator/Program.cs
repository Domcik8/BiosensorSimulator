using BiosensorSimulator.Calculators;
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
            var biosensorParameters = new FirstOrderSimulation().GetInitiationParameters();
            var simulationParameters = new Simulation1().InitiationParameters(biosensorParameters);
            var schemeCalculator = new ExplicitSchemeCalculator(biosensorParameters, simulationParameters);
            
            BaseSimulation simulation = new SingleLayerSimulation1D(
                simulationParameters, biosensorParameters, schemeCalculator);

            //Analitic model validation

            
            // AssertSimulation
            simulation.AssertSimulation();
            
            simulation.RunStableCurrentSimulation();

            simulation.ShowValidationValues();

            Console.ReadKey();

        }
    }
}
