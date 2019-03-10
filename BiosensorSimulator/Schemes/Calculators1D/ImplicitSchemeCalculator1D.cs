using BiosensorSimulator.Calculators;
using BiosensorSimulator.Calculators.SchemeParameters;
using BiosensorSimulator.Parameters.Biosensors.Base;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Schemes.Calculators1D
{
    public class ImplicitSchemeCalculator1D : ISchemeCalculator1D
    {
        public BaseBiosensor Biosensor { get; }
        public SimulationParameters SimulationParameters { get; }

        public ImplicitSchemeCalculator1D(BaseBiosensor biosensor, SimulationParameters simulationParameters)
        {
            Biosensor = biosensor;
            SimulationParameters = simulationParameters;

            foreach (var layer in biosensor.Layers)
            {
                layer.Product.ImplicitScheme = new ImplicitSchemeParameters(biosensor, layer, layer.Product);

                if (layer.Type == LayerType.SelectiveMembrane)
                    continue;

                layer.Substrate.ImplicitScheme = new ImplicitSchemeParameters(biosensor, layer, layer.Substrate);
            }
        }

        public void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            foreach (var layer in Biosensor.Layers)
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
        }

        private void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            var f = new double[layer.N];

            //ToDo: think why we use sPrev and also make these 2 for loops more effective
            for (var i = 1; i < layer.N - 1; i++)
            {
                var fermentReactionSpeed = Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);
                f[i] = -sPrev[i] + SimulationParameters.t * fermentReactionSpeed;
            }

            CalculateNextStep(layer, layer.Substrate, sCur, f);

            for (var i = 1; i < layer.N - 1; i++)
            {
                var fermentReactionSpeed = Biosensor.VMax * sPrev[i] / (Biosensor.Km + sPrev[i]);
                f[i] = -pPrev[i] - SimulationParameters.t * fermentReactionSpeed;
            }

            CalculateNextStep(layer, layer.Product, pCur, f);
        }

        public void CalculateNextStep(
            Layer layer, Substance substance,
            double[] s, double[] f)
        {
            FillSchemeParameters(substance, layer, s, f);
            var parameters = substance.ImplicitScheme;


            MatrixCalculator.SolveTriagonalLinearSystem(
                parameters.A, parameters.B, parameters.C,
                parameters.D, parameters.E, parameters.F,
                s,
                parameters.Beta1, parameters.Beta2,
                parameters.Niu1, parameters.Niu2, layer.N);
        }

        public void FillSchemeParameters(Substance substance, Layer layer, double[] s, double[] f)
        {
            var parameters = substance.ImplicitScheme;



            //if (layer.FirstLayer)
            //{
            //    parameters.Y0 = 1;
            //    parameters.Niu1 = Biosensor.S0;
            //} 
            //else
            //{
            //parameters.Y0 = 1;
            //parameters.U0 = s[layer.LowerBondIndex];

            //parameters.Niu1 = layer.H * parameters.Y0 * parameters.U0 / (1 + layer.H * parameters.Y0);
            //}

            //if (layer.LastLayer)
            //{
            //parameters.Y1 = 1;
            //parameters.Niu2 = 0;
            //}
            //else
            //{
            //parameters.Y1 = 1;
            //parameters.U1 = s[layer.UpperBondIndex];

            //parameters.Niu2 = layer.H * parameters.Y1 * parameters.U1 / (layer.H * parameters.Y1 - 1);
            //}

            //parameters.Beta1 = 1 / (1 + layer.H * parameters.Y0);
            //parameters.Beta2 = 1 / (1 - layer.H * parameters.Y1);


            //parameters.Beta1 =
            //(layer.R * layer.Substrate.DiffusionCoefficient / 2) / 1 + t * //(DsOverhh + Vmax / (Km + Sprev[1]));

            //parameters.A[layer.N - 1] = -parameters.Beta2;
            //parameters.B[0] = -parameters.Beta1;

            parameters.F = f;
            parameters.F[0] = parameters.Niu1;
            parameters.F[layer.N - 1] = parameters.Niu2;
        }
    }
}
