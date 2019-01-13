using BiosensorSimulator.Parameters.Biosensors;
using BiosensorSimulator.Parameters.Simulations;

namespace BiosensorSimulator.Parameters.Scheme
{
    public struct ImplicitSchemeParameters
    {
        public double[] A { get; set; }
        public double[] B { get; set; }
        public double[] C { get; set; }
        public double[] S { get; set; }

        public ImplicitSchemeParameters(Layer layer, Substance substance, double timeStep)
        {
            var a = substance.DiffusionCoefficient * layer.R;
            var b = a;
            var c = 1 + 2 * a;
            //var f = layer.Type == LayerType.DiffusionLayer ? 0 : -timeStep * reaction;
            
            var n = layer.N;

            A = new double[n];
            B = new double[n];
            C = new double[n];
            S = new double[n];

            for (var i = 0; i < n; i++)
            {
                A[i] = a;
                B[i] = b;
                C[i] = c;
            }

            A[0] = 0;
            C[0] = 1;
            B[n - 1] = 0;
            C[n - 1] = 1;
        }
    }
}