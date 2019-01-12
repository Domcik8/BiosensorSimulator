using System;
using System.Diagnostics;
using BiosensorSimulator.Calculators;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Simulations._1D_Simulations
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

        public void ShowValidationValues()
        {
            throw new NotImplementedException();
        }

        public void AssertSimulation()
        {
            throw new NotImplementedException();
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
        
        public static void PrintSimulationResults(Stopwatch stopwatch, double I)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");
            Console.WriteLine($"Steady current = {I}");
        }

        public static void PrintSimulationResults(Stopwatch stopwatch, double[] sCur, double[] pCur)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");
            
            for (int i = 0; i < sCur.Length; i++)
                Console.WriteLine($"S[{i}] = {sCur[i]}, P[{i}] = {pCur[i]}");
        }

        public static void PrintSimulationResults(Stopwatch stopwatch, double[] sCur, double[] pCur, double I)
        {
            Console.WriteLine($"Simulation lasted {stopwatch.ElapsedMilliseconds} miliseconds");

            Console.WriteLine($"Steady current = {I}");
            for (int i = 0; i < sCur.Length; i++)
                Console.WriteLine($"S[{i}] = {sCur[i]}, P[{i}] = {pCur[i]}");
        }

        public void AssertSimulationStability()
        {
            new ImplicitSchemeStabilityChecker().AssertStability(SimulationParameters, BiosensorParameters);
        }
    }
}
