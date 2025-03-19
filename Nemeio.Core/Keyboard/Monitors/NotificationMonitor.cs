using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Core.Keyboard.Monitors
{
    public abstract class NotificationMonitor : ResponseMonitor
    {
        public NotificationMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
        }

        /// <summary>
        /// Notification Monitor just receive data from keyboard.
        /// They never need to execute command.
        /// </summary>
        protected override IList<KeyboardResponse> ExecuteCommand(IKeyboardCommand command)
        {
            throw new NotSupportedException("ExecuteCommand is not supported on NotificationMonitor");
        }

        /// <summary>
        /// Notification Monitor just receive data from keyboard.
        /// They never need to release semaphore.
        /// </summary>
        public override bool CanRelease(IFrame frame, Exception exception) => false;

        public override void ReceiveResponse(IFrame frame, Exception exception)
        {
            var response = new KeyboardResponse()
            {
                Frame = frame,
                Exception = exception
            };

            OnReceiveNotification(response);
        }

        public abstract void OnReceiveNotification(KeyboardResponse response);
    }
}
