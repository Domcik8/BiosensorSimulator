using BiosensorSimulator.Calculators.SchemeParameters;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.Base.Layers
{
    public class Substance
    {
        public SubstanceType Type { get; set; }

        public double DiffusionCoefficient { get; set; }

        public double StartConcentration { get; set; }

        public int ReactionRate { get; set; }

        public ExplicitSchemeParameters ExplicitScheme { get; set; }

        public ImplicitSchemeParameters ImplicitScheme { get; set; }
    }

    public class Product : Substance { }

    public class Substrate : Substance { }
}
