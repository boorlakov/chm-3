using System.Diagnostics;

namespace chm_3;

public static class Program
{
    public static void Main(string[] args)
    {
        const string gguInFileName = "../../../TestData/BigTests/4545/ggu.txt"; 
        const string gglInFileName = "../../../TestData/BigTests/4545/ggl.txt";
        const string diInFileName = "../../../TestData/BigTests/4545/di.txt";
        const string igInFileName = "../../../TestData/BigTests/4545/ig.txt";
        const string jgInFileName = "../../../TestData/BigTests/4545/jg.txt";
        const string sizeInFileName = "../../../TestData/BigTests/4545/size.txt";

        using var gguInFile = new StreamReader(gguInFileName);
        using var gglInFile = new StreamReader(gglInFileName);
        using var diInFile = new StreamReader(diInFileName);
        using var igInFile = new StreamReader(igInFileName);
        using var jgInFile = new StreamReader(jgInFileName);
        using var sizeInFile = new StreamReader(sizeInFileName);

        var a = Utils.MatrixFromFilesByLine(
            gglInFileName, gguInFileName, diInFileName, igInFileName, jgInFileName, sizeInFileName);

        const string fFileName = "../../../TestData/BigTests/4545/pr.txt";
        using var fFile = new StreamReader(fFileName);
        var b = Utils.VectorFromFileByLine(fFileName);

        const string epsFileName = "../../../TestData/BigTests/4545/eps.txt";
        using var epsFile = new StreamReader(epsFileName);
        var eps = Utils.ReadDouble(epsFile);

        const string maxIterFileName = "../../../TestData/BigTests/4545/maxIter.txt";
        using var maxIterFile = new StreamReader(maxIterFileName);
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
        using var outputFile = new StreamWriter("output.txt");
        Utils.ExportToFile(outputFile, solution1);
    }
}