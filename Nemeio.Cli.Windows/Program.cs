namespace Nemeio.Cli.Windows
{
    internal static class Program
    {
        static int Main(string[] args)
        {
            var plateformDependencies = new WindowsDependencyDelegate();
            var exitCode = Cli.Program.Main(plateformDependencies, args);
            
            return exitCode;
        }
    }
}
