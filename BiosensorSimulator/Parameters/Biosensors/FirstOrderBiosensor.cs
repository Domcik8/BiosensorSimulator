using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class FirstOrderBiosensor : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                Name = "First-Order-Biosensor",
                P0 = 0,
                VMax = 100e-6, //-6 decimeters / -3 meters / -12 milimeters
                Km = 100e-6 //-6 decimeters / -3 meters / -12 milimeters
            };

            biosensor.S0 = 0.01 * biosensor.Km;
            biosensor.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 0.1, //e0 milimiter, e-3 meter
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 300e-6, //e-6 milimiter, e-12 meter
                        StartConcentration = biosensor.S0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 300e-6, //e-6 milimiter, e-12 meter
                        StartConcentration = biosensor.P0,
                        ReactionRate = 1
                    },
                    FirstLayer = true,
                    LastLayer = true
                }
            };

            return biosensor;
        }
    }
}