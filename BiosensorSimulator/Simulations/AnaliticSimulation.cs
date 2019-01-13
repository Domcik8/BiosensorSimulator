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

            var enzymeLayer = biosensorParameters.EnzymeLayer;
            var alpha = Math.Sqrt(biosensorParameters.VMax / (biosensorParameters.Km * enzymeLayer.Substrate.DiffusionCoefficient));

            var iCur = simulationParameters.ne * simulationParameters.F * enzymeLayer.Product.DiffusionCoefficient *
                          biosensorParameters.S0 / enzymeLayer.Height * (1 - 1 / Math.Cosh(alpha * enzymeLayer.Height));

            return iCur;
        }

        public double GetZeroOrderAnaliticSolution(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            biosensorParameters.S0 = 1000 * biosensorParameters.Km;

            var enzymeLayer = biosensorParameters.EnzymeLayer;
            var iCur = simulationParameters.ne * simulationParameters.F * biosensorParameters.VMax * enzymeLayer.Height / 2;

            return iCur;
        }
    }
}