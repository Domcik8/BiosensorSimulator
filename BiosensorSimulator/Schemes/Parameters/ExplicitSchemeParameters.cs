using BiosensorSimulator.Parameters.Biosensors.Base.Layers;

namespace BiosensorSimulator.Calculators.SchemeParameters
{
    public class ExplicitSchemeParameters
    {
        /// <summary>
        /// Diffusion coefficient over R
        /// </summary>
        public double DiffusionCoefficientOverR { get; set; }

        /// <summary>
        /// Diffusion coefficient over space
        /// </summary>
        public double DiffusionCoefficientOverSpace { get; set; }

        public double H2 { get; set; }
        public double W2 { get; set; }

        public ExplicitSchemeParameters(Area layer, Substance substance)
        {
            H2 = layer.H * layer.H;
            W2 = layer.W * layer.W;

            DiffusionCoefficientOverR = substance.DiffusionCoefficient * layer.R;
            DiffusionCoefficientOverSpace = substance.DiffusionCoefficient / (layer.H * layer.H);
        }
    }
}