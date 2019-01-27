namespace BiosensorSimulator.Parameters.Biosensors.Base
{
    public abstract class BaseHomogenousBiosensor : BaseBiosensor
    {
        public bool IsHomogenized { get; set; } = false;
        public bool UseEffectiveDiffusionCoefficent { get; set; } = false;
        public bool UseEffectiveReactionCoefficent { get; set; } = false;

        public double EffectiveDiffusionCoefficent { get; set; } = 1;
        public double EffectiveReactionCoefficent { get; set; } = 1;

        public abstract void Homogenize();
    }
}
