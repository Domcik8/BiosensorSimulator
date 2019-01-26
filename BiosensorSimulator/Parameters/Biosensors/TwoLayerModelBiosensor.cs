using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class TwoLayerModelBiosensor : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                Name = "Two model biosensor",
                P0 = 0,
                VMax = 10e-12,
                Km = 100e-12,
                S0 = 100e-12
            };

            //biosensor.S0 = 0.01 * biosensor.Km;
            biosensor.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 100e-3,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 300e-6,
                        StartConcentration = 0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 300e-6,
                        StartConcentration = 0,
                        ReactionRate = 1
                    }
                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 100e-3,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 600e-6,
                        StartConcentration = biosensor.S0,
                        ReactionRate = 0
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 600e-6,
                        StartConcentration = 0,
                        ReactionRate = 0
                    }
                }
            };

            return biosensor;
        }
    }
}