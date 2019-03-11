using System.Collections.Generic;
using System.Linq;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers;
using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.Base
{
    public abstract class BaseBiosensor
    {
        public string Name { get; set; }

        public double S0 { get; set; }
        public double P0 { get; set; }

        public double VMax { get; set; }
        public double Km { get; set; }

        public List<Layer> Layers { get; set; }
        public List<Bound> Bounds { get; set; }

        /// <summary>
        /// Full biosensor height
        /// </summary>
        public double Height { get; set; }

        public Layer NonHomogenousLayer => Layers.First(l => l.Type == LayerType.NonHomogenousLayer);
        public Layer EnzymeLayer => Layers.First(l => l.Type == LayerType.Enzyme);
        public Layer DiffusionLayer => Layers.First(l => l.Type == LayerType.DiffusionLayer);
        public Layer PerforatedMembraneLayer => Layers.First(l => l.Type == LayerType.PerforatedMembrane);
    }
}