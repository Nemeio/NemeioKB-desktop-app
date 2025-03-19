using Nemeio.Core.Keyboard.KeyboardFailures;

namespace Nemeio.Cli.Keyboards.Commands.Crashes
{
    internal sealed class CliKeyboardCrashLogger : KeyboardCrashLogger
    {
        public CliKeyboardCrashLogger(string path) 
            : base(path) { }
    }
}
