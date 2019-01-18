using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ExplicitSchemeStabilityChecker
    {
        public void AssertStability(SimulationParameters simulationParameters, Biosensor biosensor)
        {
            var isReactionStable = GetReactionStability(biosensor.VMax, biosensor.Km, simulationParameters.t);

            if (!isReactionStable)
            {
                throw new Exception("Simulation scheme is not stable");
            }

            foreach (var layer in biosensor.Layers)
            {
                var Dmax = Math.Max(layer.Substrate.DiffusionCoefficient, layer.Product.DiffusionCoefficient);
                var isLayerStable = layer.N == 0 || GetDiffusionStability(Dmax, layer.H, simulationParameters.t);

                if (!isLayerStable)
                {
                    throw new Exception("Simulation scheme is not stable");
                }
            }
        }

        public bool GetDiffusionStability(double D, double h, double t)
        {
            return D * t / (h * h) <= 0.25;
        }

        public bool GetReactionStability(double Vmax, double Km, double t)
        {
            return t * Vmax / Km <= 0.5;
        }
    }
}