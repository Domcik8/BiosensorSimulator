using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class TwoLayerPerforatedMembraneBiosensor : BasePerforatedMembraneBiosensor
    {
        public TwoLayerPerforatedMembraneBiosensor()
        {
            Name = "Four-Layer-perforated membrane biosensor";
            P0 = 0;
            VMax = 100e-12;
            Km = 100e-12;
            S0 = 100e-12;

            HoleRadius = 0.1e-3;
            HalfDistanceBetweenHoles = 1e-3;
            EnzymeHoleHeight = 5e-3;
            PartitionCoefficient = 0.5;

            Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.SelectiveMembrane,
                    Height = 2e-3,
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 1e-6,
                        StartConcentration = 0,
                        ReactionRate = 0
                    }
                },
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 2e-3,
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
                    Type = LayerType.PerforatedMembrane,
                    Height = 10e-3,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 0,
                        StartConcentration = 0,
                        ReactionRate = 0
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 0,
                        StartConcentration = 0,
                        ReactionRate = 0
                    }
                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 2e-3,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 600e-6,
                        StartConcentration = S0,
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

            IsHomogenized = true;
            UseEffectiveDiffusionCoefficent = true;
            UseEffectiveReactionCoefficent = true;
        }
    }
}