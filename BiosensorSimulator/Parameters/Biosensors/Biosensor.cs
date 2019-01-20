using System.Collections.Generic;
using System.Linq;
using BiosensorSimulator.Parameters.Scheme;

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
        /// <summary>
        /// Layer type
        /// </summary>
        public LayerType Type { get; set; }

        /// <summary>
        /// Layer height
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Quantity of layer space steps
        /// </summary>
        public long N { get; set; }

        /// <summary>
        /// Layer space step
        /// </summary>
        public double H { get; set; }

        /// <summary>
        /// Layer upper bond index
        /// </summary>
        public long UpperBondIndex { get; set; }

        /// <summary>
        /// Layer lower bond index
        /// </summary>
        public long LowerBondIndex { get; set; }

        /// <summary>
        /// Time step over square space step
        /// </summary>
        public double R { get; set; }

        public Product Product { get; set; }

        public Substrate Substrate { get; set; }

        public bool FirstLayer { get; set; } = false;
        public bool LastLayer { get; set; } = false;
    }

    public class Substance
    {
        public SubstanceType Type { get; set; }

        public double DiffusionCoefficient { get; set; }

        public double StartConcentration { get; set; }

        public int ReactionRate { get; set; }

        public ExplicitSchemeParameters ExplicitScheme { get; set; }

        public ImplicitSchemeParameters ImplicitScheme { get; set; }
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