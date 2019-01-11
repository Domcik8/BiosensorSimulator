namespace BiosensorSimulator.Parameters.Biosensors
{
    public class Bik0401 : IBiosensorParametersSuplier
    {
        public BiosensorParameters GetInitiationParameters()
        {
            return new BiosensorParameters()
            {
                S0 = 20e-6,
                P0 = 0,
                DSf = 3e-10,
                DPf = 3e-10,
                DSd = 6e-10,
                DPd = 6e-10,
                Vmax = 100e-6,
                Km = 1.0e-6,
                a = 1e-4,
                b = 2e-4,
                c = 1e-4,
                d = 12e-5,
                n = 2e-5
            };
        }
    }
}