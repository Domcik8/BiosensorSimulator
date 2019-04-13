using System;

namespace BiosensorSimulator.Parameters.Biosensors.Base
{
    public abstract class BaseMicroreactorBiosensor : BaseHomogenousBiosensor
    {
        public double EffectiveSubstrateDiffusionCoefficient { get; set; }
        public double EffectiveProductDiffusionCoefficient { get; set; }
        public double MicroReactorRadius { get; set; }
        public double UnitRadius { get; set; }

        public override void Homogenize()
        {
            if (UseEffectiveReactionCoefficent)
                EffectiveReactionCoefficent = GetEffectiveReactionCoefficent();

            if (UseEffectiveDiffusionCoefficent)
            {
                NonHomogenousLayer.Substrate.DiffusionCoefficient = EffectiveSubstrateDiffusionCoefficient;
                NonHomogenousLayer.Product.DiffusionCoefficient = EffectiveProductDiffusionCoefficient;
            }
        }

        private double GetEffectiveReactionCoefficent()
        {
            var enzymeArea = MicroReactorRadius * MicroReactorRadius;
            var unitArea = UnitRadius * UnitRadius;

            return enzymeArea / unitArea;
        }

        public double GetEffectiveDiffusionCoefficent(
            double enzymelayerDiffusionCoefficent1, double diffusionLayerDiffusionCoefficent2)
        {
            var enzymeArea = MicroReactorRadius * MicroReactorRadius;
            var unitArea = UnitRadius * UnitRadius;

            var relativeArea = enzymeArea / unitArea;

            var effectiveDiffusionCoefficentMax = enzymelayerDiffusionCoefficent1 * relativeArea +
                                                  (1 - relativeArea) * diffusionLayerDiffusionCoefficent2;

            var effectiveDiffusionCoefficentMin =
                enzymelayerDiffusionCoefficent1 * diffusionLayerDiffusionCoefficent2 / effectiveDiffusionCoefficentMax;

            return Math.Min(effectiveDiffusionCoefficentMin, effectiveDiffusionCoefficentMax);
        }
    }
}
