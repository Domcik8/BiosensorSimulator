namespace BiosensorSimulator.Parameters.Biosensors.Base.Layers
{
    public class Layer : Area
    {
        public bool FirstLayer { get; set; } = false;

        public bool LastLayer { get; set; } = false;

        public double FullWidth { get; set; }
    }
}
