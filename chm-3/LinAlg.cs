namespace chm_3;

public abstract class LinAlg
{
    /// <summary>
    ///     Matrix multiplication by vector
    /// </summary>
    /// <param name="m"> Matrix, stored in profile format</param>
    /// <param name="b"> Vector, that multiplies matrix </param>
    /// <returns> Resulting vector of multiplication </returns>
    public static double[] MatMul(Matrix m, double[] b)
    {
        var res = new double[b.Length];
        
        // Code goes here...

        return res;
    }

    // TODO: Write code to solve
    // TODO: Write tests
    /// <summary>
    ///     3 steps iteration method to solve linear algebraic equations
    /// </summary>
    /// <param name="m"></param>
    /// <param name="b"></param>
    /// <param name="preCondNeed"> </param>
    /// <returns> Vector x, that is solution for Ax = b equation </returns>
    /// <exception cref="NotSupportedException"> If matrix isn't decomposed</exception>
    public static double[] Solve(Matrix m, double[] b, bool preCondNeed)
    {
        if (!m.Decomposed)
        {
            throw new NotDecomposedException();
        }

        return preCondNeed ? SolveWithPreCond(m, b) : SolveWithNoPreCond(m, b);
    }

    // TODO: Write code to solve
    // TODO: Write tests
    private static double[] SolveWithPreCond(Matrix m, double[] b)
    {
        var res = new double[b.Length];

        m.PreCond();

        // Your code goes here...

        return res;
    }

    // TODO: Write code to solve
    // TODO: Write tests
    private static double[] SolveWithNoPreCond(Matrix m, double[] b)
    {
        var res = new double[b.Length];

        // Your code goes here...

        return res;
    }
}