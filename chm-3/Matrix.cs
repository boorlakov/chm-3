using static System.Math;

namespace chm_3;

/// <summary>
///     Matrix as in math. Data is stored in sparse format
/// </summary>
public class Matrix
{
    /// <summary>
    ///     al is for elements of lower triangular part of matrix
    /// </summary>
    /// <returns></returns>
    public readonly double[] Ggl;

    /// <summary>
    ///     au is for elements of upper triangular part of matrix
    /// </summary>
    public readonly double[] Ggu;

    /// di is for diagonal elements
    public readonly double[] Di;

    /// <summary>
    ///     ia is for profile matrix. i.e. by manipulating ia elements we can use our matrix
    ///     much more time and memory efficient
    /// </summary>
    public readonly int[] Ig;

    /// <summary>
    ///     Ja is for profile matrix. i.e. by manipulating ia elements we can use our matrix
    ///     much more time and memory efficient
    /// </summary>
    public readonly int[] Jg;

    public Matrix()
    {
        Ggl = default!;
        Ggu = default!;
        Di = default!;
        Ig = default!;
        Jg = default!;
        Decomposed = default!;
        Size = default!;
    }

    public Matrix(double[] ggl, double[] ggu, double[] di, int[] ig, int[] jg, int size, bool decomposed)
    {
        Ggl = ggl;
        Ggu = ggu;
        Di = di;
        Ig = ig;
        Jg = jg;
        Size = size;
        Decomposed = decomposed;
    }

    /// <summary>
    /// Stores info about LU decomposed or not
    /// </summary>
    public bool Decomposed { get; private set; }

    public int Size { get; }

    /// <summary>
    /// LU(sq)-decomposition with value=1 in diagonal elements of U matrix.
    /// Corrupts base object. To access data as one matrix you need to build it from L and U.
    /// </summary>
    /// <exception cref="DivideByZeroException"> If diagonal element is zero </exception>
    public void Factorize()
    {
        for (var i = 0; i < Size; i++)
        {
            var sumDi = 0.0;

            var i0 = Ig[i];
            var i1 = Ig[i + 1];

            for (var k = i0; k < i1; k++)
            {
                var j = Jg[k];
                var j0 = Ig[j];
                var j1 = Ig[j + 1];

                var iK = i0;
                var kJ = j0;

                var sumL = 0.0;
                var sumU = 0.0;

                while (iK < k && kJ < j1)
                {
                    if (Jg[iK] == Jg[kJ])
                    {
                        sumL += Ggl[iK] * Ggu[kJ];
                        sumU += Ggu[iK] * Ggl[kJ];
                        iK++;
                        kJ++;
                    }
                    else
                    {
                        if (Jg[iK] > Jg[kJ])
                        {
                            kJ++;
                        }
                        else
                        {
                            iK++;
                        }
                    }
                }

                if (Di[j] == 0.0)
                {
                    throw new DivideByZeroException($"Di[{j}] has thrown at pos {i} {j}");
                }
                Ggl[k] = (Ggl[k] - sumL) / Di[j];
                Ggu[k] = (Ggu[k] - sumU) / Di[j];
                
                sumDi += Ggl[k] * Ggu[k];
            }

            Di[i] = Sqrt(Di[i] - sumDi);
        }

        Decomposed = true;
    }
}