using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes.Calculators1D;
using System;
using System.Diagnostics;
using System.Linq;

namespace BiosensorSimulator.Simulations.Simulations1D
{
    public abstract class BaseSimulation1D : BaseSimulation
    {
        public ISchemeCalculator1D SchemeCalculator { get; set; }

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;

        private double? _currentFactor;

        public double CurrentFactor
        {
            get
            {
                if (_currentFactor.HasValue)
                    return _currentFactor.Value;

                var firstLayer = Biosensor.Layers.First();
                _currentFactor = SimulationParameters.ne * SimulationParameters.F
                    * firstLayer.Product.DiffusionCoefficient / firstLayer.H;
                return _currentFactor.Value;
            }
        }

        protected BaseSimulation1D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            IResultPrinter resultPrinter)
            : base(simulationParameters, biosensor, resultPrinter) { }

        // Calculate next step of biosensor
        public override void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);

            SchemeCalculator.CalculateNextStep(SCur, PCur, SPrev, PPrev);
            CalculateMatchingConditions();
            CalculateBoundaryConditions();
        }

        /// <summary>
        /// <summary>
        /// Simulation stable current 
        /// </summary>
        public override void RunStableCurrentSimulation(int maxTime = int.MaxValue)
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

        public override double GetCurrent()
        {
            return PCur[1] * CurrentFactor;
        }

        /// <summary>
        /// Set initial biosensor conditions
        /// </summary>
        public override void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N];
            PCur = new double[SimulationParameters.N];
            SPrev = new double[SimulationParameters.N];
            PPrev = new double[SimulationParameters.N];

            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            PCur[SimulationParameters.N - 1] = Biosensor.P0;
        }

        public override void PrintSimulationConcentrations(bool normalize = false)
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
    }
}