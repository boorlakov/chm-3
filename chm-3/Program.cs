namespace chm_3;

public static class Program
{
    public static void Main(string[] args)
    {
        var gguInFile = new StreamReader("ggu.txt");
        var gglInFile = new StreamReader("ggl.txt");
        var diInFile = new StreamReader("di.txt");
        var igInFile = new StreamReader("ig.txt");
        var jgInFile = new StreamReader("jg.txt");
        var sizeInFile = new StreamReader("size.txt");

        var a = Utils.MatrixFromFiles(gglInFile, gguInFile, diInFile, igInFile, jgInFile, sizeInFile);

        var fFile = new StreamReader("pr.txt");
        var b = Utils.VectorFromFile(fFile);

        var initApproxFile = new StreamReader("x.txt");
        var x = Utils.VectorFromFile(initApproxFile);

        var epsFile = new StreamReader("eps.txt");
        var eps = Utils.ReadDouble(epsFile);

        var maxIterFile = new StreamReader("maxIter.txt");
        var maxIter = Utils.ReadInt(maxIterFile);

        // TODO: Add further program behavior
    }
}