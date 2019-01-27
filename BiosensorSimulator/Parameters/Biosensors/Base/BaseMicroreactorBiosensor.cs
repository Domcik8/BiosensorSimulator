using System;

namespace BiosensorSimulator.Parameters.Biosensors.Base
{
    public abstract class BaseMicroreactorBiosensor : BaseHomogenousBiosensor
    {
        public double MicroReactorRadius { get; set; }
        public double UnitRadius { get; set; }

        public override void Homogenize()
        {
            if (UseEffectiveReactionCoefficent)
                EffectiveReactionCoefficent = GetEffectiveReactionCoefficent();

            if (UseEffectiveDiffusionCoefficent)
            {
                EnzymeLayer.Substrate.DiffusionCoefficient = GetEffectiveDiffusionCoefficent(
                    EnzymeLayer.Substrate.DiffusionCoefficient,
                    DiffusionLayer.Substrate.DiffusionCoefficient);

                EnzymeLayer.Product.DiffusionCoefficient = GetEffectiveDiffusionCoefficent(
                    EnzymeLayer.Product.DiffusionCoefficient,
                    DiffusionLayer.Product.DiffusionCoefficient);
            }
        }

        private double GetEffectiveReactionCoefficent()
        {
            var microreactorArea = MicroReactorRadius * MicroReactorRadius;
            var unitArea = UnitRadius * UnitRadius;

            return microreactorArea / unitArea;
        }

        private double GetEffectiveDiffusionCoefficent(
            double enzymelayerDiffusionCoefficent1, double diffusionLayerDiffusionCoefficent2)
        {
            var unitArea = UnitRadius * Height;
            var enzymeArea = MicroReactorRadius * Height;
            //var diffusionLayerArea = unitArea - enzymeArea;

            var relativeArea = enzymeArea / unitArea;

            var effectiveDiffusionCoefficentMax = enzymelayerDiffusionCoefficent1 * relativeArea +
                                                  (1 - relativeArea) * diffusionLayerDiffusionCoefficent2;

            var effectiveDiffusionCoefficentMin =
                enzymelayerDiffusionCoefficent1 * diffusionLayerDiffusionCoefficent2 / effectiveDiffusionCoefficentMax;

            return Math.Min(effectiveDiffusionCoefficentMin, effectiveDiffusionCoefficentMax);
        }
    }
}
