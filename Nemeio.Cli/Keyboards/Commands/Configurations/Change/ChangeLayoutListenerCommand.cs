using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Change
{
    internal sealed class ChangeLayoutListenerCommand : KeyboardCommand
    {
        private IConfigurationChangedMonitor _configurationChangeMonitor;
        private TaskCompletionSource<string> _task;

        public ChangeLayoutListenerCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source) 
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source) { }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            _task = new TaskCompletionSource<string>();

            CancellationTokenSource.Token.Register(() => 
            {
                _task.TrySetException(new ChangeListenerTimeoutException());
            });

            _configurationChangeMonitor = _monitorFactory.CreateConfigurationChangedMonitor(nemeio.CommandExecutor);
            _configurationChangeMonitor.ConfigurationChanged += Monitor_ConfigurationChanged;

            var hash = await _task.Task;

            var output = new HashOutput(hash);

            _outputWriter.WriteObject(output);
        }

        private void Monitor_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            _configurationChangeMonitor.ConfigurationChanged -= Monitor_ConfigurationChanged;
            _task.TrySetResult(e.Configuration.ToString());
        }
    }
}
