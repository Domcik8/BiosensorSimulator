using System;
using BiosensorSimulator.Calculators;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Simulations;

namespace BiosensorSimulator
{
    public class Simulation1D : ISimulation
    {
        public SimulationParameters SimulationParameters { get; }
        public BiosensorParameters BiosensorParameters { get; }
        public ISchemeCalculator SchemeCalculator { get; }
        public CurrentCalculator CurrentCalculator { get; }

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;
        public double SteadyCurrent;

        public Simulation1D(
            SimulationParameters simulationParameters, BiosensorParameters biosensorParameters,
            ISchemeCalculator schemeCalculator, CurrentCalculator currentCalculator
            )
        {
            SimulationParameters = simulationParameters;
            BiosensorParameters = biosensorParameters;
            CurrentCalculator = currentCalculator;
            SchemeCalculator = schemeCalculator;

            AssertSimulationStability(simulationParameters, biosensorParameters);
        }

        public void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N + 1];
            PCur = new double[SimulationParameters.N + 1];
            SPrev = new double[SimulationParameters.N + 1];
            PPrev = new double[SimulationParameters.N + 1];

            SCur[SimulationParameters.N] = BiosensorParameters.S0;
            PCur[SimulationParameters.N] = BiosensorParameters.P0;
        }

        public void RunSimulation()
        {
            SetInitialConditions();
            CalculateNextLayer();
        }

        public void ShowValidationValues(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            var simulation = new AnaliticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnaliticSolution(biosensorParameters, simulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnaliticSolution(biosensorParameters, simulationParameters);

            Console.WriteLine($"First order current : {firstOrderCurrent} nA/mm^2");
            Console.WriteLine($"Zero order current : {zeroOrderCurrent} uA/mm^2");
        }

        public void CalculateNextLayer()
        {
           SchemeCalculator.CalculateNextStep(
                SCur, PCur, SPrev, PPrev,
                BiosensorParameters, SimulationParameters
            );
        }

        public void CalculateSteadyCurrent()
        {
            SteadyCurrent = CurrentCalculator.CalculateStableCurrent(SCur, PCur, SPrev, PPrev);
        }

        private void AssertSimulationStability(SimulationParameters simulationParameters, BiosensorParameters biosensorParameters)
        {
            new ImplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensorParameters);
        }

    }
}
