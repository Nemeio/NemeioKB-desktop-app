using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Tools.Retry;

namespace Nemeio.Core.Keyboard.Nemeios.VersionChecker
{
    public sealed class NemeioVersionChecker : Nemeio
    {
        public NemeioVersionChecker(ILoggerFactory loggerFactory, string identifier, System.Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler) 
            : base(loggerFactory, identifier, protocolVersion, type, commandExecutor, monitorFactory, crashLogger, retryHandler) 
        {
            _stateMachine.Configure(NemeioState.Connected)
                .Permit(NemeioTrigger.Initialize, NemeioState.Init)
                .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting);

            _stateMachine.Configure(NemeioState.Init)
                .SubstateOf(NemeioState.Connected)
                .Permit(NemeioTrigger.KeyboardInitialized, NemeioState.Ready)
                .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting)
                .OnEntryAsync(InitOnEntryAsync);

            _stateMachine.Configure(NemeioState.Ready)
                .SubstateOf(NemeioState.Connected)
                .Ignore(NemeioTrigger.RefreshKeepAlive)
                .Ignore(NemeioTrigger.RefreshBattery)
                .Ignore(NemeioTrigger.StartSync);

            _stateMachine.Configure(NemeioState.Disconnecting);
        }

        protected override async Task InitKeyboardAsync()
        {
            await Task.Yield();

            //  Nothing to do here
        }
    }
}
