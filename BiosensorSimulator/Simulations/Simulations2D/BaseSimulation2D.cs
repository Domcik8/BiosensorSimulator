using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes.Calculators2D;
using System;
using System.Diagnostics;
using System.Linq;

namespace BiosensorSimulator.Simulations.Simulations2D
{
    public abstract class BaseSimulation2D : BaseSimulation
    {
        public ISchemeCalculator2D SchemeCalculator { get; set; }

        public double[,] SCur, PCur;
        public double[,] SPrev, PPrev;

        private double? _currentFactor;

        public double CurrentFactor
        {
            get
            {
                if (_currentFactor.HasValue)
                    return _currentFactor.Value;

                var firstLayer = Biosensor.Layers.First();
                _currentFactor = SimulationParameters.ne * SimulationParameters.F * 2
                    / (firstLayer.Width * firstLayer.Width);
                return _currentFactor.Value;
            }
        }

        protected BaseSimulation2D(
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
            CalculateNonLeakageConditions();
        }

        public abstract void CalculateNonLeakageConditions();

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
            double resultTime = GetResultTime();

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
            double sum = 0;
            var firstLayer = Biosensor.Layers.First();
            var spaceStepR = firstLayer.W / firstLayer.H * firstLayer.W;

            for (var j = 0; j < SCur.GetLength(1) - 1; j++)
                sum += PCur[1, j] * spaceStepR * (j + 1);

            return sum * CurrentFactor * firstLayer.Product.DiffusionCoefficient;
        }

        /// <summary>
        /// Set initial biosensor conditions
        /// </summary>
        public override void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N + 1, SimulationParameters.M + 1];
            PCur = new double[SimulationParameters.N + 1, SimulationParameters.M + 1];
            SPrev = new double[SimulationParameters.N + 1, SimulationParameters.M + 1];
            PPrev = new double[SimulationParameters.N + 1, SimulationParameters.M + 1];

            for (var j = 0; j < SCur.GetLength(1); j++)
                SCur[SimulationParameters.N, j] = Biosensor.S0;
        }

        public override void PrintSimulationConcentrations(bool normalize = false)
        {
            //throw new NotImplementedException();
        }
    }
}