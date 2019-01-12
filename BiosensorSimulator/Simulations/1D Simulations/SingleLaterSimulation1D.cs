using BiosensorSimulator.Calculators;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;
using System.Diagnostics;

namespace BiosensorSimulator.Simulations._1D_Simulations
{
    public class SingleLaterSimulation1D : BaseSimulation
    {
        public SingleLaterSimulation1D(
            SimulationParameters simulationParameters,
            BiosensorParameters biosensorParameters,
            ISchemeCalculator schemeCalculator
            ) : base(simulationParameters, biosensorParameters, schemeCalculator) { }
        
        public override void RunStableCurrentSimulation()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            SetInitialConditions();
            SteadyCurrent = new CurrentCalculator(this).CalculateStableCurrent();

            stopWatch.Stop();
        }

        public void ShowValidationValues(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            var simulation = new AnaliticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnaliticSolution(biosensorParameters, simulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnaliticSolution(biosensorParameters, simulationParameters);

            Console.WriteLine($"First order current : {firstOrderCurrent} nA/mm^2");
            Console.WriteLine($"Zero order current : {zeroOrderCurrent} uA/mm^2");
        }

        public override void AssertSimulation()
        {
            new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, BiosensorParameters);
        }

        public override void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);

            foreach (var layer in BiosensorParameters.Layers)
            {
                SchemeCalculator.CalculateNextStep(layer, SCur, PCur, SPrev, PPrev);
            }
            
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
            new ExplicitSchemeStabilityChecker().AssertStability(SimulationParameters, BiosensorParameters);
        }

        public override void SetInitialConditions()
        {
            SCur = new double[SimulationParameters.N + 1];
            PCur = new double[SimulationParameters.N + 1];
            SPrev = new double[SimulationParameters.N + 1];
            PPrev = new double[SimulationParameters.N + 1];

            SCur[SimulationParameters.N] = BiosensorParameters.S0;
            PCur[SimulationParameters.N] = BiosensorParameters.P0;
        }
    }
}
