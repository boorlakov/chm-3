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
    /// <param name="eps"> Wanted ideal accuracy </param>
    /// <param name="maxIter"> Maximal amount of iterations </param>
    /// <param name="needStats"> Is need to save statistics to file "stats_LOS.txt" </param>
    /// <returns> Solution vector </returns>
    public static double[] SolveWithLOS(
        Matrix a, 
        double[] b, 
        double eps,
        int maxIter,
        bool needStats
    )
    {
        var x = new double[b.Length];
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

            Console.Write($"\rPure LOS. Iter: {k}, R: {residual}, |RNext - R| = {absResidualDifference}");

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

    /// <summary>
    /// Solves slae like Ax = b. Required factorized version of A
    /// </summary>
    /// <param name="a"></param>
    /// <param name="preCondA"></param>
    /// <param name="b"> Input vector of right hand side </param>
    /// <param name="eps"> Wanted ideal accuracy </param>
    /// <param name="maxIter"> Maximal amount of iterations </param>
    /// <param name="needStats"> Is need to save statistics to file "stats_LOSPreCondLUsq.txt" </param>
    /// <returns> Solution vector </returns>
    /// <exception cref="NotDecomposedException"> Requires pre conditional matrix </exception>
    public static double[] SolveWithLOSPrecondLUsq(
        Matrix a, 
        Matrix preCondA, 
        double[] b, 
        double eps, 
        int maxIter, 
        bool needStats
    )
    {
        if (!preCondA.Decomposed)
        {
            throw new NotDecomposedException();
        }

        var x = new double[b.Length];

        // A x_{0}
        var matMulRes = MatMul(a, x);

        // f - A x_{0}
        for (var i = 0; i < a.Size; i++)
        {
            matMulRes[i] = b[i] - matMulRes[i];
        }

        // r = L^{-1} (f - A x_{0}) === Lr = (f - A x_{0}) ==> Forward
        var r = ForwardLUsq(preCondA, matMulRes);

        // r = (r_{0}, r_{0})         
        var residual = Dot(r, r);

        // Need for checking stagnation 
        var residualNext = residual + 1.0;

        var absResidualDifference = Math.Abs(residual - residualNext);

        // z = U^{-1} r === Uz = r ==> Backward
        var z = BackwardLUsq(preCondA, r);

        // A z_{0}
        matMulRes = MatMul(a, z);

        // p_{0} = L^{-1} Az_{0} === L p_{0} = A z_{0} ==> Forward
        var p = ForwardLUsq(preCondA, matMulRes);

        // Current iteration
        var k = 1;

        for (; residual > eps && k < maxIter && absResidualDifference > 1e-15; k++)
        {
            var pp = Dot(p, p);
            var alpha = Dot(p, r) / pp;

            absResidualDifference = Math.Abs(residual - residualNext);

            // Updating residual
            residualNext = residual;

            // We dont need to over-calculate scalar product, because we calculated at k = 0
            // r_{k} = - α^2 (p_{k-1}, p_{k-1}) 
            residual -= alpha * alpha * pp;
            Console.Write($"\rLOS With LU Precond. Iter: {k}, R: {residual}, |RNext - R| = {absResidualDifference}");

            // Updating to {k} 
            for (var i = 0; i < a.Size; i++)
            {
                x[i] += alpha * z[i];
                r[i] -= alpha * p[i];
            }

            // Go from right to left. Need to avoid finding reverse matrices
            var ur = BackwardLUsq(preCondA, r);
            matMulRes = MatMul(a, ur);

            var dotRhs = ForwardLUsq(preCondA, matMulRes);

            // β = - (p_{k-1}, L^{-1} A U^{-1} r_{k}) / (p_{k-1}, p_{k-1}) 
            var beta = -Dot(p, dotRhs) / pp;

            // Updating to {k} 
            for (var i = 0; i < a.Size; i++)
            {
                // z_{k} = U^{-1} r + β z_{k-1} 
                z[i] = ur[i] + beta * z[i];

                // p_{k} = L^{-1} A U^{-1} r_{k} + β p_{k-1}
                p[i] = dotRhs[i] + beta * p[i];
            }
        }

        if (needStats)
        {
            using var statsFile = new StreamWriter("stats_LOSPreCondLUsq.txt");
            Utils.ExportStatsToFile(statsFile, k, residual);
        }

        return x;
    }

    public static double[] MatMulD(Matrix d, double[] b)
    {
        var res = new double[b.Length];

        for (var i = 0; i < res.Length; i++)
        {
            res[i] += d.Di[i] * b[i];
        }

        return res;
    }

    public static double[] SolveWithLOSPrecondDiag(
        Matrix a,
        Matrix aDiag,
        double[] b,
        double eps,
        int maxIter,
        bool needStats
    )
    {
        if (!aDiag.Decomposed)
        {
            throw new NotDecomposedException();
        }

        var x = new double[b.Length];

        // A x_{0}
        var matMulRes = MatMul(a, x);

        // f - A x_{0}
        for (var i = 0; i < a.Size; i++)
        {
            matMulRes[i] = b[i] - matMulRes[i];
        }

        // r = L^{-1} (f - A x_{0}) === Lr = (f - A x_{0}) ==> Forward
        var r = MatMulD(aDiag, matMulRes);

        // r = (r_{0}, r_{0})         
        var residual = Dot(r, r);

        // Need for checking stagnation 
        var residualNext = residual + 1.0;

        var absResidualDifference = Math.Abs(residual - residualNext);

        // z = U^{-1} r === Uz = r ==> Backward
        var z = MatMulD(aDiag, r);

        // A z_{0}
        matMulRes = MatMul(a, z);

        // p_{0} = L^{-1} Az_{0} === L p_{0} = A z_{0} ==> Forward
        var p = MatMulD(aDiag, matMulRes);

        // Current iteration
        var k = 1;

        for (; residual > eps && k < maxIter && absResidualDifference > 1e-15; k++)
        {
            var pp = Dot(p, p);
            var alpha = Dot(p, r) / pp;

            absResidualDifference = Math.Abs(residual - residualNext);

            // Updating residual
            residualNext = residual;

            // We dont need to over-calculate scalar product, because we calculated at k = 0
            // r_{k} = - α^2 (p_{k-1}, p_{k-1}) 
            residual -= alpha * alpha * pp;
            Console.Write($"\rLOS With LU Precond. Iter: {k}, R: {residual}, |RNext - R| = {absResidualDifference}");

            // Updating to {k} 
            for (var i = 0; i < a.Size; i++)
            {
                x[i] += alpha * z[i];
                r[i] -= alpha * p[i];
            }

            // Go from right to left. Need to avoid finding reverse matrices
            var ur = MatMulD(aDiag, r);
            matMulRes = MatMul(a, ur);
            var dotRhs = MatMulD(aDiag, matMulRes);

            // β = - (p_{k-1}, L^{-1} A U^{-1} r_{k}) / (p_{k-1}, p_{k-1}) 
            var beta = -Dot(p, dotRhs) / pp;

            // Updating to {k} 
            for (var i = 0; i < a.Size; i++)
            {
                // z_{k} = U^{-1} r + β z_{k-1} 
                z[i] = ur[i] + beta * z[i];

                // p_{k} = L^{-1} A U^{-1} r_{k} + β p_{k-1}
                p[i] = dotRhs[i] + beta * p[i];
            }
        }

        if (needStats)
        {
            using var statsFile = new StreamWriter("stats_LOSPreCondDiag.txt");
            Utils.ExportStatsToFile(statsFile, k, residual);
        }

        return x;
    }

    /// <summary>
    /// Forward solution for Lx = b. Made for avoiding L^{-1}
    /// </summary>
    /// <param name="l"> Decomposed matrix in sparse format </param>
    /// <param name="b"> RHS vector </param>
    /// <returns> Solution for lower triangular part of matrix A</returns>
    /// <exception cref="NotDecomposedException"> Required decomposed matrix </exception>
    public static double[] ForwardLUsq(Matrix l, double[] b)
    {
        if (!l.Decomposed)
        {
            throw new NotDecomposedException();
        }

        var y = new double[b.Length];
        b.AsSpan().CopyTo(y);

        for (var i = 0; i < y.Length; i++)
        {
            var sum = 0.0;
            for (var j = l.Ig[i]; j < l.Ig[i + 1]; j++)
            {
                sum += l.Ggl[j] * y[l.Jg[j]];
            }

            y[i] -= sum;
            y[i] /= l.Di[i];
        }

        return y;
    }

    /// <summary>
    /// Backward solution for Ux = y. Made for avoiding U^{-1}
    /// </summary>
    /// <param name="u"> Decomposed matrix in sparse format </param>
    /// <param name="y"> RHS vector </param>
    /// <returns> Solution for upper triangular part of matrix A</returns>
    /// <exception cref="NotDecomposedException"> Required decomposed matrix </exception>
    public static double[] BackwardLUsq(Matrix u, double[] y)
    {
        if (!u.Decomposed)
        {
            throw new NotDecomposedException();
        }

        var res = new double[y.Length];
        y.AsSpan().CopyTo(res);

        for (var i = u.Size - 1; i >= 0; i--)
        {
            res[i] /= u.Di[i];

            for (var j = u.Ig[i]; j < u.Ig[i + 1]; j++)
            {
                res[u.Jg[j]] -= u.Ggu[j] * res[i];
            }
        }

        return res;
    }
}