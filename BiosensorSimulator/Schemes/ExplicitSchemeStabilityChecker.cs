using System;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Schemes
{
    public class ExplicitSchemeStabilityChecker
    {
        public void AssertStability(SimulationParameters simulationParameters, BaseBiosensor biosensor, bool is2D)
        {
            var isReactionStable = GetReactionStability(biosensor.VMax, biosensor.Km, simulationParameters.t);

            if (!isReactionStable)
                throw new Exception("Simulation scheme is not stable");

            foreach (var layer in biosensor.Layers)
            {
                if (layer.Type == LayerType.SelectiveMembrane
                    || layer.Type == LayerType.DiffusionSmallLayer
                    || layer.Type == LayerType.NonHomogenousLayer)
                    continue;

                if (layer.N == 0)
                    continue;

                var Dmax = Math.Max(layer.Substrate.DiffusionCoefficient, layer.Product.DiffusionCoefficient);
                if (!GetDiffusionStability(Dmax, layer.H, simulationParameters.t))
                    throw new Exception("Simulation scheme is not stable");

                if (!is2D) continue;

                if (layer.W == 0)
                    continue;

                if (!GetDiffusionStability(Dmax, layer.W, simulationParameters.t))
                    throw new Exception("Simulation scheme is not stable");
            }
        }

        public bool GetDiffusionStability(double D, double h, double t)
        {
            // Why we tried to change 0.25 to 0.083333333333333?
            return D * t / (h * h) <= 0.25;
        }

        public bool GetReactionStability(double Vmax, double Km, double t)
        {
            return t * Vmax / Km <= 0.5;
        }
    }
}