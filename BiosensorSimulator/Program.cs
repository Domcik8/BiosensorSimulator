using System;
using System.Diagnostics;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.SchemeCalculator;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // You can choose different starting conditions
            var biosensorParameters = new FirstOrderSimulation().GetInitiationParameters();
            var simulationParameters = new Simulation1().InitiationParameters(biosensorParameters);
            var schemeCalculator = new ImplicitSchemeCalculator(biosensorParameters, simulationParameters);

            var currentCalculator = new CurrentCalculator(simulationParameters, biosensorParameters, schemeCalculator);

            ISimulation simulation = new Simulation1D(simulationParameters, biosensorParameters, schemeCalculator, currentCalculator);

            //Analitic model validation

            
            
            //Start simulation
            simulation.RunStableCurrentSimulation();

            simulation.ShowValidationValues(biosensorParameters, simulationParameters);

            //PrintResults(stopWatch, simulation1D.SCur, simulation1D.PCur, simulation1D.SteadyCurrent);
            

            

            Console.ReadKey();

            /*Tridiagonal solution example
            Matrix matrix = new Matrix();
            var a = new double[] { 0, -1, -1};
            var b = new double[] { 3, 3, 3 };
            var c = new double[] { -1, -1, 0 };
            var r = new double[] {-1, 7, 7};
           
            matrix.SolveTridiagonalInPlace(a, b, c, r, b.Length);*/
        }

        public static void PrintResults(Stopwatch stopwatch, double[] sCur, double[] pCur, double I)
        {
            Console.WriteLine($"Simulation took {stopwatch.ElapsedMilliseconds} seconds");


            Console.WriteLine($"Steady current = {I}");
            for (int i = 0; i < sCur.Length; i++)
                Console.WriteLine($"S[{i}] = {sCur[i]}, P[{i}] = {pCur[i]}");
        }
    }
}
