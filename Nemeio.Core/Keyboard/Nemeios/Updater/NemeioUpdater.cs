using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Keyboard.Updates;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.PackageUpdater.Firmware;
using Nemeio.Core.Tools.Retry;

namespace Nemeio.Core.Keyboard.Nemeios.Updater
{
    public class NemeioUpdater : Nemeio, IUpdateHolder
    {
        private readonly IUpdateMonitor _updateMonitor;
        private readonly IUpdateProgressMonitor _updateProgressMonitor;

        public event EventHandler<InProgressUpdateProgressEventArgs> OnUpdateProgressChanged;
        public event EventHandler<FailedUpdateProgressEventArgs> OnUpdateFailed;
        public event EventHandler<RollbackUpdateProgressEventArgs> OnRollbackProgressChanged;

        public NemeioUpdater(ILoggerFactory loggerFactory, string identifier, System.Version protocolVersion, CommunicationType communication, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler) 
            : base(loggerFactory, identifier, protocolVersion, communication, commandExecutor, monitorFactory, crashLogger, retryHandler) 
        {
            _updateMonitor = monitorFactory.CreateUpdateMonitor(commandExecutor);
            _updateProgressMonitor = monitorFactory.CreateUpdateProgressMonitor(commandExecutor);
            _updateProgressMonitor.OnUpdateProgressChanged += UpdateProgressMonitor_OnUpdateProgressChanged;
            _updateProgressMonitor.OnRollbackProgressChanged += UpdateProgressMonitor_OnRollbackProgressChanged;
            _updateProgressMonitor.OnUpdateFailed += UpdateProgressMonitor_OnUpdateFailed;

            _stateMachine.Configure(NemeioState.Connected)
                .Permit(NemeioTrigger.Initialize, NemeioState.Init)
                .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting);

            _stateMachine.Configure(NemeioState.Init)
                .SubstateOf(NemeioState.Connected)
                .Permit(NemeioTrigger.KeyboardInitialized, NemeioState.Ready)
                .OnEntryAsync(InitOnEntryAsync);

            _stateMachine.Configure(NemeioState.Ready)
                .SubstateOf(NemeioState.Connected);
        }

        protected override async Task InitKeyboardAsync()
        {
            await Task.Yield();

            //  Nothing to do here
        }

        #region Update

        /// <summary>
        /// Allow to update keyboard
        /// </summary>
        /// <exception cref="UpdateNemeioFailedException">Throw if can't update keyboard</exception>
        public async Task UpdateAsync(IFirmware package)
        {
            await Task.Yield();

            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            try
            {
                _updateMonitor.SendFirmware(package.ToByteArray());
            }
            catch (KeyboardException exception)
            {
                var message = $"Update keyboard failed for keyboard <{Identifier}>";

                _logger.LogError(exception, message);

                throw new UpdateNemeioFailedException(message, exception);
            }
            catch (KeyboardCommunicationException exception)
            {
                var message = $"Update keyboard failed for keyboard <{Identifier}> because of communication issue";

                _logger.LogError(exception, message);

                throw new UpdateNemeioFailedException(message, exception);
            }
        }

        private void UpdateProgressMonitor_OnUpdateProgressChanged(object sender, InProgressUpdateProgressEventArgs e)
        {
            _logger.LogInformation($"Component=<{e.Component}>, Type=<{e.Type}>, Progress=<{e.Progress}>");

            OnUpdateProgressChanged?.Invoke(this, e);
        }

        private void UpdateProgressMonitor_OnRollbackProgressChanged(object sender, RollbackUpdateProgressEventArgs e)
        {
            _logger.LogInformation($"Component=<{e.Component}>, Type=<{e.Type}>, IsSuccess=<{e.IsSuccess}>");

            OnRollbackProgressChanged?.Invoke(this, e);
        }

        private void UpdateProgressMonitor_OnUpdateFailed(object sender, FailedUpdateProgressEventArgs e)
        {
            _logger.LogInformation($"Component=<{e.Component}>, Type=<{e.Type}>, Error=<{e.Error}>");

            OnUpdateFailed?.Invoke(this, e);
        }

        #endregion
    }
}
