using System.Diagnostics;

namespace chm_3;

public static class Program
{
    public static void Main(string[] args)
    {
        using var gguInFile = new StreamReader("../../../TestData/LittleTests/ggu.txt");
        using var gglInFile = new StreamReader("../../../TestData/LittleTests/ggl.txt");
        using var diInFile = new StreamReader("../../../TestData/LittleTests/di.txt");
        using var igInFile = new StreamReader("../../../TestData/LittleTests/ig.txt");
        using var jgInFile = new StreamReader("../../../TestData/LittleTests/jg.txt");
        using var sizeInFile = new StreamReader("../../../TestData/LittleTests/size.txt");

        var a = Utils.MatrixFromFiles(gglInFile, gguInFile, diInFile, igInFile, jgInFile, sizeInFile);

        using var fFile = new StreamReader("../../../TestData/LittleTests/pr.txt");
        var b = Utils.VectorFromFile(fFile);

        using var initApproxFile = new StreamReader("../../../TestData/LittleTests/x.txt");
        var x = Utils.VectorFromFile(initApproxFile);

        using var epsFile = new StreamReader("../../../TestData/LittleTests/eps.txt");
        var eps = Utils.ReadDouble(epsFile);

        using var maxIterFile = new StreamReader("../../../TestData/LittleTests/maxIter.txt");
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