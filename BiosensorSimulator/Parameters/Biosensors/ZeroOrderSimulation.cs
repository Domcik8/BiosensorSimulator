using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class ZeroOrderSimulation : IBiosensorParametersSuplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensorParameters = new Biosensor
            {
                P0 = 0,
                VMax = 100e-6,
                Km = 100e-6,
            };

            biosensorParameters.S0 = 1000 * biosensorParameters.Km;
            biosensorParameters.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 0.01e-3,
                    Substrate =
                    new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 300e-6,
                        StartConcentration = biosensorParameters.S0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 300e-6,
                        StartConcentration = biosensorParameters.P0,
                        ReactionRate = 1
                    }
                }
            };

            return biosensorParameters;
        }
    }
}