using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keys.Executors
{
    public class LayoutKeyExecutor : KeyExecutor
    {
        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;
        private readonly INemeio _nemeio;

        public LayoutKeyExecutor(ILoggerFactory loggerFactory, IActiveLayoutChangeHandler activeLayoutChangeHandler, IErrorManager errorManager, INemeio nemeio, string data) 
            : base(data) 
        {
            _logger = loggerFactory.CreateLogger<LayoutKeyExecutor>();
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _nemeio = nemeio ?? throw new ArgumentNullException(nameof(nemeio));
        }

        public override async Task ExecuteAsync()
        {
            try
            {
                var selectedLayoutId = new LayoutId(Data);

                await _activeLayoutChangeHandler.RequestKeyPressChangeAsync(_nemeio, selectedLayoutId);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreSetSpecificLayoutFailed)
                );
            }
        }
    }
}
