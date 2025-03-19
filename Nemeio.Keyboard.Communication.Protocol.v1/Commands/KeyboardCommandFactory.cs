using Nemeio.Core;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Keyboard.ReceiveData;
using Nemeio.Core.Services.Layouts;
using Nemeio.Keyboard.Communication.Tools.Utils;
using System.Net.NetworkInformation;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class KeyboardCommandFactory : IKeyboardCommandFactory
    {
        private IReceiveData _sysFailLog = new SysFailLog();
        private IReceiveData _getLayout = new GetLayouts();

        public IKeyboardCommand CreateBatteryCommand() => new BatteryCommand();
        public IKeyboardCommand CreateKeepAliveCommand() => new KeepAliveCommand();
        public IKeyboardCommand CreateKeyboardFailuresEndCommand() => new ReceiveDataEndCommand(_sysFailLog);
        public IKeyboardCommand CreateKeyboardFailuresPulpCommand(uint sizeToReceive, uint offset) => new ReceiveDataPulpCommand(_sysFailLog, sizeToReceive, offset);
        public IKeyboardCommand CreateKeyboardFailuresStartCommand() => new ReceiveDataStartCommand(_sysFailLog);
        public IKeyboardCommand CreateLayoutIdsCommand() => new LayoutIdsCommand();
        public IKeyboardCommand CreateSendConfigurationCommand(ILayout layout, bool isFactoryLayout) => new SendConfigurationCommand(layout, isFactoryLayout);
        public IKeyboardCommand CreateSerialNumberCommand() => new SerialNumberCommand();
        public IKeyboardCommand CreateVersionCommand() => new VersionCommand();
        public IKeyboardCommand CreateDeleteConfigurationCommand(LayoutId id) => new DeleteConfigurationCommand(id);
        public IKeyboardCommand CreateApplyConfigurationCommand(LayoutId id) => new ApplyConfigurationCommand(id);
        public IKeyboardCommand CreateSetCommunicationModeCommand(KeyboardCommunicationMode mode) => new SetCommunicationModeCommand(mode);
        public IKeyboardCommand CreateGetParametersCommand() => new GetParametersCommand();
        public IKeyboardCommand CreateSetParametersCommand(KeyboardParameters parameters, IKeyboardParameterParser parser) => new SetParameterCommand(parameters, parser);
        public IKeyboardCommand CreateFactoryResetCommand() => new FactoryResetCommand();
        public IKeyboardCommand CreateSendFirmwareCommand(byte[] firmware) => new SendFirmwareCommand(firmware);
        public IKeyboardCommand CreateGetLayoutsStartCommand() => new ReceiveDataStartCommand(_getLayout);
        public IKeyboardCommand CreateGetLayoutsPulpCommand(uint sizeToReceive, uint offset) => new ReceiveDataPulpCommand(_getLayout, sizeToReceive, offset);
        public IKeyboardCommand CreateGetLayoutsEndCommand() => new ReceiveDataEndCommand(_getLayout);
        public IKeyboardCommand CreateGetBatteryElectricalStateCommand() => new GetBatteryElectricalStatusCommand();
        public IKeyboardCommand CreateSetLedCommand(byte ledId, byte ledState) => new SetLedCommand(ledId, ledState);
        public IKeyboardCommand CreateExitElectricalTestsCommand(byte validateState) => new ExitElectricalTestsCommand(validateState);
        public IKeyboardCommand CreateSetFrontLightPowerCommand(byte power) => new SetFrontLightPowerCommand(power);
        public IKeyboardCommand CreateCheckComponentsCommand(byte componentId) => new CheckComponentsCommand(componentId);
        public IKeyboardCommand CreateClearScreenCommand() => new ClearScreenCommand();
        public IKeyboardCommand CreateSetTestBenchIdCommand(string testId) => new SetTestBenchIdCommand(testId);
        public IKeyboardCommand CreateGetTestBenchIdCommand() => new GetTestBenchIdCommand();
        public IKeyboardCommand CreateSetProvisionningCommand(string serial, PhysicalAddress mac) => new SetProvisionningCommand(serial, mac);
        public IKeyboardCommand CreateSetAdvertisingCommand(byte enable) => new SetAdvertisingCommand(enable);
        public IKeyboardCommand CreateExitFunctionalTestsCommand(byte validate) => new ExitFunctionalTestsCommand(validate);
        public IKeyboardCommand CreateDisplayCheckerBoardCommand(byte firstColor) => new DisplayCheckerBoardCommand(firstColor);
        public IKeyboardCommand CreateGetPressedKeysCommand() => new GetPressedKeysCommand();
        public IKeyboardCommand CreateCheckIfBatteryPresentCommand() => new CheckIfBatteryPresentCommand();
        
    }
}
