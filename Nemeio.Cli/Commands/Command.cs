using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;

namespace Nemeio.Cli.Commands
{
    internal abstract class Command : ICommand
    {
        protected readonly IOutputWriter _outputWriter;

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public Command(ILoggerFactory loggerFactory, IOutputWriter outputWriter, CancellationTokenSource source)
        {
            _outputWriter = outputWriter ?? throw new ArgumentNullException(nameof(outputWriter));
            CancellationTokenSource = source ?? throw new ArgumentNullException(nameof(source));
        }

        public abstract Task ExecuteAsync();
    }
}
