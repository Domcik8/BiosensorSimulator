using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;
using BiosensorSimulator.Calculators.SchemeCalculator;

namespace BiosensorSimulator.Calculators
{
    public class CurrentCalculator
    {
        public BiosensorParameters BiosensorParameters { get; }
        public SimulationParameters SimulationParameters { get; }
        public ISchemeCalculator SchemeCalculator { get; }
        public double CurrentFactor { get; }

        public CurrentCalculator(
            SimulationParameters simulationParameters, BiosensorParameters biosensorParameters,
            ISchemeCalculator schemeCalculator)
        {
            BiosensorParameters = biosensorParameters;
            SimulationParameters = simulationParameters;
            SchemeCalculator = schemeCalculator;

            CurrentFactor = simulationParameters.ne * simulationParameters.F * biosensorParameters.DPf / simulationParameters.hf;
        }

        public double CalculateStableCurrent(double[] sCur, double[] sPrev, double[] pCur, double[] pPrev)
        {
            int i = 1;
            double iPrev = 0;

            while (true)
            {
                SchemeCalculator.CalculateNextStep(
                    sCur, pCur, sPrev, pPrev,
                    BiosensorParameters, SimulationParameters
                    );

                var iCur = pCur[1] * CurrentFactor;

                if (iCur > 0 && iPrev > 0 && iCur > SimulationParameters.ZeroIBond && Math.Abs(iCur - iPrev) * i / iCur < SimulationParameters.DecayRate)
                    return iCur;

                iPrev = iCur;
                i++;
            }
        }
    }
}
