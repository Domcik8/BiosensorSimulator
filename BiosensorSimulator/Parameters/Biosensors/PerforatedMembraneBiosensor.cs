﻿using BiosensorSimulator.Parameters.Biosensors.Base;
using System;
using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class PerforatedMembraneBiosensor : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                Name = "Biosensor with perforated membrane",
                P0 = 0,
                VMax = 1e-12,
                Km = 100e-12,
                S0 = 100e-12
            };

            biosensor.Layers = new List<Layer>
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
            },
        };

            return biosensor;
        }
    }
}