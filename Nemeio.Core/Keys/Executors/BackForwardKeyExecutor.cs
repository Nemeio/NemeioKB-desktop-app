using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active;

namespace Nemeio.Core.Keys.Executors
{
    public class BackForwardKeyExecutor : KeyExecutor
    {
        private readonly ILogger _logger;
        private readonly bool _goBack;
        private readonly INemeio _nemeio;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public BackForwardKeyExecutor(ILoggerFactory loggerFactory, IActiveLayoutChangeHandler activeLayoutChangeHandler, INemeio nemeio, bool goBack, string data) 
            : base(data)
        {
            _goBack = goBack;
            _logger = loggerFactory.CreateLogger<BackForwardKeyExecutor>();
            _nemeio = nemeio ?? throw new ArgumentNullException(nameof(nemeio));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
        }

        public override async Task ExecuteAsync() => await _activeLayoutChangeHandler.RequestHistoricChangeAsync(_nemeio, _goBack);
    }
}
