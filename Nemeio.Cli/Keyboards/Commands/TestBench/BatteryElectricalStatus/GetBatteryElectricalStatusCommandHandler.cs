using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.BatteryElectricalStatus
{
    internal sealed class GetBatteryElectricalStatusCommandHandler : KeyboardCommandHandler, IGetBatteryElectricalStatusCommandHandler
    {
        public GetBatteryElectricalStatusCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("getBatteryElectricalStatus", "Allow to retrieve the battery electrical status")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            //Nothing to do here
        }

        public override async Task ExecuteAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var command = new GetBatteryElectricalStatusCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, (byte)1);

            await command.ExecuteAsync();
        }
    }
}
