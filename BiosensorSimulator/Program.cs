using BiosensorSimulator.Parameters.Biosensors.AnalyticalBiosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes.Calculators1D;
using BiosensorSimulator.Simulations.Simulations1D;
using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Schemes;
using BiosensorSimulator.Schemes.Calculators2D;
using BiosensorSimulator.Simulations.Simulations2D;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            var biosensor = new TwoLayerMicroreactorBiosensor();
            var simulationParameters = new SimulationParametersSupplier(biosensor);

            var resultPrinter = new ConsoleFilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");
            //var resultPrinter = new ConsolePrinter();
            //var resultPrinter = new FilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");

            BaseSimulation2D simulation = new MicroreactorSimulation2D(simulationParameters, biosensor, resultPrinter);

            simulation.PrintParameters();
            simulation.ShowValidationValues();

            var isSimulation2d = true;
            new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, isSimulation2d);

            /*if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                biosensor.Homogenize();*/

            simulation.SchemeCalculator = new ExplicitSchemeCalculator2D(biosensor, simulationParameters);

            if (simulation.SchemeCalculator is ImplicitSchemeCalculator1D)
                resultPrinter.Print("====Implicit Scheme Calculator====");
            else
                resultPrinter.Print("====Explicit Scheme Calculator====");

            resultPrinter.Print("====Results====");
            simulation.RunStableCurrentSimulation();

            //simulation.RunSimulation(30);

            //TwoLayer
            //simulation.RunSimulation(124, new []{6.8, 8.4, 16, 18.3, 27.8}, true);

            //First Order
            //simulation.RunSimulation(24.5, new[] { 0.5, 1, 3 });

            if (!(resultPrinter is FilePrinter))
            {
                for(var i = 0; i < 10; i++)
                    Console.ReadKey();
            }
            else
                resultPrinter.CloseStream();
        }
    }
}