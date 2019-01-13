using BiosensorSimulator.Parameters.Biosensors;

namespace BiosensorSimulator.Parameters.Scheme
{
    public struct ImplicitSchemeParameters
    {
        public double A { get; set; }

        public double B { get; set; }

        public double C { get; set; }

        public double Betha1 { get; set; }
        public double Betha2 { get; set; }

        public ImplicitSchemeParameters(Layer layer, Substance substance)
        {
           /* A = substance.DiffusionCoefficient * layer.R;
            B = A;
            C = 1 + 2 * A;
            Betha1 = 1 / (1 + layer.H * substance.StartConcentration);
            Betha2 = 1 / (1 - layer.H * layer.EndConcentration);*/

            A = B = C = Betha1 = Betha2 = 0;
        }
    }
}