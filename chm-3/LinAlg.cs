using System.Data;

namespace chm_3;

public abstract class LinAlg
{
    /// <summary>
    /// Matrix multiplication by vector
    /// </summary>
    /// <param name="m"> Matrix, stored in sparse format</param>
    /// <param name="b"> Vector, that multiplies matrix </param>
    /// <returns> Resulting vector of multiplication </returns>
    public static double[] MatMul(Matrix m, double[] b)
    {
        if (m.Size != b.Length)
        {
            throw new EvaluateException($"[ERR] Different sizes. Matrix size = {m.Size}, vector size = {b.Length}");
        }

        var res = new double[b.Length];

        for (var i = 0; i < b.Length; i++)
        {
            res[i] = m.Di[i] * b[i];

            for (var j = m.Ig[i]; j < m.Ig[i + 1]; j++)
            {
                res[i] += m.Ggl[j] * b[m.Jg[j]];
                res[m.Jg[j]] += m.Ggu[j] * b[i];
            }
        }

        return res;
    }

    public static double Dot(double[] lhs, double[] rhs)
    {
        if (lhs.Length != rhs.Length)
        {
            throw new EvaluateException(
                $"[ERR] Vectors have different sizes. lhsLen = {lhs.Length}, rhsLen = {rhs.Length}");
        }

        var res = 0.0;

        for (var i = 0; i < lhs.Length; i++)
        {
            res += lhs[i] * rhs[i];
        }

        return res;
    }

    /// <summary>
    /// Iteration method with 3 steps that solves slae Ax = b.
    /// </summary>
    /// <param name="a"> Input matrix in sparse format </param>
    /// <param name="b"> Input vector of right hand side </param>
    /// <param name="x"> Initial approximation vector </param>
    /// <param name="eps"> Wanted ideal accuracy </param>
    /// <param name="maxIter"> Maximal amount of iterations </param>
    /// <param name="needStats"> Is need to save statistics to file "stats_LOS.txt" </param>
    /// <returns> Solution vector </returns>
    public static double[] SolveWithLOS(
        Matrix a, 
        double[] b, 
        double[] x, 
        double eps,
        int maxIter,
        bool needStats
    )
    {
        var r = new double[b.Length];

        var prodAx = MatMul(a, x);

        // r = b - Ax
        for (var i = 0; i < b.Length; i++)
        {
            r[i] = b[i] - prodAx[i];
        }

        var z = new double[b.Length];
        r.AsSpan().CopyTo(z);

        var p = MatMul(a, z);

        var k = 1;
        var residual = Dot(r, r);

        // Need for checking stagnation 
        var residualNext = residual + 1.0;
        var absResidualDifference = Math.Abs(residual - residualNext);

        for (; k < maxIter && residual > eps && absResidualDifference > 1e-15; k++)
        {
            // alpha = (p_{k-1}, r_{k-1}) / (p_{k-1}, p_{k-1})
            var pp = Dot(p, p);
            var alpha = Dot(p, r) / pp;

            // x_{k} = x_{k-1} + alpha * z_{k-1}
            for (var i = 0; i < b.Length; i++)
            {
                x[i] += alpha * z[i];
            }

            absResidualDifference = Math.Abs(residual - residualNext);

            // Updating residual
            residualNext = residual;

            // (r_{k}, r_{k}) = (r_{k-1}, r_{k-1}) - alpha^2 * (p_{k-1}, p_{k-1})
            residual -= alpha * alpha * pp;

            Console.Write($"\rIter: {k}, R: {residual}, |RNext - R| = {absResidualDifference}");

            // Updating from r_{k-1} to r_{k}
            // r_{k} = r_{k-1} - alpha * p_{k-1}
            for (var i = 0; i < r.Length; i++)
            {
                r[i] -= alpha * p[i];
            }

            // beta = -(p_{k-1}, Ar_{k}) / (p_{k-1}, p_{k-1})
            var aR = MatMul(a, r);
            var beta = -Dot(p, aR) / pp;

            // z_{k} = r_{k} + beta * z_{k-1}
            for (var i = 0; i < z.Length; i++)
            {
                z[i] = r[i] + beta * z[i];
            }

            // p_{k} = Ar_{k} + beta * p_{k-1}
            for (var i = 0; i < p.Length; i++)
            {
                p[i] = aR[i] + beta * p[i];
            }
        }

        if (needStats)
        {
            using var statsFile = new StreamWriter("stats_LOS.txt");
            Utils.ExportStatsToFile(statsFile, k, residual);
        }

        return x;
    }
}