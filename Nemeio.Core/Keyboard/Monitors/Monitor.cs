using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Core.Keyboard.Monitors
{
    public abstract class Monitor
    {
        protected readonly ILogger _logger;
        protected readonly IKeyboardCommandFactory _commandFactory;

        protected IList<KeyboardResponse> Responses { get; private set; }

        public Monitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));

            Responses = new List<KeyboardResponse>();
        }

        public virtual void ReceiveResponse(IFrame frame, Exception exception)
        {
            var response = new KeyboardResponse()
            {
                Frame = frame,
                Exception = exception
            };

            Responses.Add(response);
        }
    }
}
