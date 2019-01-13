using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Simulations
{
    public class Simulation1 : ISimulationParametersSuplier
    {
        public SimulationParameters InitiationParameters(Biosensor biosensor)
        {
            var simulationParameters = new SimulationParameters()
            {
                ne = 2,
                DecayRate = 1e-5,
                F = 96485.3329,
                ZeroIBond = 1e-25,
                Nd = 100,
                Nf = 100,
                N = 100,
                t = 7.5e-12
            };
            
            //simulationParameters.hf = biosensor.c / simulationParameters.Nf;
            //simulationParameters.hd = biosensor.n / simulationParameters.Nd;

            foreach (var layer in biosensor.Layers)
            {
                simulationParameters.hf = layer.Height / simulationParameters.Nf;
                layer.N = simulationParameters.Nf;
                layer.H = simulationParameters.hf;

                layer.R = simulationParameters.t / (layer.H * layer.H);

                layer.Product.DiffusionCoefficientOverR = GetSubstanceDiffusionCoefficientOverR(layer.Product, layer.R);
                layer.Substrate.DiffusionCoefficientOverR = GetSubstanceDiffusionCoefficientOverR(layer.Substrate, layer.R);

                layer.Product.DiffusionCoefficientOverSpace = GetSubstanceDiffusionCoefficientOverSpace(layer.Product, layer.H);
                layer.Substrate.DiffusionCoefficientOverSpace = GetSubstanceDiffusionCoefficientOverSpace(layer.Substrate, layer.H);
            }

            return simulationParameters;
        }

        private static double GetSubstanceDiffusionCoefficientOverR(Substance substance, double R)
        {
            return substance.DiffusionCoefficient * R;
        }

        private static double GetSubstanceDiffusionCoefficientOverSpace(Substance substance, double H)
        {
            return substance.DiffusionCoefficient / (H * H);
        }
    }
}