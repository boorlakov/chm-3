namespace chm_3;

public static class Program
{
    public static void Main(string[] args)
    {
        var inMatrixFn = new StreamReader("inMatrix.txt");
        var a = Utils.MatrixFromFile(inMatrixFn);

        var inBiasFn = new StreamReader("inBias.txt");
        var b = Utils.VectorFromFile(inBiasFn);

        var inInitApproxFn = new StreamReader("inInitApproximation.txt");
        var x = Utils.VectorFromFile(inInitApproxFn);

        // TODO: Add further program behavior

    }
}