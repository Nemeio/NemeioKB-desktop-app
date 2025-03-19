using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Updates.Progress;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public sealed class UpdateProgressMonitor : NotificationMonitor, IUpdateProgressMonitor
    {
        private const int ComponentSize = 1;
        private const int TypeSize = 1;
        private const int PayloadSize = 1;
        private const int TotalPayloadSize = ComponentSize + TypeSize + PayloadSize;

        public event EventHandler<InProgressUpdateProgressEventArgs> OnUpdateProgressChanged;
        public event EventHandler<FailedUpdateProgressEventArgs> OnUpdateFailed;
        public event EventHandler<RollbackUpdateProgressEventArgs> OnRollbackProgressChanged;

        public UpdateProgressMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter)
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.UpdateStatus, this);
        }

        public override void OnReceiveNotification(KeyboardResponse response)
        {
            try
            {
                var payload = response.Frame.Payload;
                if (payload.Length != TotalPayloadSize)
                {
                    throw new InvalidOperationException($"Payload size must be <{TotalPayloadSize}> but it's <{payload.Length}>");
                }

                var component = payload
                    .Skip(0)
                    .Take(ComponentSize)
                    .ToList();

                var type = payload
                    .Skip(ComponentSize)
                    .Take(TypeSize)
                    .ToList();

                var progressPayload = payload
                    .Skip(ComponentSize + TypeSize)
                    .Take(PayloadSize)
                    .ToList();

                RaiseEvent(component, type, progressPayload);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Update progress notification received but parse failed");
            }
        }

        private void RaiseEvent(List<byte> component, List<byte> type, List<byte> data)
        {
            var updateType = (UpdateStatusType)type.First();
            var updateComponent = (UpdateComponent)component.First();

            switch (updateType)
            {
                case UpdateStatusType.InProgress:
                    var inProgressEventArgs = CreateInProgressEventArgs(updateComponent, data);
                    OnUpdateProgressChanged?.Invoke(this, inProgressEventArgs);
                    break;
                case UpdateStatusType.Rollback:
                    var rollbackEventArgs = CreateRollbackEventArgs(updateComponent, data);
                    OnRollbackProgressChanged?.Invoke(this, rollbackEventArgs);
                    break;
                case UpdateStatusType.Failed:
                    var failedEventArgs = CreateFailedEventArgs(updateComponent, data);
                    OnUpdateFailed?.Invoke(this, failedEventArgs);
                    break;
                default:
                    throw new InvalidOperationException($"Type <{type}> is unknown");
            }
        }

        private FailedUpdateProgressEventArgs CreateFailedEventArgs(UpdateComponent component, List<byte> data)
        {
            var keyboardErrorCode = (KeyboardErrorCode)data.First();
            var errorCode = _errorConverter.Convert(keyboardErrorCode);

            return new FailedUpdateProgressEventArgs(component, errorCode);
        }

        private InProgressUpdateProgressEventArgs CreateInProgressEventArgs(UpdateComponent component, List<byte> data)
        {
            var progression = (int)data.First();

            return new InProgressUpdateProgressEventArgs(component, progression);
        }

        private RollbackUpdateProgressEventArgs CreateRollbackEventArgs(UpdateComponent component, List<byte> data)
        {
            var state = (int)data.First();

            return new RollbackUpdateProgressEventArgs(component, state);
        }
    }
}
