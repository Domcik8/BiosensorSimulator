using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Simulations;
using System;

namespace BiosensorSimulator.Calculators
{
    public class CurrentCalculator
    {
        public BaseSimulation Simulation { get; }
        public double CurrentFactor { get; }

        public CurrentCalculator(BaseSimulation simulation)
        {
            Simulation = simulation;

            CurrentFactor = simulation.SimulationParameters.ne * simulation.SimulationParameters.F *
                simulation.BiosensorParameters.DPf / simulation.SimulationParameters.hf;
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
