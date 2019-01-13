using System.Collections.Generic;
using System.Linq;

namespace BiosensorSimulator.Parameters.Biosensors
{
    public class Biosensor
    {
        public double S0 { get; set; }
        public double P0 { get; set; }

        public double VMax { get; set; }
        public double Km { get; set; }

        public List<Layer> Layers { get; set; }
        public List<Bound> Bounds { get; set; }

        public double MicroReactorRadius { get; set; }
        public double UnitRadius { get; set; }

        /// <summary>
        /// Full biosensor height
        /// </summary>
        public double BiosensorHeight { get; set; }

        /// <summary>
        /// BiosensorHeight - ferment layer height
        /// </summary>
        public double NerstLayerHeight { get; set; }

        public Layer EnzymeLayer => Layers.First(l => l.Type == LayerType.Enzyme);
    }

    public class Layer
    {
        public LayerType Type { get; set; }

        public double Height { get; set; }

        public long N { get; set; }

        public double H { get; set; }

        /// <summary>
        /// Time step over square space step
        /// </summary>
        public double R { get; set; }

        public Product Product { get; set; }

        public Substrate Substrate { get; set; }
    }

    public class Substance
    {
        public SubstanceType Type { get; set; }

        public double DiffusionCoefficient { get; set; }

        public double StartConcentration { get; set; }

        public int ReactionRate { get; set; }

        /// <summary>
        /// DiffusionCoefficient over R
        /// </summary>
        public double DiffusionCoefficientOverR { get; set; }

        /// <summary>
        /// Diffusion coefficient over space
        /// </summary>
        public double DiffusionCoefficientOverSpace { get; set; }
    }

    public class Product : Substance { }

    public class Substrate : Substance { }

    public class Bound
    {
        public BoundType Type { get; set; }

        public double StartConcentration { get; set; }

    }

    public enum LayerType
    {
        SelectiveMembrane,
        Enzyme,
        PerforatedMembrane,
        DiffusionLayer
    }

    public enum SubstanceType
    {
        Product,
        Substrate
    }

    public enum BoundType
    {
        Start,
        End
    }
}