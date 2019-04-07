﻿using System;
using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.MicroreactorBiosensors
{
    public class TwoLayerMicroreactorBiosensor : BaseMicroreactorBiosensor
    {
        public TwoLayerMicroreactorBiosensor()
        {
            Name = "Two-Layer-Microreactor-Biosensor";
            P0 = 0;
            VMax = 3e-12;
            Km = 1e-10;
            S0 = 1e-10;

            MicroReactorRadius = 0.2;
            UnitRadius = 0.2;
            Height = 0.3;

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
                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 0.2,
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