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

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;
        public double Current;
        public double CurrentFactor { get; }

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

            if (schemeCalculator is ExplicitSchemeCalculator)
                new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, Biosensor);

            var enzymeLayer = biosensor.EnzymeLayer;
            CurrentFactor = simulationParameters.ne * simulationParameters.F * enzymeLayer.Product.DiffusionCoefficient / enzymeLayer.H;
        }

        // Calculate next step of biosensor
        public abstract void CalculateNextStep();

        /// <summary>
        /// Runs simulation till eternity. Prints result every on specified times.
        /// </summary>
        public void RunSimulation(double[] resultTimes)
        {
            RunSimulation(int.MaxValue, resultTimes);
        }

        /// <summary>
        /// Runs simulation for x seconds. Prints result every on specified times.
        /// </summary>
        public void RunSimulation(double simulationTime, double[] resultTimes, bool normalize = false)
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

                if (j < resultTimes.Length && i % resultTicks[j] == 0)
                    PrintSimulationResults(stopWatch, Current, resultTimes[j++], normalize);
            }
            stopWatch.Stop();

            PrintSimulationResults(stopWatch, Current, i * SimulationParameters.t, normalize);
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
            double iCur;
            var i = 1;
            double iPrev = 0;
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            SetInitialConditions();

            //Print result every resultTime seconds
            var resultTime = 0.5;
            // Print result every resulSteps steps
            var resultSteps = (int)(resultTime / SimulationParameters.t);

            while (true)
            {
                CalculateNextStep();

                iCur = GetCurrent();

                if (iCur > 0 && iPrev > 0
                    && iCur > SimulationParameters.ZeroIBond
                    && Math.Abs(iCur - iPrev) * i / iCur < SimulationParameters.DecayRate)
                    break;

                if (i % resultSteps == 0)
                    PrintSimulationResults(stopWatch, iCur, i / resultSteps * resultTime, false);

                iPrev = iCur;
                i++;
            }
            
            stopWatch.Stop();

            PrintSimulationResults(stopWatch, iCur, i * SimulationParameters.t, false);
            Current = iCur;
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

        public double GetCurrent()
        {
            return PCur[1] * CurrentFactor;
        }

        public abstract void Homogenize();

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

        private void PrintSimulationResults(Stopwatch stopwatch, double I, double simulationTime, bool printConcentrations = true, bool normalize = false)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Response time: {simulationTime} s");
            ResultPrinter.Print($"Current = {I} A/mm2");

            if (printConcentrations)
                PrintSimulationConcentrations(normalize);
        }

        private void PrintSimulationResults(Stopwatch stopwatch, double I, bool printConcentrations = true, bool normalize = false)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Current = {I} A/mm2");

            if (printConcentrations)
                PrintSimulationConcentrations(normalize);
        }

        private void PrintSimulationConcentrations(bool normalize = false)
        {
            if (normalize)
            {
                ResultPrinter.Print("");
                for (var i = 0; i < SCur.Length; i++)
                    ResultPrinter.Print($"SCur[{i}] = {SCur[i] / Biosensor.S0}");

                ResultPrinter.Print("");
                for (var i = 0; i < PCur.Length; i++)
                    ResultPrinter.Print($"PCur[{i}] = {PCur[i] / Biosensor.S0}");
            }
            else
            {
                ResultPrinter.Print("");
                for (var i = 0; i < SCur.Length; i++)
                    ResultPrinter.Print($"SCur[{i}] = {SCur[i]}");

                ResultPrinter.Print("");
                for (var i = 0; i < PCur.Length; i++)
                    ResultPrinter.Print($"PCur[{i}] = {PCur[i]}");
            }
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
            ResultPrinter.Print($"Height: {Biosensor.Height} m");
            ResultPrinter.Print($"Km: {Biosensor.Km} M");
            ResultPrinter.Print($"S0: {Biosensor.S0} M");
            ResultPrinter.Print($"Vmax: {Biosensor.VMax} M/s");
            ResultPrinter.Print($"Time step: {SimulationParameters.t} s");
            ResultPrinter.Print($"Steps: {SimulationParameters.N}");
            ResultPrinter.Print($"Decay rate: {SimulationParameters.DecayRate}");
            ResultPrinter.Print("");

            ResultPrinter.Print("*********Homogenization*********");
            ResultPrinter.Print($"Use Homogenization: {Biosensor.IsHomogenized}");
            ResultPrinter.Print($"Use EffectiveDiffusionCoefficent: {Biosensor.UseEffectiveDiffusionCoefficent}");
            ResultPrinter.Print($"Use EffectiveReactionCoefficent: {Biosensor.UseEffectiveReactionCoefficent}");
            ResultPrinter.Print("");

            ResultPrinter.Print("*********Microreactor parameters*********");
            ResultPrinter.Print($"Microreactor radius: {Biosensor.MicroReactorRadius}");
            ResultPrinter.Print($"Unit radius: {Biosensor.UnitRadius}");
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