using System.Text;

namespace chm_3;

public static class Utils
{
    public static Matrix MatrixFromFiles(
        StreamReader gglFile,
        StreamReader gguFile,
        StreamReader diFile,
        StreamReader igFile,
        StreamReader jgFile,
        StreamReader sizeFile
    )
    {
        var ggl = ReadDoubles(gglFile);
        var ggu = ReadDoubles(gguFile);
        var di = ReadDoubles(diFile);

        var ig = ReadInts(igFile);

        for (var i = 0; i < ig.Length; i++)
        {
            ig[i]--;
        }

        var jg = ReadInts(jgFile);

        for (var i = 0; i < jg.Length; i++)
        {
            jg[i]--;
        }

        var size = ReadInt(sizeFile);

        return new Matrix(ggl, ggu, di, ig, jg, size, false);
    }

    public static Matrix MatrixFromFilesByLine(
        string gglFileName,
        string gguFileName,
        string diFileName,
        string igFileName,
        string jgFileName,
        string sizeFileName
    )
    {
        var ggl = ReadDoublesByLine(gglFileName);
        var ggu = ReadDoublesByLine(gguFileName);
        var di = ReadDoublesByLine(diFileName);

        var ig = ReadIntsByLine(igFileName);

        for (var i = 0; i < ig.Length; i++)
        {
            ig[i]--;
        }

        var jg = ReadIntsByLine(jgFileName);

        for (var i = 0; i < jg.Length; i++)
        {
            jg[i]--;
        }

        using var sizeFile = new StreamReader(sizeFileName);
        var size = ReadInt(sizeFile);

        return new Matrix(ggl, ggu, di, ig, jg, size, false);
    }

    private static double[] ReadDoublesByLine(string fileName)
    {
        return File
            .ReadAllLines(fileName)
            .Select(double.Parse)
            .ToArray();
    }

    private static int[] ReadIntsByLine(string fileName)
    {
        return File
            .ReadAllLines(fileName)
            .Select(int.Parse)
            .ToArray();
    }

    private static double[] ReadDoubles(StreamReader file)
    {
        return file
            .ReadLine()!
            .Trim()
            .Split(' ')
            .Select(double.Parse)
            .ToArray();
    }

    private static int[] ReadInts(StreamReader file)
    {
        return file
            .ReadLine()!
            .Trim()
            .Split(' ')
            .Select(int.Parse)
            .ToArray();
    }

    public static double ReadDouble(StreamReader file) => double.Parse(file.ReadLine()!.Trim(' '));

    public static int ReadInt(StreamReader file) => int.Parse(file.ReadLine()!.Trim(' '));

    public static double[] VectorFromFile(StreamReader file) => ReadDoubles(file);

    public static double[] VectorFromFileByLine(string file) => ReadDoublesByLine(file);

    public static Matrix CopyMatrix(Matrix m)
    {
        var ggl = new double[m.Ggl.Length];
        m.Ggl.AsSpan().CopyTo(ggl);

        var ggu = new double[m.Ggu.Length];
        m.Ggu.AsSpan().CopyTo(ggu);

        var di = new double[m.Di.Length];
        m.Di.AsSpan().CopyTo(di);

        var ig = new int[m.Ig.Length];
        m.Ig.AsSpan().CopyTo(ig);

        var jg = new int[m.Jg.Length];
        m.Jg.AsSpan().CopyTo(jg);
        var size = m.Size;
        var decomposed = m.Decomposed;
        return new Matrix(ggl, ggu, di, ig, jg, size, decomposed);
    }

    public static void ExportToFile(StreamWriter outputFile, double[] vector)
    {
        var sb = new StringBuilder();

        foreach (var item in vector)
        {
            sb.Append($"{item:G15}\n");
        }

        var text = sb.ToString();

        outputFile.Write(text);
    }

    public static void ExportStatsToFile(StreamWriter outputFile, int iterationsNum, double relativeResidual, long ms)
    {
        var sb = new StringBuilder(
            $"Iterations: {iterationsNum:G15}; Relative residual: {relativeResidual}; Done for: {ms} ms.");

        var text = sb.ToString();

        outputFile.Write(text);
    }

    public static void Pprint(double[] vector)
    {
        Console.WriteLine("\nVector pprint:");

        foreach (var item in vector)
        {
            Console.Write("{0:G15}\n", item);
        }

        Console.WriteLine();
    }
}