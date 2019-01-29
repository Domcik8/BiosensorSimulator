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
            double j = 0.1;
            double k = 0.1;

            for (int i = 0; i < 10; i++)
            {
                // You can choose different starting conditions
                var biosensor = new TwoLayerPerforatedMembraneBiosensor();
                biosensor.PartitionCoefficient = k;
                var simulationParameters = new SimulationParametersSuplier1(biosensor);

                //var resultPrinter = new ConsolePrinter();
                var resultPrinter = new FilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");

                BaseSimulation simulation = new PerforatedMembrane1D(simulationParameters, biosensor, resultPrinter);

                simulation.ShowValidationValues();

                if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                    biosensor.Homogenize();

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

                k += j;
            }
        }
    }
}