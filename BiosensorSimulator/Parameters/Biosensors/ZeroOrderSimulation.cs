using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class ZeroOrderSimulation : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                Name = "Zero order biosensor",
                P0 = 0,
                VMax = 100e-6,
                Km = 100e-6,
            };

            biosensor.S0 = 1000 * biosensor.Km;
            biosensor.Layers = new List<Layer>
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
                        StartConcentration = biosensor.S0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 300e-6,
                        StartConcentration = biosensor.P0,
                        ReactionRate = 1
                    }
                }
            };

            return biosensor;
        }
    }
}