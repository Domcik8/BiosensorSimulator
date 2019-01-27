namespace BiosensorSimulator.Parameters.Biosensors.Base
{
    public abstract class BasePerforatedMembraneBiosensor : BaseHomogenousBiosensor
    {
        public double HoleRadius { get; set; }
        public double EnzymeHoleHeight { get; set; }
        public double HalfDistanceBetweenHoles { get; set; }

        public override void Homogenize()
        {
            //if (Biosensor.UseEffectiveReactionCoefficent)
            //{
            //    Biosensor.EffectiveReactionCoefficent = GetEffectiveReactionCoefficent(Biosensor);
            //}

            if (UseEffectiveDiffusionCoefficent)
            {
                PerforatedMembraneLayer.Substrate.DiffusionCoefficient = GetEffectiveDiffusionCoefficient(
                    EnzymeLayer.Substrate.DiffusionCoefficient,
                    DiffusionLayer.Substrate.DiffusionCoefficient);

                PerforatedMembraneLayer.Product.DiffusionCoefficient = GetEffectiveDiffusionCoefficient(
                    EnzymeLayer.Product.DiffusionCoefficient,
                    DiffusionLayer.Product.DiffusionCoefficient);
            }
        }

        //private double GetEffectiveReactionCoefficent(Biosensor biosensor)
        //{
        //    var microreactorArea = biosensor.MicroReactorRadius * biosensor.MicroReactorRadius;
        //    var unitArea = biosensor.UnitRadius * biosensor.UnitRadius;

        //    return microreactorArea / unitArea;
        //}

        private double GetEffectiveDiffusionCoefficient(double enzymeLayerDiffusionCoefficient, double diffusionLayerDiffusionCoefficient)
        {
            var beta = EnzymeHoleHeight / PerforatedMembraneLayer.Height;
            var alfa = (HoleRadius * HoleRadius) / (HalfDistanceBetweenHoles * HalfDistanceBetweenHoles);
            var insideHole = beta * enzymeLayerDiffusionCoefficient + (1 - beta) * diffusionLayerDiffusionCoefficient;
            return alfa * insideHole;
        }
    }
}