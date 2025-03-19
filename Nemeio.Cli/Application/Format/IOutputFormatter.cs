namespace Nemeio.Cli.Application
{
    public interface IOutputFormatter
    {
        string Format<T>(T obj) where T : class;
    }
}
