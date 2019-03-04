using BiosensorSimulator.Parameters.Biosensors.AnalyticalBiosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes.Calculators1D;
using BiosensorSimulator.Simulations.Simulations1D;
using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Schemes;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            var biosensor = new SingleLayerAnalyticalBiosensor();
            var simulationParameters = new SimulationParametersSupplier2(biosensor);

            var resultPrinter = new ConsolePrinter();
            //var resultPrinter = new FilePrinter($@"C:\BiosensorSimulations\{biosensor.Name}");

            BaseSimulation1D simulation = new Simulation1D(simulationParameters, biosensor, resultPrinter);

            simulation.PrintParameters();
            simulation.ShowValidationValues();

            var isSimulation2d = false;
            new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, isSimulation2d);

            /*if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                biosensor.Homogenize();*/

            simulation.SchemeCalculator1D = new ExplicitSchemeCalculator1D(biosensor, simulationParameters);

            if (simulation.SchemeCalculator1D is ImplicitSchemeCalculator1D)
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