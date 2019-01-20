using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class Bik0401 : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                S0 = 20e-6,
                P0 = 0,
                VMax = 100e-6,
                Km = 1.0e-6,
                MicroReactorRadius = 1e-4,
                UnitRadius = 2e-4,
                NerstLayerHeight = 2e-5,
                BiosensorHeight = 12e-5
            };

            biosensor.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height =  1e-4,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 3e-10,
                        StartConcentration = 0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 3e-10,
                        StartConcentration = 0,
                        ReactionRate = 1
                    },
                    FirstLayer = true

                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 2e-5,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 6e-10,
                        StartConcentration = biosensor.S0,
                        ReactionRate = 0
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 6e-10,
                        StartConcentration = biosensor.P0,
                        ReactionRate = 0
                    },
                    LastLayer = true
                }
            };

            return biosensor;
        }
    }
}