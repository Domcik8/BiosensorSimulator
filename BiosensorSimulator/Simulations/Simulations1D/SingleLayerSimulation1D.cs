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
            BiosensorParameters biosensorParameters,
            ISchemeCalculator schemeCalculator
            ) : base(simulationParameters, biosensorParameters, schemeCalculator) { }

        public void ShowValidationValues(BiosensorParameters biosensorParameters, SimulationParameters simulationParameters)
        {
            var simulation = new AnaliticSimulation();
            var firstOrderCurrent = simulation.GetFirstOrderAnaliticSolution(biosensorParameters, simulationParameters);
            var zeroOrderCurrent = simulation.GetZeroOrderAnaliticSolution(biosensorParameters, simulationParameters);

            Console.WriteLine($"First order current : {firstOrderCurrent} A/mm^2");
            Console.WriteLine($"Zero order current : {zeroOrderCurrent} A/mm^2");
        }

        public override void CalculateNextStep()
        {
            Array.Copy(SCur, SPrev, SCur.Length);
            Array.Copy(PCur, PPrev, PCur.Length);

            foreach (var layer in BiosensorParameters.Layers)
            {
                SchemeCalculator.CalculateNextStep(layer, SCur, PCur, SPrev, PPrev);
            }

            SetBoundaryConditions();
        }

        public void SetBoundaryConditions()
        {
            SCur[SimulationParameters.N] = BiosensorParameters.S0;
            SCur[0] = SCur[1];

            PCur[SimulationParameters.N] = BiosensorParameters.P0;
            PCur[0] = 0;
        }
    }
}