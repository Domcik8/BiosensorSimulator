using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;
using System.Linq;

namespace BiosensorSimulator.Simulations
{
    public class AnaliticSimulation
    {
        //ToDo: kodel neatitinka reiksmiu knygoje?
        public double GetFirstOrderAnaliticSolution(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            biosensorParameters.S0 = 0.01 * biosensorParameters.Km;

            var enzymeLayer = biosensorParameters.Layers.First(l => l.Type == LayerType.Enzyme);

            double alpha = Math.Sqrt(biosensorParameters.VMax / (biosensorParameters.Km * enzymeLayer.Substances.First(s => s.Type == SubstanceType.Substrate).DiffusionCoefficient));

            double iCur = simulationParameters.ne * simulationParameters.F * enzymeLayer.Substances.First(s => s.Type == SubstanceType.Product).DiffusionCoefficient *
                          biosensorParameters.S0 / enzymeLayer.Height *
                          (1 - 1 / Math.Cosh(alpha * enzymeLayer.Height));

            return iCur;
        }

        public double GetZeroOrderAnaliticSolution(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            var enzymeLayer = biosensorParameters.Layers.First(l => l.Type == LayerType.Enzyme);

            biosensorParameters.S0 = 1000 * biosensorParameters.Km;

            double iCur = simulationParameters.ne * simulationParameters.F * biosensorParameters.VMax *
                          enzymeLayer.Height / 2;

            return iCur;
        }
    }
}
