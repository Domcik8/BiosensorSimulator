using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public void CalculateNextStep(
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            /*Copy(SPrev, SCur);
            Copy(PPrev, PCur);

            double DsOverhh = BiosensorParameters.Ds / (BiosensorParameters.hd * BiosensorParameters.hd);
            double DpOverhh = BiosensorParameters.Dp / (BiosensorParameters.hd * BiosensorParameters.hd);

            for (int i = 1; i < BiosensorParameters.N; i++) // 29
            {
                double commonMember = BiosensorParameters.Vmax * SPrev[i] / (BiosensorParameters.Km + SPrev[i]);
                SCur[i] = BiosensorParameters.t * (DsOverhh * (SPrev[i + 1] - 2 * SPrev[i] + SPrev[i - 1]) - commonMember) + SPrev[i];
                PCur[i] = BiosensorParameters.t * (DpOverhh * (PPrev[i + 1] - 2 * PPrev[i] + PPrev[i - 1]) + commonMember) + PPrev[i];
            }

            SCur[BiosensorParameters.N] = BiosensorParameters.S0; //33
            SCur[0] = Scur[1]; //33
            PCur[BiosensorParameters.N] = BiosensorParameters.P0; //34
            PCur[0] = 0; //Pcur[0] = Pcur[1];//34*/

            throw new NotImplementedException();
        }

        public void Copy(double[] a, double[] b)
        {
            for (int i = 1; i < a.Length; i++)
                a[i] = b[i];
        }

        /*SPrev = SCur;
            PPrev = PPrev;

            double h2 = (BiosensorParameters.hd * BiosensorParameters.hd);
            double r = BiosensorParameters.t / h2;
            double rSd = r * (BiosensorParameters.DSd);
            double rSf = r * (BiosensorParameters.DSf - (x / h2));
            double rPd = r * (BiosensorParameters.DPd);
            double rPf = r * (BiosensorParameters.DPf );*/

        /*double[] a = new double[];
        double[] b = ;
        double[] c = ;*/
        
        public void CalculateDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }

        public void CalculateReactionDiffusionLayerNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }

        public void CalculateNextStep(Layer layer, double[] sCur, double[] pCur, double[] sPrev, double[] pPrev)
        {
            throw new NotImplementedException();
        }
    }
}
