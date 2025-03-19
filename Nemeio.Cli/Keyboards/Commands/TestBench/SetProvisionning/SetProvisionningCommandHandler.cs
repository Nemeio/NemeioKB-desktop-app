using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetProvisionning
{
    internal sealed class SetProvisionningCommandHandler : KeyboardCommandHandler, ISetProvisionningCommandHandler
    {
        private CommandOption _serial;
        private CommandOption _mac;

        public SetProvisionningCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("setProvisionning", "Allow to provision MAC and Serial to keyboard")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _serial = application.Option("-s |--serial <serial>", "12 bytes representing the serial number", CommandOptionType.SingleValue);
            _mac = application.Option("-m |--mac <mac>", "6 bytes representing the mac adress", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var serial = _serial;
            var mac= _mac;
            SetProvisionningInput setProvisionningInput = new SetProvisionningInput(serial.Value(), mac.Value());

            var cancellationTokenSource = new CancellationTokenSource();
            var command = new SetProvisionningCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, setProvisionningInput);

            await command.ExecuteAsync();
        }
    }
}
