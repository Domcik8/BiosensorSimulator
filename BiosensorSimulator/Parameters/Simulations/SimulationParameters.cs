namespace BiosensorSimulator.Parameters.Simulations
{
    public struct SimulationParameters
    {
        // Simulation1D time, time steps and space steps
        public double T, t, hd, hf;

        // Simulation number of space steps in diffusion layer and time steps
        public long Nd, Nf, N, M;
        
        // Number of electrons involved in charge transfer
        public int ne;

        // Response time decay rate
        public double DecayRate;

        // Faraday Constant
        public double F;
        
        // Lowest allowed current threshold
        public double ZeroIBond;
    }
}
