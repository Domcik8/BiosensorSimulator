using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.MicroreactorBiosensors;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;
using BiosensorSimulator.Schemes;
using BiosensorSimulator.Schemes.Calculators1D;
using BiosensorSimulator.Schemes.Calculators2D;
using BiosensorSimulator.Simulations.Simulations1D;
using BiosensorSimulator.Simulations.Simulations2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiosensorSimulator
{
    class Program
    {
        static void Main()
        {
            Console.BufferHeight = Int16.MaxValue - 1;
            int dimension, parameter;
            double y;
            List<double> values;

            var resultPrinter =
                new ConsoleFilePrinter(@"C:\BiosensorSimulations\Two-Layer-Microreactor-Biosensor");

            ReadParameters(resultPrinter, out dimension, out parameter, out y, out values);

            foreach (var value in values)
            {
                var biosensor = new TwoLayerMicroreactorBiosensor();

                SetY(y, biosensor);
                EnterInputValues(parameter, y, value, biosensor);
                var simulationParameters = new SimulationParametersSupplier(biosensor);


                //1D
                if (dimension == 1)
                {
                    BaseSimulation1D simulation = new SimpleSimulation1D(simulationParameters, biosensor, resultPrinter);
                    simulation.PrintParameters();
                    new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, false);
                    if (biosensor is BaseHomogenousBiosensor homogenousBiosensor && homogenousBiosensor.IsHomogenized)
                        biosensor.Homogenize();
                    simulation.SchemeCalculator = new ExplicitSchemeCalculator1D(biosensor, simulationParameters);

                    resultPrinter.Print("====Results====");
                    simulation.RunStableCurrentSimulation();
                }
                else if (dimension == 2)
                {
                    BaseSimulation2D simulation =
                        new MicroreactorSimulation2D(simulationParameters, biosensor, resultPrinter);
                    simulation.PrintParameters();
                    new ExplicitSchemeStabilityChecker().AssertStability(simulationParameters, biosensor, true);
                    simulation.SchemeCalculator = new ExplicitSchemeCalculator2D(biosensor, simulationParameters);

                    resultPrinter.Print("====Results====");
                    simulation.RunStableCurrentSimulation();
                }
                else throw new Exception("no papa");
            }

            if (!(resultPrinter is FilePrinter))
            {
                resultPrinter.Print("Simulations ended, please copy console data.");
                resultPrinter.CloseStream();

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
                    biosensor.VMax *= value;
                    break;
                case 2:
                    biosensor.S0 *= value;
                    break;
                case 3:
                    {
                        biosensor.DiffusionLayer.Height /= value;
                        biosensor.Height = biosensor.Layers.First().Height + value;
                        break;
                    }

                default:
                    throw new Exception("No papa");
            }
        }

        private static void SetY(double y, TwoLayerMicroreactorBiosensor biosensor)
        {
            biosensor.MicroReactorRadius = biosensor.UnitRadius * y;
            var subAreas = ((LayerWithSubAreas)biosensor.Layers.First()).SubAreas;
            subAreas.First().Width = biosensor.MicroReactorRadius;
            subAreas.Last().Width = biosensor.UnitRadius - biosensor.MicroReactorRadius;

            if (biosensor is BaseMicroreactorBiosensor microreactorBiosensor)
            {
                microreactorBiosensor.EffectiveSubstrateDiffusionCoefficient
                    = microreactorBiosensor.GetEffectiveDiffusionCoefficent(
                        microreactorBiosensor.NonHomogenousLayer.Substrate.DiffusionCoefficient,
                        microreactorBiosensor.DiffusionLayer.Substrate.DiffusionCoefficient);

                microreactorBiosensor.EffectiveProductDiffusionCoefficient
                    = microreactorBiosensor.GetEffectiveDiffusionCoefficent(
                        microreactorBiosensor.NonHomogenousLayer.Product.DiffusionCoefficient,
                        microreactorBiosensor.DiffusionLayer.Product.DiffusionCoefficient);
            }

            // Homogenization might change Deff, we need to adjust Vmax and d-c
            var c = biosensor.Layers.First().Height;

            biosensor.VMax = biosensor.EffectiveSubstrateDiffusionCoefficient * biosensor.Km / (c * c);
            
            biosensor.DiffusionLayer.Height = c * biosensor.DiffusionLayer.Substrate.DiffusionCoefficient
                                              / biosensor.EffectiveSubstrateDiffusionCoefficient;

            biosensor.VMax *= 1;
            biosensor.S0 *= 1;
            biosensor.DiffusionLayer.Height *= 1; //Anti
        }

        private static void ReadParameters(IResultPrinter resultPrinter, out int dimension, out int parameter, out double y, out List<double> values)
        {
            resultPrinter.Print("Simulation dimension (1. 1D, 2. 2D): ");
            dimension = int.Parse(Console.ReadLine());

            resultPrinter.Print("Simulated parameter (1. O2, 2.S0, 3.Bi): ");
            parameter = int.Parse(Console.ReadLine());
            resultPrinter.Print("Parameter values (separate parameters with ';', use 0 to simulate 1e-2;1e-1;1;1e1;1e2)");
            var input = Console.ReadLine();
            if (input == "0")
                input = parameter == 3 ? "1e-1;5e-1;1;5;1e1" : "1e-2;1e-1;1;1e1;1e2";

            var inputs = input.Split(';').ToList();

            var tempValues = new List<double>();
            inputs.ForEach(x => { tempValues.Add(double.Parse(x)); });
            values = tempValues;

            resultPrinter.Print("Value of y (1. 1; 2. 0,8; 3. 0,6; 4. 0,4; 5. 0,2): ");
            var tempy = int.Parse(Console.ReadLine());

            switch (tempy)
            {
                case 1:
                    y = 1;
                    break;

                case 2:
                    y = 0.8;
                    break;

                case 3:
                    y = 0.6;
                    break;

                case 4:
                    y = 0.4;
                    break;

                case 5:
                    y = 0.2;
                    break;

                default:
                    throw new Exception("no papa");
            }
        }
    }
}