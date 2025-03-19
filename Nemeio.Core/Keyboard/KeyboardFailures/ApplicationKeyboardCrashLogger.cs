namespace Nemeio.Core.Keyboard.KeyboardFailures
{
    public sealed class ApplicationKeyboardCrashLogger : KeyboardCrashLogger
    {
        public ApplicationKeyboardCrashLogger() 
            : base(NemeioConstants.LogPath) { }
    }
}
