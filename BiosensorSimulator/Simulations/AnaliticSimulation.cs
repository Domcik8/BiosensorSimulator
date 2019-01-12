using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Simulations
{
    public class AnaliticSimulation
    {
        //ToDo: kodel neatitinka reiksmiu knygoje?
        public double GetFirstOrderAnaliticSolution(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            biosensorParameters.S0 = 0.01 * biosensorParameters.Km;

            double alpha = Math.Sqrt(biosensorParameters.Vmax / (biosensorParameters.Km * biosensorParameters.DSf));

            double iCur = simulationParameters.ne * simulationParameters.F * biosensorParameters.DPf *
                          biosensorParameters.S0 / biosensorParameters.c *
                          (1 - 1 / Math.Cosh(alpha * biosensorParameters.c)) / 1e-6;

            return iCur;
        }

        public double GetZeroOrderAnaliticSolution(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            biosensorParameters.S0 = 1000 * biosensorParameters.Km;

            double iCur = simulationParameters.ne * simulationParameters.F * biosensorParameters.Vmax *
                          biosensorParameters.c / 2 / 1e-3;

            return iCur;
        }
    }
}
