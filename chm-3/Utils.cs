using System.Text;

namespace chm_3;

public static class Utils
{
    /// <param name="file"> input file for initialise matrix vectorX</param>
    /// <returns>Complete matrix</returns>
    public static Matrix MatrixFromFile(StreamReader file)
    {
        var ggl = ReadDoubles(file);
        var ggu = ReadDoubles(file);
        var di = ReadDoubles(file);
        var ig = ReadInts(file);
        var jg = ReadInts(file);
        var size = ReadInt(file);

        return new Matrix(ggl, ggu, di, ig, jg, size);
    }

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

    public static void ExportToFile(StreamWriter outputFile, double[] vectorX, double[] absVector)
    {
        var sb = new StringBuilder();

        foreach (var item in vectorX)
        {
            sb.Append($"{item:G15} ");
        }

        sb.Append('\n');

        foreach (var item in absVector)
        {
            sb.Append($"{item:G15} ");
        }

        var text = sb.ToString();

        outputFile.Write(text);
    }

    /// <summary>
    ///     Perfect print is using for debugging profile format.
    ///     Prints as matrix is in default format.
    /// </summary>
    [Obsolete("This is slow. Use this only for debug")]
    public static void Pprint(Matrix matrixA)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;

        Console.WriteLine("\nMatrix PPRINT:");

        if (!matrixA.Decomposed)
        {
            Console.WriteLine("Undecomposed:");

            for (var i = 0; i < matrixA.Size; i++)
            {
                for (var j = 0; j < matrixA.Size; j++)
                {
                    Console.Write($"{matrixA[i, j]:G15} ");
                }

                Console.WriteLine();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Decomposed:");
            Console.WriteLine("L:");

            for (var i = 0; i < matrixA.Size; i++)
            {
                for (var j = 0; j < matrixA.Size; j++)
                {
                    Console.Write($"{matrixA.L(i, j)} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("U:");

            for (var i = 0; i < matrixA.Size; i++)
            {
                for (var j = 0; j < matrixA.Size; j++)
                {
                    Console.Write($"{matrixA.U(i, j)} ");
                }

                Console.WriteLine();
            }
        }

        Console.ResetColor();
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