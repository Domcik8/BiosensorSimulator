using BiosensorSimulator.Calculators.SchemeCalculator;
using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;
using System;

namespace BiosensorSimulator.Simulations.Simulations1D
{
    public class SingleLayerSimulation1D : BaseSimulation
    {
        public SingleLayerSimulation1D(
            SimulationParameters simulationParameters,
            Biosensor biosensor,
            ISchemeCalculator schemeCalculator
            ) : base(simulationParameters, biosensor, schemeCalculator) { }

        public void ShowValidationValues(Biosensor biosensor, SimulationParameters simulationParameters)
        {
            var simulation = new AnaliticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnaliticSolution(biosensor, simulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnaliticSolution(biosensor, simulationParameters);

            Console.WriteLine($"First order current : {firstOrderCurrent} A/mm^2");
            Console.WriteLine($"Zero order current : {zeroOrderCurrent} A/mm^2");
        }

        public override void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);
            
            SchemeCalculator.CalculateNextStep(SCur, PCur, SPrev, PPrev);

            SetBoundaryConditions();
        }

        public void SetBoundaryConditions()
        {
            SCur[SimulationParameters.N - 1] = Biosensor.S0;
            SCur[0] = SCur[1];

            PCur[SimulationParameters.N - 1] = Biosensor.P0;
            PCur[0] = 0;
        }
    }
}