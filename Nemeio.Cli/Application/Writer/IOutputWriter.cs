using System;

namespace Nemeio.Cli.Application
{
    public interface IOutputWriter
    {
        void WriteLine(string message);
        void WriteObject<T>(T obj) where T : class;
        void WriteErrorLine(string message);
    }
}
