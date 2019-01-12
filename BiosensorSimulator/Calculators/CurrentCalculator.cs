using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Simulations;
using System;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Calculators
{
    public class CurrentCalculator
    {
        public BaseSimulation Simulation { get; }
        public double CurrentFactor { get; }

        public CurrentCalculator(BaseSimulation simulation)
        {
            Simulation = simulation;

            var enzymeLayer = simulation.BiosensorParameters.Layers.First(l => l.Type == LayerType.Enzyme);
            
            CurrentFactor = simulation.SimulationParameters.ne * simulation.SimulationParameters.F * 
                            enzymeLayer.Substances.First(s => s.Type == SubstanceType.Product).DiffusionCoefficient / enzymeLayer.H;
        }

        public double CalculateStableCurrent()
        {
            long i = 1;
            double iPrev = 0;

            while (true)
            {
                Simulation.CalculateNextStep();

                var iCur = Simulation.PCur[1] * CurrentFactor;

                if (iCur > 0 && iPrev > 0
                    && iCur > Simulation.SimulationParameters.ZeroIBond
                    && Math.Abs(iCur - iPrev) * i / iCur < Simulation.SimulationParameters.DecayRate
                    )
                    return iCur;

                iPrev = iCur;
                i++;
            }
        }
    }
}
