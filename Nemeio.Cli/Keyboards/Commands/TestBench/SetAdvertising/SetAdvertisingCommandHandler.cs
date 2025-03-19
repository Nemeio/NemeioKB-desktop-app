using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetAdvertising
{
    internal sealed class SetAdvertisingCommandHandler : KeyboardCommandHandler, ISetAdvertisingCommandHandler
    {
        private CommandOption _enable;


        public SetAdvertisingCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("setAdvertising", "Allow to set the BLE advertising State")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _enable = application.Option("-e |--enable <enable>", "Desired BLE Advertise State (0 = off / 1 = on)", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var id = _enable;
            SetAdvertisingInput setAdvertisingInput = new SetAdvertisingInput(uint.Parse(id.Value()));

            var cancellationTokenSource = new CancellationTokenSource();
            var command = new SetAdvertisingCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, setAdvertisingInput);

            await command.ExecuteAsync();
        }
    }
}
