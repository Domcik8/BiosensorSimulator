using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Simulations
{
    public class AnalyticSimulation
    {
        public double GetFirstOrderAnalyticSolution(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            if (biosensor.Layers.Count == 1)
                return GetFirstOrderAnalyticSolutionForSingleLayerModel(biosensor, simulationParameters);

            if (biosensor.Layers.Count == 2)
                return GetFirstOrderAnalyticSolutionForTwoLayerModel(biosensor, simulationParameters);

            return 0;
        }

        private static double GetFirstOrderAnalyticSolutionForSingleLayerModel(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            var S0 = 0.01 * biosensor.Km;

            var enzymeLayer = biosensor.EnzymeLayer;
            var alpha = Math.Sqrt(biosensor.VMax / (biosensor.Km * enzymeLayer.Substrate.DiffusionCoefficient));

            var iCur = simulationParameters.ne * simulationParameters.F * enzymeLayer.Product.DiffusionCoefficient *
                       S0 / enzymeLayer.Height * (1 - 1 / Math.Cosh(alpha * enzymeLayer.Height));

            return iCur;
        }

        private static double GetFirstOrderAnalyticSolutionForTwoLayerModel(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            var S0 = biosensor.S0;

            var enzymeLayer = biosensor.EnzymeLayer;
            var diffusionLayer = biosensor.DiffusionLayer;

            var alpha = Math.Sqrt(biosensor.VMax * enzymeLayer.Height * enzymeLayer.Height
                                  / (biosensor.Km * enzymeLayer.Substrate.DiffusionCoefficient));

            var firstMultiplier = simulationParameters.ne * simulationParameters.F *
                                  enzymeLayer.Product.DiffusionCoefficient *
                                  S0 / (enzymeLayer.Height + diffusionLayer.Height);

            var secondMultiplier = enzymeLayer.Height + diffusionLayer.Height *
                                   (diffusionLayer.Substrate.DiffusionCoefficient - alpha *
                                    enzymeLayer.Substrate.DiffusionCoefficient * Math.Sinh(alpha) / Math.Cosh(alpha)) /
                                   (diffusionLayer.Substrate.DiffusionCoefficient + alpha *
                                    (diffusionLayer.Height / enzymeLayer.Height) *
                                    enzymeLayer.Substrate.DiffusionCoefficient * Math.Sinh(alpha) / Math.Cosh(alpha));

            var thirdMultiplier =
                alpha * enzymeLayer.Substrate.DiffusionCoefficient * diffusionLayer.Height * Math.Sinh(alpha) /
                (enzymeLayer.Height * Math.Cosh(alpha)) + enzymeLayer.Substrate.DiffusionCoefficient *
                diffusionLayer.Product.DiffusionCoefficient /
                enzymeLayer.Product.DiffusionCoefficient *
                (1 - 1 / Math.Cosh(alpha));

            return firstMultiplier * secondMultiplier * thirdMultiplier / (diffusionLayer.Product.DiffusionCoefficient *
                                                                           enzymeLayer.Height + enzymeLayer.Product.DiffusionCoefficient * diffusionLayer.Height);
        }

        public double GetZeroOrderAnalyticSolution(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            if (biosensor.Layers.Count == 1)
                return GetZeroOrderAnalyticalSolutionForSingleLayerModel(biosensor, simulationParameters);

            if (biosensor.Layers.Count == 2)
                return GetZeroOrderAnalyticalSolutionForTwoLayerModel(biosensor, simulationParameters);

            return 0;
        }

        private static double GetZeroOrderAnalyticalSolutionForSingleLayerModel(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            var enzymeLayer = biosensor.EnzymeLayer;
            var iCur = simulationParameters.ne * simulationParameters.F * biosensor.VMax * enzymeLayer.Height / 2;

            return iCur;
        }

        public double GetZeroOrderAnalyticalSolutionForTwoLayerModel(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            var enzymeLayer = biosensor.EnzymeLayer;
            var diffusionLayer = biosensor.DiffusionLayer;
            var iCur = simulationParameters.ne * simulationParameters.F * biosensor.VMax * enzymeLayer.Height *
                       (diffusionLayer.Product.DiffusionCoefficient * diffusionLayer.Height +
                        2 * enzymeLayer.Product.DiffusionCoefficient * diffusionLayer.Height) /
                       (2 * (diffusionLayer.Product.DiffusionCoefficient * diffusionLayer.Height +
                             enzymeLayer.Product.DiffusionCoefficient * diffusionLayer.Height));

            return iCur;
        }
    }
}