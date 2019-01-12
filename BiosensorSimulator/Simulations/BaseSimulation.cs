using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;
using System.Diagnostics;

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

        /// <summary>
        /// Simulation stable current 
        /// </summary>
        public void RunStableCurrentSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var stableCurrent = GetStableCurrent();

            stopWatch.Stop();

            PrintSimulationResults(stopWatch, stableCurrent);
        }

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

        /// <summary>
        /// Assert that simulation steps are correct
        /// </summary>
        public void AssertSimulationStability()
        {
            new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, BiosensorParameters);
        }

        /// <summary>
        /// Set initial biosensor conditions
        /// </summary>
        private void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N + 1];
            PCur = new double[SimulationParameters.N + 1];
            SPrev = new double[SimulationParameters.N + 1];
            PPrev = new double[SimulationParameters.N + 1];

            SCur[SimulationParameters.N] = BiosensorParameters.S0;
            PCur[SimulationParameters.N] = BiosensorParameters.P0;
        }

        /// <summary>
        /// Get stable current 
        /// </summary>
        private double GetStableCurrent()
        {
            long i = 1;
            double iPrev = 0;
            var currentFactor = SimulationParameters.ne * SimulationParameters.F * BiosensorParameters.DPf / SimulationParameters.hf;

            while (true)
            {
                CalculateNextStep();

                var iCur = PCur[1] * currentFactor;

                if (iCur > 0 && iPrev > 0
                    && iCur > SimulationParameters.ZeroIBond
                    && Math.Abs(iCur - iPrev) * i / iCur < SimulationParameters.DecayRate
                )
                    return iCur;

                iPrev = iCur;
                i++;
            }
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double I)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");
            Console.WriteLine($"Steady current = {I}");
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double[] sCur, double[] pCur)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");

            for (int i = 0; i < sCur.Length; i++)
                Console.WriteLine($"S[{i}] = {sCur[i]}, P[{i}] = {pCur[i]}");
        }
    }
}
