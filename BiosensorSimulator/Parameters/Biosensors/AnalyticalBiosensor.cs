using BiosensorSimulator.Parameters.Biosensors.Base;
using System.Collections.Generic;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class AnalyticalBiosensor : BaseBiosensor
    {
        public AnalyticalBiosensor()
        {
            Name = "Analytical-Biosensor";
            P0 = 0;
            VMax = 1000e-12; //-6 decimeters / -3 meters / -12 milimeters
            Km = 100e-12; //-6 decimeters / -3 meters / -12 milimeters

            // First Order
            // S0 = 0.01 * Km;

            // Zero Order
            S0 = 1000000 * Km;

            Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 0.1, //e0 milimiter, e-3 meter
                    Substrate = new Substrate
                    {
                        Type = SubstanceType.Substrate,
                        DiffusionCoefficient = 300e-6, //e-6 milimiter, e-12 meter
                        StartConcentration = S0,
                        ReactionRate = 1
                    },
                    Product = new Product
                    {
                        Type = SubstanceType.Product,
                        DiffusionCoefficient = 300e-6, //e-6 milimiter, e-12 meter
                        StartConcentration = P0,
                        ReactionRate = 1
                    },
                    FirstLayer = true,
                    LastLayer = true
                }
            };
        }
    }
}