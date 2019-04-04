﻿using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.MicroreactorBiosensors
{
    public class TwoLayerAnalyticalMicroreactorBiosensor : BaseMicroreactorBiosensor
    {
        public TwoLayerAnalyticalMicroreactorBiosensor()
        {
            Name = "Two-Layer-Analytical-Microreactor-Biosensor";
            P0 = 0;
            VMax = 1e-12;
            Km = 100e-12;
            S0 = 1000000 * Km;

            //MicroReactorRadius = 0.1;
            //UnitRadius = 0.1;
            MicroReactorRadius = 0.1;
            UnitRadius = 0.1;
            Height = 0.1;

            Layers = new List<Layer>
            {
                new LayerWithSubAreas
                {
                    Type = LayerType.NonHomogenousLayer,
                    Height = 0.1,
                    Width = UnitRadius,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 3e-4,
                        StartConcentration = 0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 3e-4,
                        StartConcentration = 0,
                        ReactionRate = 1
                    },
                    SubAreas = new List<Area>
                    {
                        new Area
                        {
                            Type = LayerType.Enzyme,
                            Width = MicroReactorRadius,
                            Substrate = new Substrate
                            {
                                Type = SubstanceType.Substrate,
                                DiffusionCoefficient = 3e-4,
                                StartConcentration = 0,
                                ReactionRate = 1
                            },
                            Product = new Product
                            {
                                Type = SubstanceType.Product,
                                DiffusionCoefficient = 3e-4,
                                StartConcentration = 0,
                                ReactionRate = 1
                            }
                        },
                        new Area
                        {
                            Type = LayerType.DiffusionLayer,
                            Width = UnitRadius - MicroReactorRadius,
                            Substrate = new Substrate
                            {
                                Type = SubstanceType.Substrate,
                                DiffusionCoefficient = 6e-4,
                                StartConcentration = S0,
                                ReactionRate = 0
                            },
                            Product = new Product
                            {
                                Type = SubstanceType.Product,
                                DiffusionCoefficient = 6e-4,
                                StartConcentration = 0,
                                ReactionRate = 0
                            }
                        }
                    },
                }
                ,
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 0.1,
                    Width = UnitRadius,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 6e-4,
                        StartConcentration = S0,
                        ReactionRate = 0
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 6e-4,
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