using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class TwoLayerMicroreactorBiosensor : BaseBiosensor
    {
        public TwoLayerMicroreactorBiosensor()
        {
            Name = "Two-Layer-Microreactor-Biosensor";
            P0 = 0;
            VMax = 100e-3;
            Km = 100e-3;
            S0 = 20e-3;

            MicroReactorRadius = 0.16e-3;
            UnitRadius = 0.2e-3;
            Height = 0.12e-3;

            Layers = new List<Layer>
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
                        StartConcentration = S0,
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

            IsHomogenized = true;
            UseEffectiveDiffusionCoefficent = true;
            UseEffectiveReactionCoefficent = true;
        }
    }
}