using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.Errors;

namespace Nemeio.Core.Keyboard.Monitors
{
    public abstract class ResponseMonitor : Monitor
    {
        private readonly IKeyboardCommandExecutor _commandExecutor;
        protected readonly SemaphoreSlim _commandSemaphore;
        protected readonly IKeyboardErrorConverter _errorConverter;

        private static object _executeCommandLock = new object();

        private int _numberOfFrameReceived = 0;
        private int _numberOfFrameWaited = 0;

        public ResponseMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory)
        {
            _commandSemaphore = new SemaphoreSlim(0, 1);
            _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
            _errorConverter = errorConverter ?? throw new ArgumentNullException(nameof(errorConverter));
        }

        protected virtual IList<KeyboardResponse> ExecuteCommand(IKeyboardCommand command)
        {
            lock (_executeCommandLock)
            {
                Responses.Clear();

                _numberOfFrameWaited = command.FrameCount;
                _numberOfFrameReceived = 0;

                var canSchedule = _commandExecutor.ScheduleCommand(command);
                if (canSchedule)
                {
                    _commandSemaphore.Wait();
                }

                return Responses;
            }
        }

        public virtual bool CanRelease(IFrame frame, Exception exception)
        {
            _numberOfFrameReceived += 1;

            var raiseNumberOfFrame = _numberOfFrameReceived == _numberOfFrameWaited;
            var meetException = exception != null;

            if (meetException)
            {
                _logger.LogTrace($"CanRelease : meet exception <{exception.GetType().Name}> on <{GetType().Name}>");
            }

            return raiseNumberOfFrame || meetException;
        }

        public override void ReceiveResponse(IFrame frame, Exception exception)
        {
            base.ReceiveResponse(frame, exception);

            if (CanRelease(frame, exception) && _commandSemaphore.CurrentCount == 0)
            {
                _commandSemaphore.Release();
            }
        }

        protected void CheckResponsesAndThrowIfNeeded(IList<KeyboardResponse> responses)
        {
            if (responses == null)
            {
                throw new NoKeyboardResponseException();
            }
            else if (!responses.Any())
            {
                throw new NoKeyboardResponseException();
            }
            else if (responses.Any(x => x.Exception != null))
            {
                var exceptions = responses
                    .Where(x => x.Exception != null)
                    .Select(x => x.Exception)
                    .ToList();

                var aggregateException = new AggregateException(exceptions);

                throw new CommandFailedException("Communication with keyboard failed", aggregateException);
            }
        }

        protected void CheckKeyboardErrorCodeAndThrowIfNeeded(KeyboardResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var payload = response.Frame.Payload;
            var errorCode = payload.First();

            if (errorCode != (byte)KeyboardErrorCode.Success)
            {
                throw new KeyboardException((KeyboardErrorCode)errorCode, $"Monitor <{this.GetType().Name}> failed with error code ({errorCode})");
            }
        }
    }
}
