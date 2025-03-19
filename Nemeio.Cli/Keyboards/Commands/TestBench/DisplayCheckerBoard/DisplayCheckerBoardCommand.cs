using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands;
using Nemeio.Cli.Commands.Exceptions;
using Nemeio.Cli.Keyboards.Awaiters;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Services.Batteries;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.DisplayCheckerBoard
{
    internal sealed class DisplayCheckerBoardCommand : KeyboardCommand
    {
        private readonly DisplayCheckerBoardInput _displayCheckerBoardInput;
        public DisplayCheckerBoardCommand(ILoggerFactory loggerFactory, IOutputWriter outputWriter, IKeyboardAwaiterFactory keyboardAwaiterFactory, IMonitorFactory monitorFactory, CancellationTokenSource source, DisplayCheckerBoardInput displayCheckerBoardInput)
            : base(loggerFactory, outputWriter, keyboardAwaiterFactory, monitorFactory, source)
        {
            if (displayCheckerBoardInput==null)
            {
                throw new ArgumentNullException(nameof(displayCheckerBoardInput));
            }

            _displayCheckerBoardInput = displayCheckerBoardInput;
        }

        public override async Task ApplyAsync(Nemeio nemeio)
        {
            var timeout = TimeSpan.FromSeconds(5);

            await Task.Run(() =>
            {
                GenericTestBenchResult result = null;
                try
                {
                    var displayCheckerBoardMonitor = _monitorFactory.CreateDisplayCheckerBoardMonitor(nemeio.CommandExecutor);
                    result = displayCheckerBoardMonitor.DisplayCheckerBoard(_displayCheckerBoardInput.FirstColor);
                    _outputWriter.WriteObject(result);
                }
                catch (KeyboardException keyboardException)
                {
                    var output = new GenericTestBenchResult(keyboardException.ErrorCode);
                    _logger.LogError(this.GetType().Name, keyboardException);
                    _outputWriter.WriteObject(output);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"DisplayCheckerBoard failed";
                    _logger.LogError(errorMessage, exception);
                    _outputWriter.WriteObject(exception.Message);
                    throw new DisplayCheckerBoardFailedException("DisplayCheckerBoard failed", exception);
                }
            });
        }
    }
}
