using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class TwoLayerPerforatedMembraneBiosensor2D : BaseBiosensor
    {
        public TwoLayerPerforatedMembraneBiosensor2D()
        {
            Name = "Two-Layer-perforated membrane biosensor 2D";
            P0 = 0;
            VMax = 100e-12;
            Km = 100e-12;
            S0 = 100e-12;

            Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.SelectiveMembrane,
                    Height = 2e-3,
                    Width = 1e-3,
                    FullWidth = 1e-3,
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
                    Width = 1e-3,
                    FullWidth = 1e-3,
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
                //new Layer
                //{
                //    Type = LayerType.Enzyme,
                //    Height = 0,
                //    Width = 0.1e-3,
                //    Substrate = new Substrate
                //    {
                //        Type = SubstanceType.Substrate,
                //        DiffusionCoefficient = 300e-6,
                //        StartConcentration = 0,
                //        ReactionRate = 1
                //    },
                //    Product = new Product
                //    {
                //        Type = SubstanceType.Product,
                //        DiffusionCoefficient = 300e-6,
                //        StartConcentration = 0,
                //        ReactionRate = 1
                //    }
                //},
                new Layer
                {
                    Type = LayerType.DiffusionSmallLayer,
                    Height = 10e-3,
                    Width = 0.4e-3,
                    FullWidth = 1e-3,
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
                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 2e-3,
                    Width = 1e-3,
                    FullWidth = 1e-3,
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
        }
    }
}