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

        return new Matrix(ggl, ggu, di, ig, jg, size);
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

    public static void ExportToFile(StreamWriter outputFile, double[] vectorX)
    {
        var sb = new StringBuilder();

        foreach (var item in vectorX)
        {
            sb.Append($"{item:G15} ");
        }

        var text = sb.ToString();

        outputFile.Write(text);
    }

    public static void Pprint(double[] vectorX)
    {
        Console.WriteLine("\nVector PPRINT:");

        foreach (var item in vectorX)
        {
            Console.Write("{0:G15} ", item);
        }

        Console.WriteLine();
    }
}