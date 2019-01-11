namespace BiosensorSimulator.Parameters.Biosensors
{
    public struct BiosensorParameters
    {
        // Initial concentration of substrate and product
        public double S0, P0;

        // Substrate and Product diffusion cofficents in enzyme layer
        public double DSf, DPf;

        // Substrate and Product diffusion cofficent in diffusion layer
        public double DSd, DPd;

        // Maximal enzymatic rate
        public double Vmax;

        // Michaelis constant
        public double Km;

        // Biosensor unit parameters:
        // a - microreactor radius,
        // b - unit radius,
        // c - ferment layer height,
        // d - diffusion layer height,
        // n - Nernst layer height (d - c)
        public double a, b, c, d, n;
    }
}
