using BiosensorSimulator.Parameters.Scheme;
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

        public bool IsHomogenized { get; set; } = false;
        public bool UseEffectiveDiffusionCoefficent { get; set; } = false;
        public bool UseEffectiveReactionCoefficent { get; set; } = false;

        public double EffectiveDiffusionCoefficent { get; set; } = 1;
        public double EffectiveReactionCoefficent { get; set; } = 1;


        public double MicroReactorRadius { get; set; }
        public double UnitRadius { get; set; }

        /// <summary>
        /// Full biosensor height
        /// </summary>
        public double Height { get; set; }
        
        public Layer EnzymeLayer => Layers.First(l => l.Type == LayerType.Enzyme);
        public Layer DiffusionLayer => Layers.First(l => l.Type == LayerType.DiffusionLayer);
    }
}