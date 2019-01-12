using System;
using System.Diagnostics;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Simulations
{
    public abstract class BaseSimulation
    {
        public SimulationParameters SimulationParameters { get; }
        public BiosensorParameters BiosensorParameters { get; }
        public ISchemeCalculator SchemeCalculator { get; }

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;
        public double SteadyCurrent;

        protected BaseSimulation(
            SimulationParameters simulationParameters,
            BiosensorParameters biosensorParameters,
            ISchemeCalculator schemeCalculator)
        {
            SimulationParameters = simulationParameters;
            BiosensorParameters = biosensorParameters;
            SchemeCalculator = schemeCalculator;
        }

        // Run simulation for x s
        public void RunSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            SetInitialConditions();

            for (var i = 0; i < SimulationParameters.M; i++)
                CalculateNextStep();

            stopWatch.Stop();
        }

        // Get simulation stable surrent 
        public abstract void RunStableCurrentSimulation();

        // Calculate next step of biosensor
        public abstract void CalculateNextStep();

        // Show ZeroCondition and First condition
        public void ShowValidationValues()
        {
            var simulation = new AnaliticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnaliticSolution(BiosensorParameters, SimulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnaliticSolution(BiosensorParameters, SimulationParameters);

            Console.WriteLine($"First order current : {firstOrderCurrent} nA/mm^2");
            Console.WriteLine($"Zero order current : {zeroOrderCurrent} uA/mm^2");
        }

        // Assert that simulation steps are correct
        public abstract void AssertSimulation();

        public abstract void SetInitialConditions();
    }
}
