using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class TwoLayerMicroreactorBiosensor : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                Name = "Two-Layer-Microreactor-Biosensor",
                P0 = 0,
                VMax = 100e-3,
                Km = 100e-3,
                S0 = 20e-3
            };

            biosensor.MicroReactorRadius = 0.16e-3;
            biosensor.UnitRadius = 0.2e-3;
            biosensor.Height = 0.12e-3;

            biosensor.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 0.1e-3,
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
                    }
                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 0.02e-3,
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
                        StartConcentration = 0,
                        ReactionRate = 0
                    }
                }
            };

            biosensor.IsHomogenized = true;
            biosensor.UseEffectiveDiffusionCoefficent = true;
            biosensor.UseEffectiveReactionCoefficent = true;

            return biosensor;
        }
    }
}