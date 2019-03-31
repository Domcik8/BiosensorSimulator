using System;
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
            Console.WriteLine("S0: ");
            var s0 = double.Parse(Console.ReadLine());
            Console.WriteLine("Microreactor radius (mm): ");
            var mr = double.Parse(Console.ReadLine());
            Console.WriteLine("Unit radius (mm): ");
            var ur = double.Parse(Console.ReadLine());

            Name = "Two-Layer-Microreactor-Biosensor";
            P0 = 0;
            VMax = 100e-12;
            Km = 100e-12;
            //S0 = 20e-12;
            //S0 = 100e-12;
            S0 = s0;

            //MicroReactorRadius = 0.1;
            //UnitRadius = 0.1;
            MicroReactorRadius = mr;
            UnitRadius = ur;
            Height = 0.12;

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
                    Height = 0.02,
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