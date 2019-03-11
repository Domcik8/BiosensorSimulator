namespace BiosensorSimulator.Parameters.Simulations
{
    public class SimulationParameters
    {
        // Simulation1D time, time steps
        public double T { get; set; }
        public double t { get; set; }

        // Simulation number of space steps and time steWps
        public long N { get; set; }
        public long M { get; set; }

        // Number of electrons involved in charge transfer
        public double ne { get; set; }

        // Response time decay rate
        public double DecayRate { get; set; }

        // Faraday Constant
        public double F { get; set; }

        // Lowest allowed current threshold
        public double ZeroIBond { get; set; }
    }
}
