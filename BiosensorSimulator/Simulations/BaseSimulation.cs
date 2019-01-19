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
        public Biosensor Biosensor { get; }
        public ISchemeCalculator SchemeCalculator { get; }

        public double CurrentFactor { get; }

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;
        public double Current;

        protected BaseSimulation(
            SimulationParameters simulationParameters,
            Biosensor biosensor,
            ISchemeCalculator schemeCalculator)
        {
            SimulationParameters = simulationParameters;
            Biosensor = biosensor;
            SchemeCalculator = schemeCalculator;

            var enzymeLayer = biosensor.EnzymeLayer;
            CurrentFactor = simulationParameters.ne * simulationParameters.F * enzymeLayer.Product.DiffusionCoefficient / enzymeLayer.H;
        }

        // Run simulation for x s
        public void RunSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            SetInitialConditions();

            for (var i = 0; i < SimulationParameters.M; i++)
                CalculateNextStep();

            Current = GetCurrent();

            stopWatch.Stop();

            PrintSimulationResults(stopWatch, Current);
        }

        /// <summary>
        /// Simulation stable current 
        /// </summary>
        public void RunStableCurrentSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            SetInitialConditions();
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
            var firstOrderCurrent = simulation.GetFirstOrderAnaliticSolution(Biosensor, SimulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnaliticSolution(Biosensor, SimulationParameters);

            Console.WriteLine($"First order current : {firstOrderCurrent} A/mm^2");
            Console.WriteLine($"Zero order current : {zeroOrderCurrent} A/mm^2");
        }

        /// <summary>
        /// Assert that simulation steps are correct
        /// </summary>
        public void AssertSimulationStability()
        {
            new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, Biosensor);
        }

        /// <summary>
        /// Set initial biosensor conditions
        /// </summary>
        private void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N];
            PCur = new double[SimulationParameters.N];
            SPrev = new double[SimulationParameters.N];
            PPrev = new double[SimulationParameters.N];

            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            PCur[SimulationParameters.N - 1] = Biosensor.P0;
        }

        /// <summary>
        /// Get stable current 
        /// </summary>
        private double GetStableCurrent()
        {
            long i = 1;
            double iPrev = 0;

            while (true)
            {
                CalculateNextStep();

                var iCur = GetCurrent();

                if (iCur > 0 && iPrev > 0
                    && iCur > SimulationParameters.ZeroIBond
                    && Math.Abs(iCur - iPrev) * i / iCur < SimulationParameters.DecayRate
                )
                    return iCur;

                iPrev = iCur;
                i++;
            }
        }

        private double GetCurrent()
        {
            return PCur[1] * CurrentFactor;
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double I)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");
            Console.WriteLine($"Steady current = {I} A/m^2");
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double[] sCur, double[] pCur, double I)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");
            Console.WriteLine($"Current = {I} A/m^2");

            for (var i = 0; i < sCur.Length; i++)
                Console.WriteLine($"S[{i}] = {sCur[i]}, P[{i}] = {pCur[i]}");
        }
    }
}