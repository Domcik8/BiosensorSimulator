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

            /*substance.ImplicitScheme.A[0] = -1 / (1 + layer.H * sCur[LAYER_START]);
            substance.ImplicitScheme.B[n] = -1 / (1 - layer.H * sCur[LAYER_END]);
            
            MatrixCalculator.SolveTridiagonalInPlace(a, c.Clone(), b, r, n);*/
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
