using System;

namespace Nemeio.Tools.Core.Helpers
{
    public static class ConsoleHelper
    {
        public static void BreakLine() => WriteLine(string.Empty);

        public static void WriteLine(string message) => Console.WriteLine(message);

        public static void WriteException(Exception exception)
        {
            WriteError(exception.Message);
            BreakLine();
            WriteError(exception.StackTrace);

            if (exception.InnerException != null)
            {
                WriteException(exception.InnerException);
            }
        }

        public static void WriteError(string error) => WriteWithColor(error, ConsoleColor.Red);

        public static void WriteSuccess(string message) => WriteWithColor(message, ConsoleColor.Green);

        public static void WriteWithColor(string message, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(message);
            Console.ForegroundColor = previousColor;
        }
    }
}
