using System;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ExplicitSchemeStabilityChecker
    {
        public void AssertStability(SimulationParameters simulationParameters, BiosensorParameters biosensorParameters)
        {
            var isReactionStable = GetReactionStability(
                biosensorParameters.VMax, biosensorParameters.Km, simulationParameters.t);


            foreach (var layer in biosensorParameters.Layers)
            {
                var Dmax = Math.Max(layer.Substances.First(s => s.Type == SubstanceType.Substrate).DiffusionCoefficient, 
                    layer.Substances.First(s => s.Type == SubstanceType.Product).DiffusionCoefficient);

                var isLayerStable = GetDiffusionStability(
                    Dmax, layer.Height / layer.N, simulationParameters.t);

                if (!isLayerStable)
                {
                    throw new Exception("Simulation scheme is not stable");
                }
            }

            //var Dmax = Math.Max(biosensorParameters.DSf, biosensorParameters.DPf);
            //var isDiffusionStableInFermentLayer = GetDiffusionStability(
            //    Dmax, biosensorParameters.c / simulationParameters.Nf, simulationParameters.t);

            //Dmax = Math.Max(biosensorParameters.DSd, biosensorParameters.DPd);
            //bool isDiffusionStableInDiffusionLayer = GetDiffusionStability(
            //    Dmax, biosensorParameters.n / simulationParameters.Nd, simulationParameters.t);

            if (isReactionStable)
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
