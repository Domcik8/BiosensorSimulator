using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.MicroreactorBiosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes;
using BiosensorSimulator.Schemes.Calculators1D;
using BiosensorSimulator.Simulations.Simulations1D;
using System;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            Console.BufferHeight = Int16.MaxValue - 1;

            var vm = new[] { 3e-8, 3e-9, 3e-10, 3e-11, 3e-12 };

            var resultPrinter =
                new ConsoleFilePrinter($@"C:\BiosensorSimulations\Two-Layer-Microreactor-Biosensor");
            //var resultPrinter = new ConsolePrinter();
            //var resultPrinter = new FilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");



            foreach (double Vmax in vm)
            {
                var biosensor = new TwoLayerMicroreactorBiosensor();
                biosensor.VMax = Vmax;

                var simulationParameters = new SimulationParametersSupplier(biosensor);

                //var resultPrinter = new ConsoleFilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");
                //var resultPrinter = new ConsolePrinter();
                //var resultPrinter = new FilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");

                //1D
                BaseSimulation1D simulation = new SimpleSimulation1D(simulationParameters, biosensor, resultPrinter);
                simulation.PrintParameters();
                simulation.ShowValidationValues();
                new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, false);
                if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                    biosensor.Homogenize();
                simulation.SchemeCalculator = new ExplicitSchemeCalculator1D(biosensor, simulationParameters);


                //2D
                //BaseSimulation2D simulation = new MicroreactorSimulation2D(simulationParameters, biosensor, resultPrinter);
                //simulation.PrintParameters();
                //simulation.ShowValidationValues();
                //var isSimulation2d = true;
                //new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, isSimulation2d);
                //simulation.SchemeCalculator = new ExplicitSchemeCalculator2D(biosensor, simulationParameters);


                if (simulation.SchemeCalculator is ImplicitSchemeCalculator1D)
                    resultPrinter.Print("====Implicit Scheme Calculator====");
                else
                    resultPrinter.Print("====Explicit Scheme Calculator====");

                resultPrinter.Print("====Results====");
                simulation.RunStableCurrentSimulation();
            }

            if (!(resultPrinter is FilePrinter))
            {
                resultPrinter.CloseStream();
                Console.WriteLine("Simulations ended, please copy console data.");

                for (var j = 0; j < 15; j++)
                    Console.ReadKey();
            }
            else
                resultPrinter.CloseStream();
        }
    }
}