using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.MicroreactorBiosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes;
using BiosensorSimulator.Schemes.Calculators1D;
using BiosensorSimulator.Simulations.Simulations1D;
using System;
using System.Collections.Generic;
using System.Linq;
using BiosensorSimulator.Schemes.Calculators2D;
using BiosensorSimulator.Simulations.Simulations2D;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            Console.BufferHeight = Int16.MaxValue - 1;
            int parameter;
            double y;
            List<double> values;

            ReadParameters(out parameter, out y, out values);

            var resultPrinter =
                new ConsoleFilePrinter(@"C:\BiosensorSimulations\Two-Layer-Microreactor-Biosensor");

            foreach (var value in values)
            {
                var biosensor = new TwoLayerMicroreactorBiosensor();
                EnterInputValues(parameter, y, value, biosensor);

                var simulationParameters = new SimulationParametersSupplier(biosensor);

                //1D
                //BaseSimulation1D simulation = new SimpleSimulation1D(simulationParameters, biosensor, resultPrinter);
                //simulation.PrintParameters();
                //simulation.ShowValidationValues();
                //new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, false);
                //if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                //    biosensor.Homogenize();
                //simulation.SchemeCalculator = new ExplicitSchemeCalculator1D(biosensor, simulationParameters);

                //2D
                BaseSimulation2D simulation = new MicroreactorSimulation2D(simulationParameters, biosensor, resultPrinter);
                simulation.PrintParameters();
                simulation.ShowValidationValues();
                var isSimulation2d = true;
                new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, isSimulation2d);
                simulation.SchemeCalculator = new ExplicitSchemeCalculator2D(biosensor, simulationParameters);

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

        private static void EnterInputValues(int parameter, double y, double value, TwoLayerMicroreactorBiosensor biosensor)
        {
            switch (parameter)
            {
                case 1:
                    biosensor.VMax = value;
                    break;
                case 2:
                    biosensor.S0 = value;
                    break;
                case 3:
                    {
                        biosensor.Height = biosensor.NonHomogenousLayer.H + value;
                        biosensor.DiffusionLayer.Height = value;
                        break;
                    }

                default:
                    throw new Exception("No papa");
            }

            biosensor.MicroReactorRadius = biosensor.UnitRadius * y;
        }

        private static void ReadParameters(out int parameter, out double y, out List<double> values)
        {
            Console.WriteLine("o = 1, s0 = 1, bi = 1");

            Console.WriteLine("Simulated parameter (1. Vmax, 2.S0, 3.Bi): ");
            parameter = int.Parse(Console.ReadLine());
            Console.WriteLine("Parameter values (separate parameters with ';')");
            var input = Console.ReadLine();
            var inputs = input.Split(';').ToList();

            var tempValues = new List<double>();
            inputs.ForEach(x => { tempValues.Add(double.Parse(x)); });
            values = tempValues;

            Console.WriteLine("Value of y: ");
            y = double.Parse(Console.ReadLine());
        }
    }
}