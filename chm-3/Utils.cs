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
        StreamReader gglFile,
        StreamReader gguFile,
        StreamReader diFile,
        StreamReader igFile,
        StreamReader jgFile,
        StreamReader sizeFile
    )
    {
        var ggl = ReadDoublesByLine(gglFile);
        var ggu = ReadDoublesByLine(gguFile);
        var di = ReadDoublesByLine(diFile);

        var ig = ReadIntsByLine(igFile);

        for (var i = 0; i < ig.Length; i++)
        {
            ig[i]--;
        }

        var jg = ReadIntsByLine(jgFile);

        for (var i = 0; i < jg.Length; i++)
        {
            jg[i]--;
        }

        var size = ReadInt(sizeFile);

        return new Matrix(ggl, ggu, di, ig, jg, size, false);
    }

    private static double[] ReadDoublesByLine(StreamReader file)
    {
        return file
            .ReadLine()!
            .Trim()
            .Split('\n')
            .Select(double.Parse)
            .ToArray();
    }

    private static int[] ReadIntsByLine(StreamReader file)
    {
        return file
            .ReadLine()!
            .Trim()
            .Split('\n')
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

    public static Matrix CopyMatrix(Matrix m)
    {
        return new Matrix(m.Ggl, m.Ggu, m.Di, m.Ig, m.Jg, m.Size, m.Decomposed);
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