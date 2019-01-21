namespace BiosensorSimulator.Calculators
{
    public static class MatrixCalculator
    {
        /// <summary>
        /// Tridiagonal solution example: 
        ///
        /// Matrix matrix = new Matrix();
        /// var a = new double[] { 0, -1, -1};
        /// var b = new double[] { 3, 3, 3 };
        /// var c = new double[] { -1, -1, 0 };
        /// var r = new double[] {-1, 7, 7};
        /// 
        /// matrix.SolveTridiagonalInPlace(a, b, c, r, b.Length);
        /// </summary>
        public static double[] SolveTridiagonalInPlace(
            double[] a, double[] b, double[] c, double[] x, long X)
        {
            /*
                solves Ax = v where A is a tridiagonal matrix consisting of vectors a, b, c
                x - initially contains the input vector v, and returns the solution x. indexed from 0 to X - 1 inclusive
                X - number of equations (length of vector x)
                a - subdiagonal (means it is the diagonal below the main diagonal), indexed from 1 to X - 1 inclusive
                b - the main diagonal, indexed from 0 to X - 1 inclusive
                c - superdiagonal (means it is the diagonal above the main diagonal), indexed from 0 to X - 2 inclusive

                Note: contents of input vector c will be modified, making this a one-time-use function (scratch space can be allocated instead for this purpose to make it reusable)
                Note 2: We don't check for diagonal dominance, etc.; this is not guaranteed stable
                */

            c[0] = c[0] / b[0];
            x[0] = x[0] / b[0];

            /* loop from 1 to X - 1 inclusive, performing the forward sweep */
            for (long ix = 1; ix < X; ix++)
            {
                var m = 1.0 / (b[ix] - a[ix] * c[ix - 1]);
                c[ix] = c[ix] * m;
                x[ix] = (x[ix] - a[ix] * x[ix - 1]) * m;
            }

            /* loop from X - 2 to 0 inclusive (safely testing loop condition for an unsigned integer), to perform the back substitution */
            for (var ix = X - 2; ix > -1; ix--)
                x[ix] -= c[ix] * x[ix + 1];

            return x;
        }

        public static double[] SolveTriagonalLinearSystem(
            double[] a, double[] b, double[] c,
            double[] d, double[] e, double[] f,
            double[] s,
            double beta1, double beta2,
            double u1, double u2, long n)
        {
            d[1] = b[0] / (c[0]);
            e[1] = (f[0]) / (c[0]);
            //d[1] = b[0] / (c[0] - a[0] * beta1);
            //e[1] = (a[0] * u1 - f[0]) / (c[0] - a[0] * beta1);

            for (var i = 1; i < n - 1; i++)
            {
                var nextIndex = i + 1;
                var denominator = c[i] - d[i] * a[i];
                d[nextIndex] = b[i] / denominator;
                e[nextIndex] = (a[i] * e[i] - f[i]) / denominator;
            }

            s[n - 1] = (u2 + beta2 * e[n - 1]) / (1 - d[n - 1] * beta2);

            for (var i = n - 2; i >= 0; i--)
                s[i] = d[i + 1] * s[i + 1] + e[i + 1];

            s[0] = beta1 * s[1] + u1;

            return s;
        }
    }
}
