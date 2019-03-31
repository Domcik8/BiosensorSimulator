using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.PerforatedMembraneBiosensors
{
    public class PerforatedMembraneBiosensor : BaseBiosensor
    {
        public PerforatedMembraneBiosensor()
        {
            Name = "Perforated-Membrane-Biosensor";
            P0 = 0;
            VMax = 1e-12;
            Km = 100e-12;
            S0 = 100e-12;

            Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.SelectiveMembrane,
                    Height = 2e-3,
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 3e-6,
                        StartConcentration = 0,
                        ReactionRate = 0
                    }
                },
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 4e-3,
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
                    Height = 8e-3,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 30e-6,
                        StartConcentration = 0,
                        ReactionRate = 0
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 30e-6,
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
                },
            };
        }
    }
}