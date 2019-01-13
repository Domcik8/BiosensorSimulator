namespace BiosensorSimulator.Parameters.Scheme
{
    public struct ExplicitSchemeParameters
    {
        /// <summary>
        /// DiffusionCoefficient over R
        /// </summary>
        public double DiffusionCoefficientOverR { get; set; }

        /// <summary>
        /// Diffusion coefficient over space
        /// </summary>
        public double DiffusionCoefficientOverSpace { get; set; }
    }
}