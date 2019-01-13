using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Scheme;

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

                layer.Product.ExplicitScheme = GetExplicitSchemeParameters(layer, layer.Product);
                layer.Substrate.ExplicitScheme = GetExplicitSchemeParameters(layer, layer.Substrate);

                layer.Product.ImplicitScheme = GetImplicitSchemeParameters(layer, layer.Product);
                layer.Substrate.ImplicitScheme = GetImplicitSchemeParameters(layer, layer.Substrate);
            }

            return simulationParameters;
        }

        private static ExplicitSchemeParameters GetExplicitSchemeParameters(Layer layer, Substance substance)
        {
            return new ExplicitSchemeParameters
            {
                DiffusionCoefficientOverR = substance.DiffusionCoefficient * layer.R,
                DiffusionCoefficientOverSpace = substance.DiffusionCoefficient / (layer.H * layer.H)
            };
        }

        private static ImplicitSchemeParameters GetImplicitSchemeParameters(Layer layer, Substance substance)
        {
            return new ImplicitSchemeParameters();
        }
    }
}