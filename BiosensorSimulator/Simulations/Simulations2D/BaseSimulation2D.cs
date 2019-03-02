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
                _currentFactor = SimulationParameters.ne * SimulationParameters.F * firstLayer.Product.DiffusionCoefficient * (2 / (firstLayer.Width * firstLayer.Width));
                ResultPrinter.Print("H" + firstLayer.H.ToString());
                ResultPrinter.Print("W" + firstLayer.W.ToString());
                ResultPrinter.Print(_currentFactor.Value.ToString());

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
        }

        /// <summary>
        /// Simulation stable current 
        /// </summary>
        public override void RunStableCurrentSimulation(int maxTime = int.MaxValue)
        {
            double iCur;
            var i = 1;
            double iPrev = 0;
            var firstLayer = Biosensor.Layers.First();

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

                double sum = 0;
                double sum2 = 0;
                double sum3 = 0;

                for (int j = 0; j < SCur.GetLength(1) - 1; j++)
                {
                    sum += (PCur[1, j] + PCur[1, j + 1]) / (2 * firstLayer.H) * (j * firstLayer.W) * (firstLayer.W);
                    //sum += (PCur[1, j] + PCur[1, j + 1]) / (2 * firstLayer.H) * ((j * firstLayer.W + (j + 1) * firstLayer.W) / 2) * (firstLayer.W);
                    //sum2 += (PCur[1, j]) / (firstLayer.H) * firstLayer.W * j;
                    // sum3 += (PCur[1, j]) / (firstLayer.H) * firstLayer.W;
                    //sum += (PCur[1, j] + PCur[1, j + 1]) / (2 * firstLayer.H) * ((j * firstLayer.W + (j + 1) * firstLayer.W) / 2);
                }

                iCur = sum * CurrentFactor;

                //iCur = GetCurrent();

                if (iCur > 0 && iPrev > 0
                             && iCur > SimulationParameters.ZeroIBond
                             && Math.Abs(iCur - iPrev) * i / iCur < SimulationParameters.DecayRate)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < SCur.GetLength(1); l++)
                        {
                            ResultPrinter.Print(PCur[k, l].ToString());
                        }
                        ResultPrinter.Print("");
                    }
                    break;
                }


                if (i % resultSteps == 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < SCur.GetLength(1); l++)
                        {
                            ResultPrinter.Print(PCur[k, l].ToString());
                        }
                        ResultPrinter.Print("");
                    }

                    ResultPrinter.Print("Resultssss");
                    PrintSimulationResultsSimple(iCur);
                    ResultPrinter.Print(sum.ToString());
                    //ResultPrinter.Print(sum3.ToString());
                    ResultPrinter.Print("");
                }

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

            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                sum += PCur[1, j];
            }

            return sum * CurrentFactor;
        }

        /// <summary>
        /// Set initial biosensor conditions
        /// </summary>
        public override void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N, SimulationParameters.M];
            PCur = new double[SimulationParameters.N, SimulationParameters.M];
            SPrev = new double[SimulationParameters.N, SimulationParameters.M];
            PPrev = new double[SimulationParameters.N, SimulationParameters.M];

            for (int j = 0; j < SCur.GetLength(1); j++)
            {
                SCur[SimulationParameters.N - 1, j] = Biosensor.S0;
            }
        }

        public override void PrintSimulationConcentrations(bool normalize = false)
        {
            throw new NotImplementedException();
        }
    }
}