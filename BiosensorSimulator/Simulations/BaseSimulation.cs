using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;
using System.Diagnostics;
using BiosensorSimulator.Results;

namespace BiosensorSimulator.Simulations
{
    public abstract class BaseSimulation
    {
        public SimulationParameters SimulationParameters { get; }
        public Biosensor Biosensor { get; }
        public ISchemeCalculator SchemeCalculator { get; }

        protected IResultPrinter ResultPrinter { get; }

        public double CurrentFactor { get; }

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;
        public double Current;

        protected BaseSimulation(
            SimulationParameters simulationParameters,
            Biosensor biosensor,
            ISchemeCalculator schemeCalculator,
            IResultPrinter resultPrinter)
        {
            SimulationParameters = simulationParameters;
            Biosensor = biosensor;
            SchemeCalculator = schemeCalculator;
            ResultPrinter = resultPrinter;

            var enzymeLayer = biosensor.EnzymeLayer;
            CurrentFactor = simulationParameters.ne * simulationParameters.F * enzymeLayer.Product.DiffusionCoefficient / enzymeLayer.H;
        }

        // Calculate next step of biosensor
        public abstract void CalculateNextStep();

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

        // Show ZeroCondition and First condition and two model condition
        public void ShowValidationValues()
        {
            var simulation = new AnalyticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnalyticSolution(Biosensor, SimulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnalyticSolution(Biosensor, SimulationParameters);

            ResultPrinter.Print("====Analytic validations====");
            ResultPrinter.Print($"First order current: {firstOrderCurrent / 1000000} A/mm^2");
            ResultPrinter.Print($"Zero order current: {zeroOrderCurrent / 1000000} A/mm^2");

            if (Biosensor.Layers.Count == 2)
            {
                var twoModelCurrent = simulation.GetTwoCompartmentModelAnalyticSolution(Biosensor, SimulationParameters);
                ResultPrinter.Print($"Two model current: {twoModelCurrent / 1000000} A/mm^2");
            }
   
            ResultPrinter.Print("");
        }

        /// <summary>
        /// Assert that simulation steps are correct
        /// </summary>
        public void AssertSimulationStability()
        {
            new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, Biosensor);
        }

        /// <summary>
        /// Write parameters for result
        /// </summary>
        public void PrintParameters()
        {
            ResultPrinter.Print("*********" + Biosensor.Name + "*********");
            ResultPrinter.Print("");
            ResultPrinter.Print("====Parameters====");
            ResultPrinter.Print($"Km: {Biosensor.Km} M");
            ResultPrinter.Print($"S0: {Biosensor.S0} M");
            ResultPrinter.Print($"Vmax: {Biosensor.VMax} M/s");
            ResultPrinter.Print($"Time step: {SimulationParameters.t} s");
            ResultPrinter.Print($"Steps: {SimulationParameters.N}");
            ResultPrinter.Print($"Decay rate: {SimulationParameters.DecayRate}");
            ResultPrinter.Print("");

            foreach (var biosensorLayer in Biosensor.Layers)
            {
                ResultPrinter.Print($"{biosensorLayer.Type}:");
                ResultPrinter.Print($"Height: {biosensorLayer.Height} m");
                ResultPrinter.Print($"Dp: {biosensorLayer.Product.DiffusionCoefficient} m2/s");
                ResultPrinter.Print($"Ds: {biosensorLayer.Substrate.DiffusionCoefficient} m2/s");
                ResultPrinter.Print($"Steps count: {biosensorLayer.N}");
                ResultPrinter.Print($"Step: {biosensorLayer.H} M");
                ResultPrinter.Print("");
            }
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

            SCur[SimulationParameters.N] = Biosensor.S0;
            PCur[SimulationParameters.N] = Biosensor.P0;
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
                             && Math.Abs(iCur - iPrev) * i / iCur < SimulationParameters.DecayRate)
                {
                    return iCur;
                }

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
            ResultPrinter.Print("====Results====");
            ResultPrinter.Print($"Simulation lasted {stopwatch.ElapsedMilliseconds} milliseconds");
            ResultPrinter.Print($"Steady current = {I / 1000000 } A/mm2");

            if (ResultPrinter is ConsolePrinter)
            {
                Console.ReadKey();
            }
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double[] sCur, double[] pCur, double I)
        {
            ResultPrinter.Print($"Simulation lasted {stopwatch.ElapsedMilliseconds} milliseconds");
            ResultPrinter.Print($"Current = {I} A");

            for (var i = 0; i < sCur.Length; i++)
            {
                ResultPrinter.Print($"S[{i}] = {sCur[i]}, P[{i}] = {pCur[i]}");
            }
        }
    }
}