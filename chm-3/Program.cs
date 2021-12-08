using System.Diagnostics;

namespace chm_3;

public static class Program
{
    public static void Main(string[] args)
    {
        using var gguInFile = new StreamReader("ggu.txt");
        using var gglInFile = new StreamReader("ggl.txt");
        using var diInFile = new StreamReader("di.txt");
        using var igInFile = new StreamReader("ig.txt");
        using var jgInFile = new StreamReader("jg.txt");
        using var sizeInFile = new StreamReader("size.txt");

        var a = Utils.MatrixFromFiles(gglInFile, gguInFile, diInFile, igInFile, jgInFile, sizeInFile);

        using var fFile = new StreamReader("pr.txt");
        var b = Utils.VectorFromFile(fFile);

        using var initApproxFile = new StreamReader("x.txt");
        var x = Utils.VectorFromFile(initApproxFile);

        using var epsFile = new StreamReader("eps.txt");
        var eps = Utils.ReadDouble(epsFile);

        using var maxIterFile = new StreamReader("maxIter.txt");
        var maxIter = Utils.ReadInt(maxIterFile);

        var time = Stopwatch.StartNew();

        var solution = LinAlg.SolveWithLOS(a, b, x, eps, maxIter, true);

        time.Stop();

        Console.Write($"\nSolution vector for ({time.ElapsedMilliseconds} ms.):");
        Utils.Pprint(solution);

        using var outputFile = new StreamWriter("output.txt");
        Utils.ExportToFile(outputFile, solution);
    }
}