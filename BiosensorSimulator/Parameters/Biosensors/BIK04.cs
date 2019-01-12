using System.Collections.Generic;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class Bik0401 : IBiosensorParametersSuplier
    {
        public BiosensorParameters GetInitiationParameters()
        {
            //new BiosensorParameters()
            //{
                //S0 = 20e-6,
                //P0 = 0,
                //DSf = 3e-10,
                //DPf = 3e-10,
                //DSd = 6e-10,
                //DPd = 6e-10,
                //Vmax = 100e-6,
                //Km = 1.0e-6,
                //a = 1e-4,
                //b = 2e-4,
                //c = 1e-4,
                //d = 12e-5,
                //n = 2e-5
            //};

            var biosensorParameters = new BiosensorParameters
            {
                S0 = 20e-6,
                P0 = 0,
                VMax = 100e-6,
                Km = 1.0e-6,
                MicroReactorRadius = 1e-4,
                UnitRadius = 2e-4,
                NerstLayerHeight = 2e-5
            };

            biosensorParameters.Layers = new List<Layer>
            {
                new Layer
                {
                    Type = LayerType.Enzyme,
                    Height =  1e-4,
                    Substances = new List<Substance>
                    {
                        new Substance
                        {
                            Type = SubstanceType.Substrate,
                            DiffusionCoefficient = 3e-10,
                            StartConcentration = 0,
                            ReactionRate = 1
                        },
                        new Substance
                        {
                            Type = SubstanceType.Product,
                            DiffusionCoefficient = 3e-10,
                            StartConcentration = 0,
                            ReactionRate = 1
                        }
                    }
                },
                new Layer
                {
                    Type = LayerType.DiffusionLayer,
                    Height = 12e-5,
                    Substances = new List<Substance>
                    {
                        new Substance
                        {
                            Type = SubstanceType.Substrate,
                            DiffusionCoefficient = 6e-10,
                            StartConcentration = biosensorParameters.S0,
                            ReactionRate = 0
                        },
                        new Substance
                        {
                            Type = SubstanceType.Product,
                            DiffusionCoefficient = 6e-10,
                            StartConcentration = biosensorParameters.P0,
                            ReactionRate = 0
                        }
                    }
                }
            };

            return biosensorParameters;
        }
    }
}