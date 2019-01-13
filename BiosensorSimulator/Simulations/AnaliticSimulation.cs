using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Simulations
{
    public class AnaliticSimulation
    {
        //ToDo: kodel neatitinka reiksmiu knygoje?
        public double GetFirstOrderAnaliticSolution(Biosensor biosensor, SimulationParameters simulationParameters)
        {
            biosensor.S0 = 0.01 * biosensor.Km;

            var enzymeLayer = biosensor.EnzymeLayer;
            var alpha = Math.Sqrt(biosensor.VMax / (biosensor.Km * enzymeLayer.Substrate.DiffusionCoefficient));

            var iCur = simulationParameters.ne * simulationParameters.F * enzymeLayer.Product.DiffusionCoefficient *
                          biosensor.S0 / enzymeLayer.Height * (1 - 1 / Math.Cosh(alpha * enzymeLayer.Height));

            return iCur;
        }

        public double GetZeroOrderAnaliticSolution(Biosensor biosensor, SimulationParameters simulationParameters)
        {
            biosensor.S0 = 1000 * biosensor.Km;

            var enzymeLayer = biosensor.EnzymeLayer;
            var iCur = simulationParameters.ne * simulationParameters.F * biosensor.VMax * enzymeLayer.Height / 2;

            return iCur;
        }
    }
}