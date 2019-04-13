using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
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

        protected double GetResultTime()
        {
            switch (Biosensor.DiffusionLayer.Height)
            {
                case 2e-2:
                case 4e-2:
                    return 0.5;
                case 2e-1:
                    return 5;
                case 4e-1:
                    return 10;
                case 2e0:
                    return 50;

                default:
                    return 0.5;
            }
        }

        /// <summary>
        /// Simulation stable current 
        /// </summary>
        public abstract void RunStableCurrentSimulation(int maxTime = int.MaxValue);

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

        public void PrintSimulationResults(Stopwatch stopwatch, double I, double simulationTime,
            bool printConcentrations = true, bool normalize = false)
        {
            ResultPrinter.Print("");
            ResultPrinter.Print("----------------------------------------------------");
            ResultPrinter.Print($"Simulation time: {stopwatch.ElapsedMilliseconds} ms");
            ResultPrinter.Print($"Response time: {simulationTime} s");
            ResultPrinter.Print($"Current = {I} A/mm2");

            if (printConcentrations)
                PrintSimulationConcentrations(normalize);
        }

        public void PrintSimulationResults(Stopwatch stopwatch, double I,
            bool printConcentrations = true, bool normalize = false)
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
        public virtual void PrintParameters()
        {
            ResultPrinter.Print("***************************" + Biosensor.Name + "***************************");
            ResultPrinter.Print("");

            PrintMainBiosensorParameters();

            ResultPrinter.Print("====Biosensor parameters====");
            ResultPrinter.Print($"Height: {Biosensor.Height} m");
            ResultPrinter.Print($"Km: {Biosensor.Km} M");
            ResultPrinter.Print($"S0: {Biosensor.S0} M");
            ResultPrinter.Print($"Vmax: {Biosensor.VMax} M/s");
            ResultPrinter.Print("====Simulation parameters====");
            ResultPrinter.Print($"Time step: {SimulationParameters.t} s");
            ResultPrinter.Print($"Steps: {SimulationParameters.N}");
            ResultPrinter.Print($"Decay rate: {SimulationParameters.DecayRate}");
            ResultPrinter.Print("");

            if (Biosensor is BaseMicroreactorBiosensor microreactorBiosensor)
                PrintMicroreactorParameters(microreactorBiosensor);

            if (Biosensor is BasePerforatedMembraneBiosensor basePerforatedMembraneBiosensor)
                PrintPerforatedMembraneParameters(basePerforatedMembraneBiosensor);

            foreach (var biosensorLayer in Biosensor.Layers)
            {
                PrintAreaParameters(biosensorLayer);

                if (biosensorLayer.Type == LayerType.NonHomogenousLayer)
                    foreach (var area in ((LayerWithSubAreas)biosensorLayer).SubAreas)
                        PrintAreaParameters(area);
            }
        }

        private void PrintMainBiosensorParameters()
        {
            ResultPrinter.Print("====Biosensor Main parameters====");
            ResultPrinter.Print($"o2: {GetBiosensorDimensionlessParameterO2()}");
            ResultPrinter.Print($"S0: {GetBiosensorDimensionlessParameterS0()}");
            ResultPrinter.Print($"y: {GetBiosensorDimensionlessParameterY()}");
            ResultPrinter.Print($"Bi: {GetBiosensorDimensionlessParameterBi()}");
        }

        private double GetBiosensorDimensionlessParameterBi()
        {
            var effectiveDiffusionCoefficient = ((BaseMicroreactorBiosensor)Biosensor).GetEffectiveDiffusionCoefficent(
                Biosensor.NonHomogenousLayer.Product.DiffusionCoefficient,
                Biosensor.DiffusionLayer.Product.DiffusionCoefficient);

            return Biosensor.Layers.First().Height * Biosensor.DiffusionLayer.Substrate.DiffusionCoefficient
                   / effectiveDiffusionCoefficient
                   / Biosensor.DiffusionLayer.Height;
        }

        private double GetBiosensorDimensionlessParameterY()
        {
            return Biosensor.NonHomogenousLayer.SubAreas.First().Width / Biosensor.NonHomogenousLayer.Width;
        }

        private double GetBiosensorDimensionlessParameterO2()
        {
            var effectiveDiffusionCoefficient = ((BaseMicroreactorBiosensor)Biosensor).GetEffectiveDiffusionCoefficent(
                Biosensor.NonHomogenousLayer.Product.DiffusionCoefficient,
                Biosensor.DiffusionLayer.Product.DiffusionCoefficient);

            var height = Biosensor.Layers.First().Height;

            return Biosensor.VMax * height * height
                   / Biosensor.Km
                   / effectiveDiffusionCoefficient;
        }

        private double GetBiosensorDimensionlessParameterS0()
        {
            return Biosensor.S0 / Biosensor.Km;
        }

        private void PrintAreaParameters(Area area)
        {
            ResultPrinter.Print($"{area.Type}:");
            ResultPrinter.Print($"Height: {area.Height} m");
            ResultPrinter.Print($"Width: {area.Width} m");
            ResultPrinter.Print($"Dp: {area.Product.DiffusionCoefficient} m2/s");
            ResultPrinter.Print($"Ds: {area.Substrate?.DiffusionCoefficient} m2/s");
            ResultPrinter.Print($"Steps count: {area.N}");
            ResultPrinter.Print($"Step: {area.H} M");
            ResultPrinter.Print("");
        }

        private void PrintMicroreactorParameters(BaseMicroreactorBiosensor microreactorBiosensor)
        {
            ResultPrinter.Print("*********Microreactor parameters*********");
            ResultPrinter.Print($"Microreactor radius: {microreactorBiosensor.MicroReactorRadius}");
            ResultPrinter.Print($"Unit radius: {microreactorBiosensor.UnitRadius}");
            ResultPrinter.Print("");
        }

        private void PrintPerforatedMembraneParameters(BasePerforatedMembraneBiosensor basePerforatedMembraneBiosensor)
        {
            ResultPrinter.Print("*********Perforated membrane parameters*********");
            ResultPrinter.Print($"Hole radius: {basePerforatedMembraneBiosensor.HoleRadius}");
            ResultPrinter.Print($"Half distance between holes: {basePerforatedMembraneBiosensor.HalfDistanceBetweenHoles}");
            ResultPrinter.Print($"Enzyme height in hole: {basePerforatedMembraneBiosensor.EnzymeHoleHeight}");
            ResultPrinter.Print($"Partition coeffient: {basePerforatedMembraneBiosensor.PartitionCoefficient}");
            ResultPrinter.Print("");
        }
    }
}