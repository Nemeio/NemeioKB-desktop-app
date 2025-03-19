using System.Diagnostics;
using System.IO;

namespace Nemeio.Cli.Linux
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var plateformDependencies = new LinuxDependencyDelegate();
            var exitCode = Cli.Program.Main(plateformDependencies, args);

            return exitCode;
        }
    }
}
