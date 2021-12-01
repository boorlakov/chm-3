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
    public readonly double[] Al;

    /// <summary>
    ///     au is for elements of upper triangular part of matrix
    /// </summary>
    public readonly double[] Au;

    /// di is for diagonal elements
    public readonly double[] Di;

    /// <summary>
    ///     ia is for profile matrix. i.e. by manipulating ia elements we can use our matrix
    ///     much more time and memory efficient
    /// </summary>
    public readonly int[] Ia;

    public Matrix()
    {
        Di = Array.Empty<double>();
        Au = Array.Empty<double>();
        Al = Array.Empty<double>();
        Ia = Array.Empty<int>();
        Decomposed = false;
    }

    public Matrix(int size, double[] di, int[] ia, double[] au, double[] al)
    {
        Size = size;
        Di = di ?? throw new ArgumentNullException(nameof(di));
        Ia = ia ?? throw new ArgumentNullException(nameof(ia));
        Au = au ?? throw new ArgumentNullException(nameof(au));
        Al = al ?? throw new ArgumentNullException(nameof(al));
        Decomposed = false;
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
    public void ToLU()
    {
        for (var i = 1; i < Size; i++)
        {
            var sumDi = 0.0;
            var j0 = i - (Ia[i + 1] - Ia[i]);

            for (var ii = Ia[i] - 1; ii < Ia[i + 1] - 1; ii++)
            {
                var j = ii - Ia[i] + j0 + 1;
                var jBeg = Ia[j] - 1;
                var jEnd = Ia[j + 1] - 1;

                if (jBeg < jEnd)
                {
                    var j0J = j - (jEnd - jBeg);
                    var jjBeg = Max(j0, j0J);
                    var jjEnd = Min(j, i - 1);
                    var cL = 0.0;

                    for (var k = 0; k <= jjEnd - jjBeg - 1; k++)
                    {
                        var indAu = Ia[j] + jjBeg - j0J + k - 1;
                        var indAl = Ia[i] + jjBeg - j0 + k - 1;

                        cL += Au[indAu] * Al[indAl];
                    }

                    Al[ii] -= cL;
                    var cU = 0.0;

                    for (var k = 0; k <= jjEnd - jjBeg - 1; k++)
                    {
                        var indAl = Ia[j] + jjBeg - j0J + k - 1;
                        var indAu = Ia[i] + jjBeg - j0 + k - 1;

                        cU += Au[indAu] * Al[indAl];
                    }

                    Au[ii] -= cU;
                }

                if (Di[j] == 0.0)
                {
                    throw new DivideByZeroException($"No dividing by zero. DEBUG INFO: [i:{i}; j:{j}]");
                }

                Au[ii] /= Di[j];
                sumDi += Al[ii] * Au[ii];
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
    /// <exception cref="FieldAccessException"> If matrix is not decomposed </exception>
    /// <returns></returns>
    [Obsolete("Accessing data this way is not efficient and must be removed")]
    public double U(int i, int j)
    {
        if (!Decomposed)
        {
            throw new FieldAccessException("Matrix must be decomposed.");
        }

        return i <= j ? this[i, j] : 0.0;
    }

    /// <summary>
    ///     l[i][j] of Lower triangular matrix L
    /// </summary>
    /// <param name="i"> rows </param>
    /// <param name="j"> columns</param>
    /// <exception cref="FieldAccessException"> If matrix is not decomposed </exception>
    /// <returns></returns>
    [Obsolete("Accessing data this way is not efficient and must be removed")]
    public double L(int i, int j)
    {
        if (!Decomposed)
        {
            throw new FieldAccessException("Matrix must be decomposed.");
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
            return j + 1 <= i - (Ia[i + 1] - Ia[i]) ? 0.0 : Al[Ia[i + 1] + j - 1 - i];
        }

        return i + 1 <= j - (Ia[j + 1] - Ia[j]) ? 0.0 : Au[Ia[j + 1] + i - j - 1];
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
                if (!(j <= i - (Ia[i + 1] - Ia[i]) + 1))
                {
                    Al[Ia[i + 1] + j - 1 - i] = value;
                }
            }
            else
            {
                if (!(i < j - (Ia[j + 1] - Ia[j]) + 1))
                {
                    Au[Ia[j + 1] + i - j - 1] = value;
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

        foreach (var item in Ia)
        {
            sb.Append($"{item:G15} ");
        }

        sb.Append("\nau:\n");

        foreach (var item in Au)
        {
            sb.Append($"{item:G15} ");
        }

        sb.Append("\nal:\n");

        foreach (var item in Al)
        {
            sb.Append($"{item:G15} ");
        }

        return sb.ToString();
    }
}