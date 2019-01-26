using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            if (schemeCalculator is ExplicitSchemeCalculator)
                new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, Biosensor);
        }

        // Calculate next step of biosensor
        public abstract void CalculateNextStep();

        /// <summary>
        /// Runs simulation till eternity. Prints result every on specified times.
        /// </summary>
        public void RunSimulation(int[] resultTimes)
        {
            RunSimulation(int.MaxValue, resultTimes);
        }

        /// <summary>
        /// Runs simulation for x seconds. Prints result every on specified times.
        /// </summary>
        public void RunSimulation(int simulationTime, int[] resultTimes)
        {
            int i, j = 0;
            var resultTicks = new int[resultTimes.Length];
            var m = simulationTime / SimulationParameters.t;

            // Calculate when to print results
            for (var k = 0; k < resultTimes.Length; k++)
                resultTicks[k] = (int)(resultTimes[k] / SimulationParameters.t);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            SetInitialConditions();

            for (i = 1; i <= m; i++)
            {
                CalculateNextStep();

                Current = GetCurrent();

                if (i % resultTicks[j] == 0)
                    PrintSimulationResults(stopWatch, Current, resultTimes[j++]);
            }
            stopWatch.Stop();

            PrintSimulationResults(stopWatch, Current, i * SimulationParameters.t);
        }

        /// <summary>
        /// Runs simulation till eternity. Prints result every 0.5s.
        /// </summary>
        public void RunSimulation()
        {
            RunSimulation(int.MaxValue);
        }

        /// <summary>
        /// Runs simulation for x seconds. Prints result every 0.5s.
        /// </summary>
        /// <param name="simulationTime"></param>
        public void RunSimulation(int simulationTime)
        {
            var i = 0;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var m = simulationTime / SimulationParameters.t;

            //Print result every resultTime seconds
            var resultTime = 0.5;
            // Print result every resulSteps steps
            var resultSteps = (int)(resultTime / SimulationParameters.t);

            SetInitialConditions();

            for (i = 1; i <= m; i++)
            {
                CalculateNextStep();

                Current = GetCurrent();

                if (i % resultSteps == 0)
                    PrintSimulationResults(stopWatch, Current, i / resultSteps * resultTime);
            }
            stopWatch.Stop();

            PrintSimulationResults(stopWatch, Current, i * SimulationParameters.t);
        }

        /// <summary>
        /// Simulation stable current 
        /// </summary>
        public void RunStableCurrentSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            SetInitialConditions();
            var stableCurrent = GetStableCurrent(out var i);

            stopWatch.Stop();

            PrintSimulationResults(stopWatch, stableCurrent, i * SimulationParameters.t);
        }

        // Show ZeroCondition and First condition and two model condition
        public void ShowValidationValues()
        {
            var simulation = new AnalyticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnalyticSolution(Biosensor, SimulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnalyticSolution(Biosensor, SimulationParameters);

            ResultPrinter.Print("====Analytic validations====");
            ResultPrinter.Print($"First order current: {firstOrderCurrent } A/mm^2");
            ResultPrinter.Print($"Zero order current: {zeroOrderCurrent } A/mm^2");

            if (Biosensor.Layers.Count == 2)
            {
                var twoModelCurrent = simulation.GetTwoCompartmentModelAnalyticSolution(Biosensor, SimulationParameters);
                ResultPrinter.Print($"Two model current: {twoModelCurrent } A/mm^2");
            }

            ResultPrinter.Print("");
        }

        /// <summary>
        /// Get stable current 
        /// </summary>
        private double GetStableCurrent(out long i)
        {
            i = 1;
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

        private void PrintSimulationResults(Stopwatch stopwatch, double I, double simulationTime)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Response time: {simulationTime} s");
            ResultPrinter.Print($"Current = {I} A/mm2");
            PrintSimulationConcentrations();
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double I)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Current = {I} A/mm2");
            PrintSimulationConcentrations();
        }

        private void PrintSimulationConcentrations()
        {
            ResultPrinter.Print("");
            for (int i = 0; i < SCur.Length; i++)
                ResultPrinter.Print($"SCur[{i}] = {SCur[i]}");

            ResultPrinter.Print("");
            for (int i = 0; i < SPrev.Length; i++)
                ResultPrinter.Print($"SPrev[{i}] = {SPrev[i]}");
        }

        /// <summary>
        /// Write parameters for result
        /// </summary>
        public void PrintParameters()
        {
            ResultPrinter.Print("*********" + Biosensor.Name + "*********");
            ResultPrinter.Print("");

            if (SchemeCalculator is ImplicitSchemeCalculator)
                ResultPrinter.Print("====Implicit Scheme Calculator====");
            else
                ResultPrinter.Print("====Explicit Scheme Calculator====");

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
                ResultPrinter.Print($"Ds: {biosensorLayer.Substrate?.DiffusionCoefficient} m2/s");
                ResultPrinter.Print($"Steps count: {biosensorLayer.N}");
                ResultPrinter.Print($"Step: {biosensorLayer.H} M");
                ResultPrinter.Print("");
            }
        }
    }
}