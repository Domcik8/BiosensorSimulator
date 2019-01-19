using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Parameters.Scheme
{
    public struct ImplicitSchemeParameters
    {
        public double[] A { get; set; }
        public double[] B { get; set; }
        public double[] C { get; set; }

        public double[] F { get; set; }

        public double[] D { get; set; }
        public double[] E { get; set; }

        public double Beta1 { get; set; }
        public double Beta2 { get; set; }

        public ImplicitSchemeParameters(Layer layer, Substance substance)
        {
            var a = substance.DiffusionCoefficient * layer.R;
            var c = 1 + 2 * a;
            var n = layer.N;

            A = new double[n];
            B = new double[n];
            C = new double[n];
            D = new double[n];
            E = new double[n];
            F = new double[n];

            for (var i = 1; i < n - 1; i++)
            {
                A[i] = B[i] = a;
                C[i] = c;
            }

            A[0] = 0;
            C[0] = 1;
            B[n - 1] = 0;
            C[n - 1] = 1;

            Beta1 = Beta2 = 0;
        }
    }
}