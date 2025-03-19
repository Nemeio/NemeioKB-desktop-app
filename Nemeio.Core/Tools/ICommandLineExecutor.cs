namespace Nemeio.Core.Tools
{
    public interface ICommandLineExecutor
    {
        string Execute(string command, string arguments);
    }
}
