using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Handlers.Keyboards;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FrontLightPower
{
    internal sealed class SetFrontLightPowerCommandHandler : KeyboardCommandHandler, ISetFrontLightPowerCommandHandler
    {
        private CommandOption _power;


        public SetFrontLightPowerCommandHandler(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, new CommandInfo("setFrontLightPower", "Allow to define the Front Light Power (%)")) { }

        public override void RegisterOptions(CommandLineApplication application)
        {
            _power = application.Option("-p |--power <power>", "percentage of power()", CommandOptionType.SingleValue);
        }

        public override async Task ExecuteAsync()
        {
            var id = _power;
            FrontLightPowerInput frontLightPowerInput = new FrontLightPowerInput(uint.Parse(id.Value()));

            var cancellationTokenSource = new CancellationTokenSource();
            var command = new SetFrontLightPowerCommand(_loggerFactory, _outputWriter, _keyboardAwaiterFactory, _monitorFactory, cancellationTokenSource, frontLightPowerInput);

            await command.ExecuteAsync();
        }
    }
}
