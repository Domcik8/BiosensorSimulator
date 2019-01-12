namespace BiosensorSimulator.Calculators
{
    public class Matrix
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
        public double[] SolveTridiagonalInPlace(
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
    }
}
