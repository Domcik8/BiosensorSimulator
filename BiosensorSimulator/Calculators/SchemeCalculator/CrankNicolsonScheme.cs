using BiosensorSimulator.Parameters.Simulations;
using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Scheme;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class CrankNicolsonScheme : ISchemeCalculator
    {
        public Biosensor Biosensor { get; }
        public SimulationParameters SimulationParameters { get; }

        public CrankNicolsonScheme(Biosensor biosensor, SimulationParameters simulationParameters)
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
            var t = SimulationParameters.t;
            var Scur = sCur;
            var Sprev = sPrev;
            var Pcur = pCur;
            var Pprev = pPrev;

            var N = layer.N;
            double[] aS = new double[N];
            double[] bS = new double[N];
            double[] cS = new double[N];
            double[] fS = new double[N];

            double[] aP = new double[N];
            double[] bP = new double[N];
            double[] cP = new double[N];
            double[] fP = new double[N];
            double[] d = new double[N];
            double[] e = new double[N];

            double DsOverhh = layer.Substrate.DiffusionCoefficient / (layer.H * layer.H);
            double DpOverhh = layer.Product.DiffusionCoefficient / (layer.H * layer.H);

            Scur[N - 1] = Biosensor.S0;
            Pcur[N - 1] = Biosensor.P0;

            double abS = t * DsOverhh / 2;
            double abP = t * DpOverhh / 2;
            double c = 1 + 2 * abP;

            for (var i = 0; i < N - 1; i++)
            {
                aS[i] = abS;
                bS[i] = abS;
                aP[i] = abP;
                bP[i] = abP;
                cP[i] = c;
            }
            
            double cSfirst, cSlast, cPfirst, cPlast, fSfirst, fSlast,
                fPfirst, fPlast, beta1S, u1S, beta2S, u2S, beta1P, u1P, beta2P, u2P;
            var Vmax = Biosensor.VMax;
            var Km = Biosensor.Km;
            
            for (var i = 1; i < N - 1; i++)
            {
                cS[i] = 1 + t * (DsOverhh + Vmax / (Km + Sprev[i]));
                fS[i] = -Sprev[i] - t * DsOverhh * (Sprev[i + 1] - 2 * Sprev[i] + Sprev[i - 1]) / 2;
            }

            cSfirst = 1 + t * (DsOverhh + Vmax / (Km + Sprev[1]));
            cSlast = 1 + t * (DsOverhh + Vmax / (Km + Sprev[N - 1]));

            fSfirst = -Sprev[1] - t * DsOverhh * (Sprev[2] - 2 * Sprev[1] + Sprev[0]) / 2;
            fSlast = -Sprev[N - 2] - t * DsOverhh * (Sprev[N - 1] - 2 * Sprev[N - 2] + Sprev[N - 3] + Scur[N - 1]) / 2;

            beta1S = abS / cSfirst; u1S = -fSfirst / cSfirst;
            beta2S = abS / cSlast; u2S = -fSlast / cSlast;

            MatrixCalculator.SolveTriagonalLinearSystem(aS, bS, cS, d, e, fS, Scur, beta1S, beta2S, u1S, u2S, N);

            for (int i = 1; i < N - 1; i++)
            {
                fP[i] = -Pprev[i] - t * (DpOverhh * (Pprev[i + 1] - 2 * Pprev[i] + Pprev[i - 1]) / 2 + Vmax * Scur[i] / (Km + Sprev[i]));
            }

            cPfirst = c;
            cPlast = c;

            fPfirst = -Pprev[1] - t * (DpOverhh * (Pprev[2] - 2 * Pprev[1] + Pprev[0]) / 2 + Vmax * Scur[1] / (Km + Sprev[1]));
            fPlast = -Pprev[N - 2] - t * (DpOverhh * (Pprev[N - 1] - 2 * Pprev[N - 2] + Pprev[N - 3]) / 2 + Vmax * Scur[N - 2] / (Km + Sprev[N - 2]));

            beta1P = abP / cPfirst; u1P = -fPfirst / cPfirst;
            beta2P = abP / cPlast; u2P = -fPlast / cPlast;

            MatrixCalculator.SolveTriagonalLinearSystem(aP, bP, cP, d, e, fP, Pcur, beta1P, beta2P, u1P, u2P, N);

            Scur[0] = Scur[1];
            Pcur[0] = 0;
        }
    }
}
