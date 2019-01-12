using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.SchemeCalculator
{
    public class ImplicitSchemeStabilityChecker
    {
        public void AssertStability(SimulationParameters simulationParameters, BiosensorParameters biosensorParameters)
        {
            bool reactionStability = GetReactionStability(
                biosensorParameters.Vmax, biosensorParameters.Km, simulationParameters.t);

            double Dmax = Math.Max(biosensorParameters.DSf, biosensorParameters.DPf);
            bool reactionDiffusionLayerStability = GetDiffusionStability(
                Dmax, biosensorParameters.c / simulationParameters.Nf, simulationParameters.t);

            Dmax = Math.Max(biosensorParameters.DSd, biosensorParameters.DPd);
            bool diffusionLayerStability = GetDiffusionStability(
                Dmax, biosensorParameters.n / simulationParameters.Nd, simulationParameters.t);

            if (reactionStability && reactionDiffusionLayerStability && diffusionLayerStability)
                return;

            throw new Exception("Simulation scheme is not stable");
        }
        
        private bool GetDiffusionStability(double D, double h, double t)
        {
            return D * t / (h * h) <= 0.25;
        }

        private bool GetReactionStability(double Vmax, double Km, double t)
        {
            return t * Vmax / Km <= 0.5;
        }
    }
}
