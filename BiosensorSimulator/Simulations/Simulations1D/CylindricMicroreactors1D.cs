using System;
using System.Linq;
using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Simulations;
using BiosensorSimulator.Results;

namespace BiosensorSimulator.Simulations.Simulations1D
{
    public class CylindricMicroreactors1D : BaseSimulation
    {
        public CylindricMicroreactors1D(
            SimulationParameters simulationParameters,
            BaseBiosensor biosensor,
            ISchemeCalculator schemeCalculator,
            IResultPrinter resultPrinter) : base(simulationParameters, biosensor, schemeCalculator, resultPrinter) { }

        public override void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);

            SchemeCalculator.CalculateNextStep(SCur, PCur, SPrev, PPrev);
            CalculateMatchingConditions();
            CalculateBoundaryConditions();
        }

        public override void Homogenize()
        {
            if (Biosensor.UseEffectiveReactionCoefficent)
                Biosensor.EffectiveReactionCoefficent = GetEffectiveReactionCoefficent(Biosensor);

            if (Biosensor.UseEffectiveDiffusionCoefficent)
            {
                Biosensor.EnzymeLayer.Substrate.DiffusionCoefficient = GetEffectiveDiffusionCoefficent(
                    Biosensor,
                    Biosensor.EnzymeLayer.Substrate.DiffusionCoefficient,
                    Biosensor.DiffusionLayer.Substrate.DiffusionCoefficient);

                Biosensor.EnzymeLayer.Product.DiffusionCoefficient = GetEffectiveDiffusionCoefficent(
                    Biosensor,
                    Biosensor.EnzymeLayer.Product.DiffusionCoefficient,
                    Biosensor.DiffusionLayer.Product.DiffusionCoefficient);
            }
        }

        private double GetEffectiveReactionCoefficent(BaseBiosensor biosensor)
        {
            var microreactorArea = biosensor.MicroReactorRadius * biosensor.MicroReactorRadius;
            var unitArea = biosensor.UnitRadius * biosensor.UnitRadius;

            return microreactorArea / unitArea;
        }

        private double GetEffectiveDiffusionCoefficent(BaseBiosensor biosensor,
            double enzymelayerDiffusionCoefficent1, double diffusionLayerDiffusionCoefficent2)
        {
            var unitArea = biosensor.UnitRadius * biosensor.Height;
            var enzymeArea = biosensor.MicroReactorRadius * biosensor.Height;
            //var diffusionLayerArea = unitArea - enzymeArea;

            var relativeArea = enzymeArea / unitArea;

            var effectiveDiffusionCoefficentMax = enzymelayerDiffusionCoefficent1 * relativeArea +
                                                  (1 - relativeArea) * diffusionLayerDiffusionCoefficent2;

            var effectiveDiffusionCoefficentMin =
                enzymelayerDiffusionCoefficent1 * diffusionLayerDiffusionCoefficent2 / effectiveDiffusionCoefficentMax;

            return Math.Min(effectiveDiffusionCoefficentMin, effectiveDiffusionCoefficentMax);
        }

        private void CalculateBoundaryConditions()
        {
            var firstLayer = Biosensor.Layers.First();

            if (firstLayer.Type == LayerType.SelectiveMembrane)
            {
                SetBoundaryConditionsWithSelectiveMembrane(firstLayer);
            }
            else
            {
                SetBoundaryConditions();
            }
        }

        private void SetBoundaryConditions()
        {
            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            SCur[0] = SCur[1];

            PCur[SimulationParameters.N - 1] = Biosensor.P0;
            PCur[0] = 0;
        }

        private void SetBoundaryConditionsWithSelectiveMembrane(Layer selectiveMembraneLayer)
        {
            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            SCur[selectiveMembraneLayer.UpperBondIndex] = SCur[selectiveMembraneLayer.UpperBondIndex + 1];

            PCur[SimulationParameters.N - 1] = Biosensor.P0;
            PCur[0] = 0;
        }

        private void CalculateMatchingConditions()
        {
            foreach (var layer in Biosensor.Layers)
            {
                var index = Biosensor.Layers.IndexOf(layer);

                if (index == 0)
                {
                    continue;
                }

                var previousLayer = Biosensor.Layers[index - 1];

                if (previousLayer.Type == LayerType.SelectiveMembrane)
                {
                    SetMatchingConditionsWithSelectiveMembrane(layer, previousLayer);
                }
                else
                {
                    SetMatchingConditions(layer, Biosensor.Layers[index - 1]);
                }
            }
        }

        private void SetMatchingConditions(Layer layer, Layer previousLayer)
        {

            SCur[layer.LowerBondIndex] =
                (previousLayer.H * layer.Substrate.DiffusionCoefficient * SCur[layer.LowerBondIndex + 1] + layer.H * previousLayer.Substrate.DiffusionCoefficient *
                 SCur[layer.LowerBondIndex - 1]) / (layer.H * previousLayer.Substrate.DiffusionCoefficient + previousLayer.H * layer.Substrate.DiffusionCoefficient);

            PCur[layer.LowerBondIndex] =
                (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1] + layer.H * previousLayer.Product.DiffusionCoefficient *
                 PCur[layer.LowerBondIndex - 1]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
        }

        private void SetMatchingConditionsWithSelectiveMembrane(Layer layer, Layer previousLayer)
        {
            PCur[layer.LowerBondIndex] =
                (previousLayer.H * layer.Product.DiffusionCoefficient * PCur[layer.LowerBondIndex + 1] + layer.H * previousLayer.Product.DiffusionCoefficient *
                 PCur[layer.LowerBondIndex - 1]) / (layer.H * previousLayer.Product.DiffusionCoefficient + previousLayer.H * layer.Product.DiffusionCoefficient);
        }
    }
}