using BiosensorSimulator.Parameters.Simulations;
using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Scheme;

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
                    throw new NotImplementedException();
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

        private void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            var f = new double[layer.N];

            for (var i = 1; i < layer.N - 1; i++)
            {
                var fermentReactionSpeed = Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);
                f[i] = -SimulationParameters.t * fermentReactionSpeed;
            }
            
            CalculateNextStep(layer, layer.Substrate.ImplicitScheme, sCur, f);

            for (var i = 1; i < layer.N - 1; i++)
                f[i] *= -1;

            CalculateNextStep(layer, layer.Substrate.ImplicitScheme, pCur, f);
        }

        public void CalculateNextStep(
            Layer layer, ImplicitSchemeParameters parameters,
            double[] s, double[] f)
        {
            FillSchemeParameters(parameters, layer, s, f);


            s[layer.N - 1] = //layer.LastLayer ? 0 :
                (parameters.Niu2 + parameters.Beta2 * parameters.E[layer.N - 1]) /
                (1 - parameters.D[layer.N - 1] * parameters.Beta2);
            
            for (var i = layer.N - 2; i >= 0; i--)
                s[i] = parameters.D[i + 1] * s[i + 1] + parameters.E[i + 1];

            s[0] = layer.FirstLayer ? Biosensor.S0 :
                parameters.Beta1 * s[1] + parameters.Niu1;
        }

        public void FillSchemeParameters(ImplicitSchemeParameters parameters, Layer layer, double[] s, double[] f)
        {
            if (layer.FirstLayer)
            {
                parameters.Y0 = 1;
                parameters.Niu1 = Biosensor.S0;
            } 
            else
            {
                //parameters.Y0 = 1;
                //parameters.U0 = s[layer.LowerBondIndex];
                
                parameters.Niu1 = layer.H * parameters.Y0 * parameters.U0 / (1 + layer.H * parameters.Y0);
            }

            if (layer.LastLayer)
            {
                parameters.Y1 = 1;
                parameters.Niu2 = 0;
            }
            else
            {
                //parameters.Y1 = 1;
                //parameters.U1 = s[layer.UpperBondIndex];

                parameters.Niu2 = layer.H * parameters.Y1 * parameters.U1 / (layer.H * parameters.Y1 - 1);
            }

            parameters.Beta1 = 1 / (1 + layer.H * parameters.Y0);
            parameters.Beta2 = 1 / (1 - layer.H * parameters.Y1);

            parameters.A[layer.N - 1] = -parameters.Beta2;
            parameters.B[0] = -parameters.Beta1;

            parameters.F = f;
            parameters.F[0] = parameters.Niu1;
            parameters.F[layer.N - 1] = parameters.Niu2;

            parameters.D[0] = parameters.B[1] / (parameters.C[1] - parameters.A[1] * parameters.Beta1);
            parameters.E[0] = (parameters.A[1] * parameters.Niu1 - parameters.F[1]) /
                                  (parameters.C[1] -  parameters.A[1] * parameters.Beta1);

            for (var i = 0; i < layer.N - 1; i++)
            {
                parameters.D[i + 1] = parameters.B[i] / (parameters.C[i] - parameters.D[i] * parameters.A[i]);
                parameters.E[i + 1] = (parameters.A[i] * parameters.E[i] - parameters.F[i]) /
                    (parameters.C[i] - parameters.D[i] * parameters.A[i]);
            } 
        }
    }
}
