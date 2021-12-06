using System.Text;
using static System.Math;

namespace chm_3;

/// <summary>
///     Matrix as in math. Data is stored in profile format
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

    public Matrix(double[] ggl, double[] ggu, double[] di, int[] ig, int[] jg, int size)
    {
        Ggl = ggl;
        Ggu = ggu;
        Di = di;
        Ig = ig;
        Jg = jg;
        Decomposed = false;
        Size = size;
    }

    /// <summary>
    ///     Stores info about LU decomposed or not
    /// </summary>
    public bool Decomposed { get; private set; }

    /// <summary>
    ///     Warning: accessing the data in that way is not fast
    /// </summary>
    /// <param name="i"> row </param>
    /// <param name="j"> column </param>
    [Obsolete("Accessing data this way is not efficient and must be removed")]
    public double this[int i, int j]
    {
        get => GetElement(i, j);
        set => SetElement(i, j, value);
    }

    public int Size { get; }

    // TODO: Add code to method
    // TODO: Add documentation
    public void PreCond()
    {
    }

    /// <summary>
    ///     LU-decomposition with value=1 in diagonal elements of U matrix.
    ///     Corrupts base object. To access data as one matrix you need to build it from L and U.
    /// </summary>
    /// <exception cref="DivideByZeroException"> If diagonal element is zero </exception>
    // TODO: Fix it to LU(sq)
    [Obsolete("This is LU. Need to fix it to LU(sq)")]
    public void Factorize()
    {
        for (var i = 1; i < Size; i++)
        {
            var sumDi = 0.0;
            var j0 = i - (Ig[i + 1] - Ig[i]);

            for (var ii = Ig[i] - 1; ii < Ig[i + 1] - 1; ii++)
            {
                var j = ii - Ig[i] + j0 + 1;
                var jBeg = Ig[j] - 1;
                var jEnd = Ig[j + 1] - 1;

                if (jBeg < jEnd)
                {
                    var j0J = j - (jEnd - jBeg);
                    var jjBeg = Max(j0, j0J);
                    var jjEnd = Min(j, i - 1);
                    var cL = 0.0;

                    for (var k = 0; k <= jjEnd - jjBeg - 1; k++)
                    {
                        var indAu = Ig[j] + jjBeg - j0J + k - 1;
                        var indAl = Ig[i] + jjBeg - j0 + k - 1;

                        cL += Ggu[indAu] * Ggl[indAl];
                    }

                    Ggl[ii] -= cL;
                    var cU = 0.0;

                    for (var k = 0; k <= jjEnd - jjBeg - 1; k++)
                    {
                        var indAl = Ig[j] + jjBeg - j0J + k - 1;
                        var indAu = Ig[i] + jjBeg - j0 + k - 1;

                        cU += Ggu[indAu] * Ggl[indAl];
                    }

                    Ggu[ii] -= cU;
                }

                if (Di[j] == 0.0)
                {
                    throw new DivideByZeroException($"No dividing by zero. DEBUG INFO: [i:{i}; j:{j}]");
                }

                Ggu[ii] /= Di[j];
                sumDi += Ggl[ii] * Ggu[ii];
            }

            Di[i] -= sumDi;
        }

        Decomposed = true;
    }

    /// <summary>
    ///     Was made for debugging LU-decomposition.
    /// </summary>
    /// <returns></returns>
    public void CheckDecomposition()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nLU-check:");

        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                var c = 0.0;

                for (var k = 0; k < Size; k++)
                {
                    c += L(i, k) * U(k, j);
                }

                Console.Write($"{c:G15} ");
            }

            Console.WriteLine();
        }

        Console.ResetColor();
    }

    /// <summary>
    ///     u[i][j] of Upper triangular matrix U
    /// </summary>
    /// <param name="i"> rows </param>
    /// <param name="j"> columns</param>
    /// <exception cref="NotDecomposedException"> If matrix is not decomposed </exception>
    /// <returns></returns>
    [Obsolete("Accessing data this way is not efficient and must be removed")]
    public double U(int i, int j)
    {
        if (!Decomposed)
        {
            throw new NotDecomposedException();
        }

        return i <= j ? this[i, j] : 0.0;
    }

    /// <summary>
    ///     l[i][j] of Lower triangular matrix L
    /// </summary>
    /// <param name="i"> rows </param>
    /// <param name="j"> columns</param>
    /// <exception cref="NotDecomposedException"> If matrix is not decomposed </exception>
    /// <returns></returns>
    [Obsolete("Accessing data this way is not efficient and must be removed")]
    public double L(int i, int j)
    {
        if (!Decomposed)
        {
            throw new NotDecomposedException();
        }

        return i >= j ? this[i, j] : 0.0;
    }

    /// <summary>
    ///     WARNING: Accessing data this way is not efficient
    ///     Because of profile format we need to refer A[i][j] special way. 
    ///     We have that method for accessing data more naturally.    
    /// </summary>
    /// <param name="i"> rows </param>
    /// <param name="j"> columns </param>
    /// <returns></returns>
    [Obsolete("Accessing data this way is not efficient and must be removed")]
    private double GetElement(int i, int j)
    {
        if (i == j)
        {
            return Di[i];
        }

        if (i > j)
        {
            return j + 1 <= i - (Ig[i + 1] - Ig[i]) ? 0.0 : Ggl[Ig[i + 1] + j - 1 - i];
        }

        return i + 1 <= j - (Ig[j + 1] - Ig[j]) ? 0.0 : Ggu[Ig[j + 1] + i - j - 1];
    }

    [Obsolete("Accessing data this way is not efficient and must be removed")]
    void SetElement(int i, int j, double value)
    {
        if (i == j)
        {
            Di[i] = value;
        }
        else
        {
            if (i > j)
            {
                if (!(j <= i - (Ig[i + 1] - Ig[i]) + 1))
                {
                    Ggl[Ig[i + 1] + j - 1 - i] = value;
                }
            }
            else
            {
                if (!(i < j - (Ig[j + 1] - Ig[j]) + 1))
                {
                    Ggu[Ig[j + 1] + i - j - 1] = value;
                }
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"{nameof(Matrix)}:\ndi:\n");

        foreach (var item in Di)
        {
            sb.Append($"{item:G15} ");
        }

        sb.Append("\nia:\n");

        foreach (var item in Ig)
        {
            sb.Append($"{item:G15} ");
        }

        sb.Append("\nau:\n");

        foreach (var item in Ggu)
        {
            sb.Append($"{item:G15} ");
        }

        sb.Append("\nal:\n");

        foreach (var item in Ggl)
        {
            sb.Append($"{item:G15} ");
        }

        return sb.ToString();
    }
}