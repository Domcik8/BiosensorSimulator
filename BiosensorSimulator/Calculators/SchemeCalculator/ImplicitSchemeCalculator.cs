using BiosensorSimulator.Parameters.Simulations;
using System;
using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public Biosensor Biosensor { get; }
        public SimulationParameters SimulationParameters { get; }

        public ImplicitSchemeCalculator(Biosensor biosensor, SimulationParameters simulationParameters)
        {
            Biosensor = biosensor;
            SimulationParameters = simulationParameters;
        }

        public void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            foreach (var layer in Biosensor.Layers)
                CalculateNextStep(layer, sCur, pCur, sPrev, pPrev);
        }

        public void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            switch (layer.Type)
            {
                case LayerType.Enzyme:
                    CalculateReactionDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                    break;

                case LayerType.DiffusionLayer:
                    CalculateDiffusionLayerNextStep(layer, sCur, pCur, sPrev, pPrev);
                    break;

                case LayerType.SelectiveMembrane:
                    throw new NotImplementedException();
                    break;

                case LayerType.PerforatedMembrane:
                    throw new NotImplementedException();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
           
        }

        private void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            var n = layer.N;
            var s = new double[n];

            for (var i = 1; i < n - 1; i++)
            {
                var fermentReactionSpeed = Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);
                s[i] = -SimulationParameters.t * fermentReactionSpeed;
            }

            sCur = CalculateNextStep(layer, layer.Substrate, sCur, pCur, sPrev, pPrev, s);

            for (var i = 1; i < n - 1; i++)
                s[i] *= -1;

            pCur = CalculateNextStep(layer, layer.Product, sCur, pCur, sPrev, pPrev, s);
        }

        public double[] CalculateNextStep(
            Layer layer, Substance substance,
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev, double[] s)
        {
            var n = layer.N;
            var a = substance.ImplicitScheme.A;
            var b = substance.ImplicitScheme.B;
            var c = substance.ImplicitScheme.C;

            substance.ImplicitScheme.A[n - 1] = -1 / (1 + layer.H * sCur[layer.UpperBondIndex]);
            substance.ImplicitScheme.B[0] = -1 / (1 - layer.H * sCur[layer.LowerBondIndex]);
            
            return MatrixCalculator.SolveTridiagonalInPlace(a, (double[])c.Clone(), b, s, n);
        }
    }
}
