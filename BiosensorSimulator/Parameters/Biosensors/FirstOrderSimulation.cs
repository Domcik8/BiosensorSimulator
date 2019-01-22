﻿using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class FirstOrderSimulation : IBiosensorSupplier
    {
        public Biosensor GetInitiationParameters()
        {
            var biosensor = new Biosensor
            {
                Name = "First order biosensor",
                P0 = 0,
                VMax = 100e-6, //3
                Km = 100e-6 //9
            };

            biosensor.S0 = 0.01 * biosensor.Km;
            biosensor.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 0.01, //e-3 e-6,
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 300e-6, //e-12,
                        StartConcentration = biosensor.S0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 300e-6,
                        StartConcentration = biosensor.P0,
                        ReactionRate = 1
                    },
                    FirstLayer = true,
                    LastLayer = true
                }
            };

            return biosensor;
        }
    }
}