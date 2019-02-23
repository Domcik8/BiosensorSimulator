using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Simulations;
using BiosensorSimulator.Simulations.Simulations1D;
using System;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            for (int i = 0; i < 4; i++)
            {
                // You can choose different starting conditions
                var biosensor = new TwoLayerPerforatedMembraneBiosensor();
                var simulationParameters = new SimulationParametersSuplier1(biosensor);

                if (i == 0)
                {
                    biosensor.HoleRadius = 0.1e-3;
                    biosensor.HalfDistanceBetweenHoles = 2e-3;
                    biosensor.EnzymeHoleHeight = 5e-3;
                }

                if (i == 1)
                {
                    biosensor.HoleRadius = 0.1e-3;
                    biosensor.HalfDistanceBetweenHoles = 4e-3;
                    biosensor.EnzymeHoleHeight = 5e-3;
                }

                if (i == 2)
                {
                    biosensor.HoleRadius = 0.1e-3;
                    biosensor.HalfDistanceBetweenHoles = 1e-3;
                    biosensor.EnzymeHoleHeight = 0;
                }

                if (i == 3)
                {
                    biosensor.HoleRadius = 0.1e-3;
                    biosensor.HalfDistanceBetweenHoles = 1e-3;
                    biosensor.EnzymeHoleHeight = 10e-3;
                }

                //var resultPrinter = new ConsolePrinter();
                var resultPrinter = new FilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");

                BaseSimulation simulation = new PerforatedMembrane1D(simulationParameters, biosensor, resultPrinter);

                //simulation.ShowValidationValues();

                if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                {
                    biosensor.Homogenize();
                }

                simulation.SchemeCalculator = new ExplicitSchemeCalculator(biosensor, simulationParameters);

                if (simulation.SchemeCalculator is ImplicitSchemeCalculator)
                    resultPrinter.Print("====Implicit Scheme Calculator====");
                else
                {
                    resultPrinter.Print("====Explicit Scheme Calculator====");
                    new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor);
                }

                simulation.PrintParameters();
                resultPrinter.Print("====Results====");
                simulation.RunStableCurrentSimulation();

                //TwoLayer
                //simulation.RunSimulation(124, new []{6.8, 8.4, 16, 18.3, 27.8}, true);

                //First Order
                //simulation.RunSimulation(24.5, new[] { 0.5, 1, 3 });

                if (resultPrinter is ConsolePrinter)
                {
                    Console.ReadKey();
                    Console.ReadKey();
                    Console.ReadKey();
                }
                else
                    resultPrinter.CloseStream();
            }
        }
    }
}