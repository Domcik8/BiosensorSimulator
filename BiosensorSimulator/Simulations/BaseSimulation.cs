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

            if (schemeCalculator is ExplicitSchemeCalculator)
                new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, Biosensor);
        }

        // Calculate next step of biosensor
        public abstract void CalculateNextStep();

        // Run simulation for x s
        public void RunSimulation()
        {
            RunSimulation(int.MaxValue);
        }

        // Run simulation for x s
        public void RunSimulation(int simulationTime)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var m = simulationTime / SimulationParameters.t;

            //Print result every resultTime seconds
            var resultTime = 0.5;
            // Print result every resulSteps steps
            var resultSteps = (int)(resultTime / SimulationParameters.t);

            SetInitialConditions();

            for (var i = 1; i <= m; i++)
            {
                CalculateNextStep();

                Current = GetCurrent();

                if (i % resultSteps == 0)
                    PrintSimulationResults(stopWatch, Current, i / resultSteps * resultTime);
            }
            PrintSimulationResults(stopWatch, Current);
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
            ResultPrinter.Print($"Simulated biosensor response time: {simulationTime} s");
            ResultPrinter.Print($"Simulation lasted {stopwatch.ElapsedMilliseconds} milliseconds");
            ResultPrinter.Print($"Steady current = {I / 1000000} A/mm2");
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double I)
        {
            ResultPrinter.Print($"Simulation lasted {stopwatch.ElapsedMilliseconds} milliseconds");
            ResultPrinter.Print($"Steady current = {I / 1000000} A/mm2");
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