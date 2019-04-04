using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.Base.Layers
{
    public class Area
    {
        public LayerType Type { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        /// <summary>
        /// Quantity of layer space steps in axis Y
        /// </summary>
        public long N { get; set; }

        /// <summary>
        /// Quantity of layer space steps in axis X
        /// </summary>
        public long M { get; set; }

        /// <summary>
        /// Layer space step in axis Y
        /// </summary>
        public double H { get; set; }

        /// <summary>
        /// Layer space step in axis X
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Layer upper bond index
        /// </summary>
        public long UpperBondIndex { get; set; }

        /// <summary>
        /// Layer lower bond index
        /// </summary>
        public long LowerBondIndex { get; set; }

        /// <summary>
        /// Area left bond index
        /// </summary>
        public long LeftBondIndex { get; set; }

        /// <summary>
        /// Area right bond index
        /// </summary>
        public long RightBondIndex { get; set; }

        /// <summary>
        /// Time step over square space step
        /// </summary>
        public double R { get; set; }

        public Product Product { get; set; }

        public Substrate Substrate { get; set; }
    }
}
