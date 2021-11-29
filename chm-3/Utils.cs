using System.Text;

namespace chm_3;

public static class Utils
{
    /// <param name="file"> input file for initialise matrix vectorX</param>
    /// <returns>Complete matrix</returns>
    public static Matrix MatrixFromFile(StreamReader file)
    {
        var ln = file.ReadLine()!
            .Trim();

        var size = int.Parse(ln);

        var di = ReadDoubles(file);

        var ia = ReadInts(file);

        var au = ReadDoubles(file);

        var al = ReadDoubles(file);

        return new Matrix(size, di, ia, au, al);
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