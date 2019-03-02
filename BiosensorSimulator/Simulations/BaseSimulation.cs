using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using System;
using System.Diagnostics;
using System.Linq;

namespace BiosensorSimulator.Simulations
{
    public abstract class BaseSimulation
    {
        public SimulationParameters SimulationParameters { get; }
        public BaseBiosensor Biosensor { get; }

        protected IResultPrinter ResultPrinter { get; }

        public double Current;

        protected BaseSimulation(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter)
        {
            SimulationParameters = simulationParameters;
            Biosensor = biosensor;
            ResultPrinter = resultPrinter;
        }

        // Calculate next step of biosensor
        public abstract void CalculateNextStep();
        public abstract void CalculateMatchingConditions();
        public abstract void CalculateBoundaryConditions();

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
        public abstract void RunStableCurrentSimulation();
        public void RunStableCurrentSimulation(int maxTime = int.MaxValue)
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
            var maxSteps = (int)(maxTime / SimulationParameters.t);

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

                if (i % maxSteps == 0)
                    break;

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

            ResultPrinter.Print("");
        }

        public abstract double GetCurrent();

        /// <summary>
        /// Set initial biosensor conditions
        /// </summary>
        public abstract void SetInitialConditions();

        public void PrintSimulationResultsSimple(double I)
        {
            ResultPrinter.Print(I.ToString());
        }

        public void PrintSimulationResults(Stopwatch stopwatch, double I, double simulationTime, bool printConcentrations = true, bool normalize = false)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Response time: {simulationTime} s");
            ResultPrinter.Print($"Current = {I} A/mm2");

            if (printConcentrations)
                PrintSimulationConcentrations(normalize);
        }

        public void PrintSimulationResults(Stopwatch stopwatch, double I, bool printConcentrations = true, bool normalize = false)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Current = {I} A/mm2");

            if (printConcentrations)
                PrintSimulationConcentrations(normalize);
        }

        public abstract void PrintSimulationConcentrations(bool normalize = false);

        /// <summary>
        /// Write parameters for result
        /// </summary>
        public void PrintParameters()
        {
            ResultPrinter.Print("*********" + Biosensor.Name + "*********");
            ResultPrinter.Print("");

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

            ResultPrinter.Print("====Main parameters====");
            ResultPrinter.Print($"Diffusion module: " +
                                $"{Biosensor.VMax * Biosensor.EnzymeLayer.Height * Biosensor.EnzymeLayer.Height / (Biosensor.EnzymeLayer.Substrate.DiffusionCoefficient * Biosensor.Km)}");
            ResultPrinter.Print("");

            if (Biosensor is BaseHomogenousBiosensor homogenousBiosensor)
            {
                ResultPrinter.Print("*********Homogenization*********");
                ResultPrinter.Print($"Use Homogenization: {homogenousBiosensor.IsHomogenized}");
                ResultPrinter.Print($"Use EffectiveDiffusionCoefficent: {homogenousBiosensor.UseEffectiveDiffusionCoefficent}");
                ResultPrinter.Print($"Use EffectiveReactionCoefficent: {homogenousBiosensor.UseEffectiveReactionCoefficent}");
                ResultPrinter.Print("");
            }

            if (Biosensor is BaseMicroreactorBiosensor microreactorBiosensor)
            {
                ResultPrinter.Print("*********Microreactor parameters*********");
                ResultPrinter.Print($"Microreactor radius: {microreactorBiosensor.MicroReactorRadius}");
                ResultPrinter.Print($"Unit radius: {microreactorBiosensor.UnitRadius}");
                ResultPrinter.Print("");
            }

            if (Biosensor is BasePerforatedMembraneBiosensor basePerforatedMembraneBiosensor)
            {
                ResultPrinter.Print("*********Perforated membrane parameters*********");
                ResultPrinter.Print($"Hole radius: {basePerforatedMembraneBiosensor.HoleRadius}");
                ResultPrinter.Print($"Half distance between holes: {basePerforatedMembraneBiosensor.HalfDistanceBetweenHoles}");
                ResultPrinter.Print($"Enzyme height in hole: {basePerforatedMembraneBiosensor.EnzymeHoleHeight}");
                ResultPrinter.Print($"Partition coeffient: {basePerforatedMembraneBiosensor.PartitionCoefficient}");
                ResultPrinter.Print("");
            }

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