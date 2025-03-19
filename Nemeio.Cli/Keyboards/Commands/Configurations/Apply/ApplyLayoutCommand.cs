using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Cli.Keyboards.Commands.Configurations.Change;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Apply
{
    internal sealed class ApplyLayoutCommand : LayoutHashCommand
    {
        private IConfigurationChangedMonitor _configurationChangeMonitor;
        private TaskCompletionSource<string> _task;
        private bool _mustWaitSwitch;

        public ApplyLayoutCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, string layoutHash, bool waitSwitch)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source, layoutHash)
        {
            _mustWaitSwitch = waitSwitch;
        }

        public override async Task RunAsync(Nemeio nemeio, LayoutId layoutId)
        {
            if (_mustWaitSwitch)
            {
                _task = new TaskCompletionSource<string>();

                CancellationTokenSource.Token.Register(() =>
                {
                    _task.TrySetException(new ChangeListenerTimeoutException());
                });

                _configurationChangeMonitor = _monitorFactory.CreateConfigurationChangedMonitor(nemeio.CommandExecutor);
                _configurationChangeMonitor.ConfigurationChanged += Monitor_ConfigurationChanged;
            }

            var applyConfigurationMonitor = _monitorFactory.CreateApplyConfigurationMonitor(nemeio.CommandExecutor);

            await Task.Run(async () =>
            {
                try
                {
                    applyConfigurationMonitor.Apply(layoutId);

                    if (_mustWaitSwitch)
                    {
                        var layoutHash = await _task.Task;

                        var output = new HashOutput(layoutHash);

                        _outputWriter.WriteObject(output);
                    }
                }
                catch (KeyboardException exception)
                {
                    _logger.LogError(exception, $"Keyboard exception");

                    if (exception.ErrorCode == KeyboardErrorCode.NotFound)
                    {
                        throw new LayoutNotFoundException();
                    }
                    else
                    {
                        throw new ApplyLayoutFailedException();
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Unknown exception");

                    throw;
                }
            }, CancellationTokenSource.Token);
        }

        private void Monitor_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            _configurationChangeMonitor.ConfigurationChanged -= Monitor_ConfigurationChanged;
            _task.TrySetResult(e.Configuration.ToString());
        }
    }
}
