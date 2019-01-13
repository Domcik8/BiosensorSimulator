using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Scheme
{
    public struct ExplicitSchemeParameters
    {
        /// <summary>
        /// Diffusion coefficient over R
        /// </summary>
        public double DiffusionCoefficientOverR { get; set; }

        /// <summary>
        /// Diffusion coefficient over space
        /// </summary>
        public double DiffusionCoefficientOverSpace { get; set; }

        public ExplicitSchemeParameters(Layer layer, Substance substance)
        {
            DiffusionCoefficientOverR = substance.DiffusionCoefficient * layer.R;
            DiffusionCoefficientOverSpace = substance.DiffusionCoefficient / (layer.H * layer.H);
        }
    }
}