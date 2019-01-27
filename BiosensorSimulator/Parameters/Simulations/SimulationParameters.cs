namespace BiosensorSimulator.Parameters.Simulations
{
    public class SimulationParameters
    {
        // Simulation1D time, time steps
        public double T, t;

        // Simulation number of space steps and time steps
        public long N, M;
        
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
