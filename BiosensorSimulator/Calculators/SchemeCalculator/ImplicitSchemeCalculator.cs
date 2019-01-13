using System;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Calculators.SchemeCalculator
{
    public class ImplicitSchemeCalculator : ISchemeCalculator
    {
        public void CalculateNextStep(
            double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            Biosensor biosensor, SimulationParameters simulationParameters)
        {
            /*Copy(SPrev, SCur);
            Copy(PPrev, PCur);

            double DsOverhh = Biosensor.Ds / (Biosensor.hd * Biosensor.hd);
            double DpOverhh = Biosensor.Dp / (Biosensor.hd * Biosensor.hd);

            for (int i = 1; i < Biosensor.N; i++) // 29
            {
                double commonMember = Biosensor.Vmax * SPrev[i] / (Biosensor.Km + SPrev[i]);
                SCur[i] = Biosensor.t * (DsOverhh * (SPrev[i + 1] - 2 * SPrev[i] + SPrev[i - 1]) - commonMember) + SPrev[i];
                PCur[i] = Biosensor.t * (DpOverhh * (PPrev[i + 1] - 2 * PPrev[i] + PPrev[i - 1]) + commonMember) + PPrev[i];
            }

            SCur[Biosensor.N] = Biosensor.S0; //33
            SCur[0] = Scur[1]; //33
            PCur[Biosensor.N] = Biosensor.P0; //34
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

            double h2 = (Biosensor.hd * Biosensor.hd);
            double r = Biosensor.t / h2;
            double rSd = r * (Biosensor.DSd);
            double rSf = r * (Biosensor.DSf - (x / h2));
            double rPd = r * (Biosensor.DPd);
            double rPf = r * (Biosensor.DPf );*/

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
