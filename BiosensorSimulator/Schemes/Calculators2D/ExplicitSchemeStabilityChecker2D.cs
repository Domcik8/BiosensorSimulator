using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Schemes.Calculators2D
{
    public class ExplicitSchemeStabilityChecker2D
    {
        public void AssertStability(SimulationParameters simulationParameters, BaseBiosensor biosensor)
        {
            var isReactionStable = GetReactionStability(biosensor.VMax, biosensor.Km, simulationParameters.t);

            if (!isReactionStable)
            {
                throw new Exception("Simulation scheme is not stable");
            }

            foreach (var layer in biosensor.Layers)
            {
                if (layer.N == 0 || layer.H == 0 || layer.Type == LayerType.SelectiveMembrane)
                {
                    continue;
                }

                if (layer.Type == LayerType.DiffusionSmallLayer)
                {
                    continue;
                }

                var Dmax = Math.Max(layer.Substrate.DiffusionCoefficient, layer.Product.DiffusionCoefficient);
                var Wmin = Math.Min(layer.H * layer.H, layer.W * layer.W);
                var isLayerStable = GetDiffusionStability(Dmax, layer.H, simulationParameters.t, Wmin);

                if (!isLayerStable)
                {
                    throw new Exception("Simulation scheme is not stable");
                }
            }
        }

        public bool GetDiffusionStability(double D, double h, double t, double w)
        {
            return D * t / w <= 0.0833333333333333333;
        }

        public bool GetReactionStability(double Vmax, double Km, double t)
        {
            return t * Vmax / Km <= 0.5;
        }
    }
}