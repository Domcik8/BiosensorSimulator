using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class ZeroOrderSimulation : IBiosensorParametersSuplier
    {
        public BiosensorParameters GetInitiationParameters()
        {
            //BiosensorParameters biosensorParameters = new BiosensorParameters()
            //{
            //    P0 = 0,
            //    DSf = 300e-6,
            //    DPf = 300e-6,
            //    Vmax = 100e-6,
            //    Km = 100e-6,
            //    c = 0.01e-3
            //};

            //biosensorParameters.S0 = 1000 * biosensorParameters.Km;

            var biosensorParameters = new BiosensorParameters
            {
                S0 = 1000 * 100e-6,
                P0 = 0,
                VMax = 100e-6,
                Km = 100e-6,
            };

            biosensorParameters.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height = 0.01e-3,
                    Substances = new List<Substance>
                    {
                        new Substance
                        {
                            Type = SubstanceType.Substrate,
                            DiffusionCoefficient = 300e-6,
                            StartConcentration = biosensorParameters.S0,
                            ReactionRate = 1
                        },
                        new Substance
                        {
                            Type = SubstanceType.Product,
                            DiffusionCoefficient = 300e-6,
                            StartConcentration = biosensorParameters.P0,
                            ReactionRate = 1
                        }
                    }
                }
            };

            return biosensorParameters;
        }
    }
}