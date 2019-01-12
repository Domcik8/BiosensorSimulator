using System;
using System.Diagnostics;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.SchemeCalculator;

namespace BiosensorSimulator._1D_Simulations
{
    public class SingleLaterSimulation1D : ISimulation
    {
        public SimulationParameters SimulationParameters { get; }
        public BiosensorParameters BiosensorParameters { get; }
        public ISchemeCalculator SchemeCalculator { get; }
        public CurrentCalculator CurrentCalculator { get; }

        public double[] SCur, PCur;
        public double[] SPrev, PPrev;
        public double SteadyCurrent;

        public SingleLaterSimulation1D(
            SimulationParameters simulationParameters, BiosensorParameters biosensorParameters,
            ISchemeCalculator schemeCalculator, CurrentCalculator currentCalculator)
        {
            SimulationParameters = simulationParameters;
            BiosensorParameters = biosensorParameters;
            SchemeCalculator = schemeCalculator;
            CurrentCalculator = currentCalculator;
        }

        public void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N + 1];
            PCur = new double[SimulationParameters.N + 1];
            SPrev = new double[SimulationParameters.N + 1];
            PPrev = new double[SimulationParameters.N + 1];

            SCur[SimulationParameters.N] = BiosensorParameters.S0;
            PCur[SimulationParameters.N] = BiosensorParameters.P0;
        }

        public void RunSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            AssertSimulationStability(SimulationParameters, BiosensorParameters);
            SetInitialConditions();

            for (var i = 0; i < SimulationParameters.M; i++)
                CalculateNextStep();

            stopWatch.Stop();
        }

        public void RunStableCurrentSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            SteadyCurrent = CurrentCalculator.CalculateStableCurrent(SCur, PCur, SPrev, PPrev);
            
            stopWatch.Stop();
        }

        private void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);

            SchemeCalculator.CalculateDiffusionLayerNextStep(SCur, PCur, SPrev, PPrev);
            SetBondaryConditions();
        }

        public void SetBondaryConditions()
        {
            SCur[SimulationParameters.N] = BiosensorParameters.S0;
            SCur[0] = SCur[1];

            PCur[SimulationParameters.N] = BiosensorParameters.P0;
            PCur[0] = 0;
        }

        public void AssertSimulationStability(SimulationParameters simulationParameters, BiosensorParameters biosensorParameters)
        {
            new ImplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensorParameters);
        }
    }
}
