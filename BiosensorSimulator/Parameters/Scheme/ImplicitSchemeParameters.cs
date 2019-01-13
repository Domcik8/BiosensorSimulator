namespace BiosensorSimulator.Parameters.Scheme
{
    public struct ImplicitSchemeParameters
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