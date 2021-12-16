using System.Diagnostics;

namespace chm_3;

public static class Program
{
    public static void Main(string[] args)
    {
        using var gguInFile = new StreamReader("../../../TestData/LittleTests/minus/ggu.txt");
        using var gglInFile = new StreamReader("../../../TestData/LittleTests/minus/ggl.txt");
        using var diInFile = new StreamReader("../../../TestData/LittleTests/minus/di.txt");
        using var igInFile = new StreamReader("../../../TestData/LittleTests/minus/ig.txt");
        using var jgInFile = new StreamReader("../../../TestData/LittleTests/minus/jg.txt");
        using var sizeInFile = new StreamReader("../../../TestData/LittleTests/minus/size.txt");

        var a = Utils.MatrixFromFiles(gglInFile, gguInFile, diInFile, igInFile, jgInFile, sizeInFile);

        using var fFile = new StreamReader("../../../TestData/LittleTests/minus/pr.txt");
        var b = Utils.VectorFromFile(fFile);

        using var epsFile = new StreamReader("../../../TestData/LittleTests/minus/eps.txt");
        var eps = Utils.ReadDouble(epsFile);

        using var maxIterFile = new StreamReader("../../../TestData/LittleTests/minus/maxIter.txt");
        var maxIter = Utils.ReadInt(maxIterFile);

        var preCondA = Utils.CopyMatrix(a);
        var aDiag = Utils.CopyMatrix(a);

        preCondA.Factorize();
        aDiag.DiagFactorize();

        var time = Stopwatch.StartNew();
        var solution0 = LinAlg.SolveWithLOS(a, b, eps, maxIter, true);
        Console.Write($"\n1) Done by ({time.ElapsedMilliseconds} ms.)");
        time.Stop();

        var time1 = Stopwatch.StartNew();
        var solution1 = LinAlg.SolveWithLOSPrecondLUsq(a, preCondA, b, eps, maxIter, true);
        time1.Stop();
        Console.Write($"\n2) Done by ({time1.ElapsedMilliseconds} ms.)");

        var time2 = Stopwatch.StartNew();
        var solution2 = LinAlg.SolveWithLOSPrecondDiag(a, aDiag, b, eps, maxIter, true);
        time2.Stop();
        Console.Write($"\n3) Done by ({time2.ElapsedMilliseconds} ms.)");
        // Utils.Pprint(solution0);
        // Utils.Pprint(solution1);
        // Utils.Pprint(solution2);
        
        // using var outputFile = new StreamWriter("output.txt");
        // Utils.ExportToFile(outputFile, solution);
    }
}