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

        public void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
        }

        public void CalculateNextStepForSubstance(
            Layer layer, Substance substance,
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            var n = layer.N;
            var a = substance.ImplicitScheme.A;
            var b = substance.ImplicitScheme.B;
            var c = substance.ImplicitScheme.C;
            var s = new double[n];

            substance.ImplicitScheme.A[0] = -1 / (1 + layer.H * sCur[layer.UpperBondIndex]);
            substance.ImplicitScheme.B[n] = -1 / (1 - layer.H * sCur[layer.LowerBondIndex]);

            for (var i = 1; i < n - 1; i++)
                s[i] = -SimulationParameters.t * Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);
            
            MatrixCalculator.SolveTridiagonalInPlace(a, (double[])c.Clone(), b, s, n);
        }

        public void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }
    }
}
