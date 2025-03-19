using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Services.Layouts;
using System.Net.NetworkInformation;

namespace Nemeio.Core.Keyboard.Communication.Commands
{
    public interface IKeyboardCommandFactory
    {
        IKeyboardCommand CreateBatteryCommand();
        IKeyboardCommand CreateKeepAliveCommand();
        IKeyboardCommand CreateVersionCommand();
        IKeyboardCommand CreateSerialNumberCommand();
        IKeyboardCommand CreateKeyboardFailuresStartCommand();
        IKeyboardCommand CreateKeyboardFailuresPulpCommand(uint sizeToReceive, uint offset);
        IKeyboardCommand CreateKeyboardFailuresEndCommand();
        IKeyboardCommand CreateLayoutIdsCommand();
        IKeyboardCommand CreateSendConfigurationCommand(ILayout layout, bool isFactoryLayout);
        IKeyboardCommand CreateDeleteConfigurationCommand(LayoutId hash);
        IKeyboardCommand CreateApplyConfigurationCommand(LayoutId hash);
        IKeyboardCommand CreateSetCommunicationModeCommand(KeyboardCommunicationMode mode);
        IKeyboardCommand CreateGetParametersCommand();

        IKeyboardCommand CreateSetParametersCommand(KeyboardParameters parameters, IKeyboardParameterParser parser);
        IKeyboardCommand CreateFactoryResetCommand();
        IKeyboardCommand CreateSendFirmwareCommand(byte[] firmware);
        IKeyboardCommand CreateGetLayoutsEndCommand();
        IKeyboardCommand CreateGetLayoutsPulpCommand(uint sizeToReceive, uint offset);
        IKeyboardCommand CreateGetLayoutsStartCommand();
        IKeyboardCommand CreateGetBatteryElectricalStateCommand();
        IKeyboardCommand CreateSetFrontLightPowerCommand(byte power);
        IKeyboardCommand CreateSetLedCommand(byte ledId, byte ledState);
        IKeyboardCommand CreateExitElectricalTestsCommand(byte validateState);
        IKeyboardCommand CreateSetTestBenchIdCommand(string testId);
        IKeyboardCommand CreateGetTestBenchIdCommand();
        IKeyboardCommand CreateCheckComponentsCommand(byte componentId);
        IKeyboardCommand CreateClearScreenCommand();
        IKeyboardCommand CreateSetProvisionningCommand(string serial, PhysicalAddress mac);
        IKeyboardCommand CreateSetAdvertisingCommand(byte enable);
        IKeyboardCommand CreateExitFunctionalTestsCommand(byte validate);
        IKeyboardCommand CreateGetPressedKeysCommand();
        IKeyboardCommand CreateCheckIfBatteryPresentCommand();
        
        IKeyboardCommand CreateDisplayCheckerBoardCommand(byte firstColor);

    }
}
