﻿using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Simulations
{
    public interface ISimulationParametersSuplier
    {
        SimulationParameters InitiationParameters(Biosensor biosensor);
    }
}
