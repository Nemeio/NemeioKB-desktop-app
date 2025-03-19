using System.Diagnostics;

namespace Nemeio.Core.Tools
{
    public sealed class StandardCommandLineExecutor : ICommandLineExecutor
    {
        public string Execute(string command, string arguments = null)
        {
            // Start the child process.
            var process = new Process();

            // Redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = command;
            process.StartInfo.CreateNoWindow = true;

            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.Start();

            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }
}
