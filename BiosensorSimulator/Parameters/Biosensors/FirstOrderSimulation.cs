namespace BiosensorSimulator.Parameters.Biosensors
{
    public class FirstOrderSimulation : IBiosensorParametersSuplier
    {
        public BiosensorParameters GetInitiationParameters()
        {
            BiosensorParameters biosensorParameters = new BiosensorParameters()
            {
                P0 = 0,
                DSf = 300e-6,
                DPf = 300e-6,
                Vmax = 100e-6,
                Km = 100e-6,
                c = 0.01e-3
            };

            biosensorParameters.S0 = 0.01 * biosensorParameters.Km;

            return biosensorParameters;
        }
    }
}