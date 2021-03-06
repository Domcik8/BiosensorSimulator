﻿using BiosensorSimulator.Parameters.Biosensors.Base.Layers.Enums;

namespace BiosensorSimulator.Parameters.Biosensors.Base.Layers
{
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
}
