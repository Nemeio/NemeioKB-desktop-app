using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Monitors;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Add
{
    internal sealed class AddLayoutCommand : KeyboardCommand
    {
        private readonly CompressedLayoutFile _layoutFile;
        private readonly bool _isFactoryLayout;

        public AddLayoutCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, CompressedLayoutFile layoutFile, bool isFactoryLayout = false)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            _layoutFile = layoutFile ?? throw new ArgumentNullException(nameof(layoutFile));
            _isFactoryLayout = isFactoryLayout;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var layout = await _layoutFile.LoadLayoutAsync();
            layout.LayoutInfo.Factory = _isFactoryLayout;
            layout.CalculateImageHash();
            var addLayoutMonitor = _monitorFactory.CreateAddConfigurationMonitor(nemeio.CommandExecutor);

            await Task.Run(() =>
            {
                try
                {
                    addLayoutMonitor.SendConfiguration(layout, _isFactoryLayout);
                }
                catch (Exception exception)
                {
                    var addLayoutsErrorMessage = "Add layout failed";

                    _logger.LogError(exception, addLayoutsErrorMessage);

                    throw new AddLayoutCommandFailedException(addLayoutsErrorMessage, exception);
                }
            });
         }
    }
}

