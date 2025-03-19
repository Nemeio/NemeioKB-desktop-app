using System;

namespace Nemeio.Cli.Application
{
    public sealed class ConsoleOutputWriter : IOutputWriter
    {
        private readonly IOutputFormatter _outputFormatter;

        public ConsoleOutputWriter(IOutputFormatter outputFormatter)
        {
            _outputFormatter = outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter));
        }

        public void WriteErrorLine(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            WriteLine(message);
            Console.ResetColor();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteObject<T>(T obj) where T : class
        {
            var data = _outputFormatter.Format(obj);

            WriteLine(data);
        }
    }
}
