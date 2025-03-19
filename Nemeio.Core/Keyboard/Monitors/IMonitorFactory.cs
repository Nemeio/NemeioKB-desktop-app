using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.BatteryElectricalStatus;
using Nemeio.Core.Keyboard.CheckIfBatteryPresent;
using Nemeio.Core.Keyboard.ClearScreen;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.DisplayCheckerBoard;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.GetLayouts;
using Nemeio.Core.Keyboard.KeepAlive;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.LayoutsIds;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Keyboard.PressedKeys;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.SetAdvertising;
using Nemeio.Core.Keyboard.SetFrontLightPower;
using Nemeio.Core.Keyboard.SetLed;
using Nemeio.Core.Keyboard.SetProvisionning;
using Nemeio.Core.Keyboard.TestBenchId.Get;
using Nemeio.Core.Keyboard.TestBenchId.Set;
using Nemeio.Core.Keyboard.Updates;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.Keyboard.ExitElectricalTests;
using Nemeio.Core.Keyboard.ExitFunctionalTests;
using Nemeio.Core.Keyboard.Version;

namespace Nemeio.Core.Keyboard.Monitors
{
    public interface IMonitorFactory
    {
        IBatteryMonitor CreateBatteryMonitor(IKeyboardCommandExecutor commandExecutor);
        IKeepAliveMonitor CreateKeepAliveMonitor(IKeyboardCommandExecutor commandExecutor);
        IAddConfigurationMonitor CreateAddConfigurationMonitor(IKeyboardCommandExecutor commandExecutor);
        IDeleteConfigurationMonitor CreateDeleteConfigurationMonitor(IKeyboardCommandExecutor commandExecutor);
        IApplyConfigurationMonitor CreateApplyConfigurationMonitor(IKeyboardCommandExecutor commandExecutor);
        ISerialNumberMonitor CreateSerialNumberMonitor(IKeyboardCommandExecutor commandExecutor);
        IVersionMonitor CreateVersionMonitor(IKeyboardCommandExecutor commandExecutor);
        IKeyboardFailuresMonitor CreateKeyboardFailuresMonitor(IKeyboardCommandExecutor commandExecutor);
        ILayoutHashMonitor CreateLayoutHashMonitor(IKeyboardCommandExecutor commandExecutor);
        ICommunicationModeMonitor CreateCommunicationModeMonitor(IKeyboardCommandExecutor commandExecutor);
        IConfigurationChangedMonitor CreateConfigurationChangedMonitor(IKeyboardCommandExecutor commandExecutor);
        IKeyPressedMonitor CreateKeyPressedMonitor(IKeyboardCommandExecutor commandExecutor);
        IParametersMonitor CreateParametersMonitor(System.Version keyboardProtocolVersion, IKeyboardCommandExecutor commandExecutor);
        IFactoryResetMonitor CreateFactoryResetMonitor(IKeyboardCommandExecutor commandExecutor);
        IUpdateMonitor CreateUpdateMonitor(IKeyboardCommandExecutor commandExecutor);
        IGetLayoutsMonitor CreateGetLayoutsMonitor(IKeyboardCommandExecutor commandExecutor);
        IUpdateProgressMonitor CreateUpdateProgressMonitor(IKeyboardCommandExecutor commandExecutor);
        IGetBatteryElectricalStatusMonitor CreateGetBatteryStatusMonitor(IKeyboardCommandExecutor commandExecutor);
        ISetLedMonitor CreateSetLedMonitor(IKeyboardCommandExecutor commandExecutor);
        IExitElectricalTestsMonitor CreateExitElectricalTestsMonitor(IKeyboardCommandExecutor commandExecutor);
        ISetFrontLightPowerMonitor CreateSetFrontLightPowerMonitor(IKeyboardCommandExecutor commandExecutor);
        ICheckComponentsMonitor CreateCheckComponentsMonitor(IKeyboardCommandExecutor commandExecutor);
        IClearScreenMonitor CreateClearScreenMonitor(IKeyboardCommandExecutor commandExecutor);
        ISetTestBenchIdMonitor CreateSetTestBenchIdMonitor(IKeyboardCommandExecutor commandExecutor);
        IGetTestBenchIdMonitor CreateGetTestBenchIdMonitor(IKeyboardCommandExecutor commandExecutor);
        ISetProvisionningMonitor CreateSetProvisionningMonitor(IKeyboardCommandExecutor commandExecutor);
        IDisplayCheckerBoardMonitor CreateDisplayCheckerBoardMonitor(IKeyboardCommandExecutor commandExecutor);
        ISetAdvertisingMonitor CreateSetAdvertisingMonitor(IKeyboardCommandExecutor commandExecutor);
        IExitFunctionalTestsMonitor CreateExitFunctionalTestsMonitor(IKeyboardCommandExecutor commandExecutor);
        IGetPressedKeysMonitor CreateGetPressedKeysMonitor(IKeyboardCommandExecutor commandExecutor);
        ICheckIfBatteryPresentMonitor CreateCheckIfBatteryPresentMonitor(IKeyboardCommandExecutor commandExecutor);
    }
}
