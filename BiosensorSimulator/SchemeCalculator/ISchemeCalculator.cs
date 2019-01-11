using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.SchemeCalculator
{
    public interface ISchemeCalculator
    {
        void CalculateNextStep(double[] sCur, double[] pCur, double[] sPrev, double[] pPrev,
            BiosensorParameters biosensorParameters, SimulationParameters simulationParameters);
    }
}
