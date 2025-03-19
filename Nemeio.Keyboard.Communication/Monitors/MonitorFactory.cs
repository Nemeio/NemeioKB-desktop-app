using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.BatteryElectricalStatus;
using Nemeio.Core.Keyboard.CheckIfBatteryPresent;
using Nemeio.Core.Keyboard.ClearScreen;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
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
using Nemeio.Core.Keyboard.Monitors;
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
using Nemeio.Keyboard.Communication.Protocol.v1.Monitors;

namespace Nemeio.Keyboard.Communication.Monitors
{
    public class MonitorFactory : IMonitorFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IKeyboardCommandFactory _keyboardCommandFactory;
        private readonly IKeyboardErrorConverter _errorConverter;
        
        public MonitorFactory(ILoggerFactory loggerFactory, IKeyboardCommandFactory keyboardCommandFactory, IKeyboardErrorConverter errorConverter)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _keyboardCommandFactory = keyboardCommandFactory ?? throw new ArgumentNullException(nameof(keyboardCommandFactory));
            _errorConverter = errorConverter ?? throw new ArgumentNullException(nameof(errorConverter));
        }
        public IBatteryMonitor CreateBatteryMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new BatteryMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IKeepAliveMonitor CreateKeepAliveMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new KeepAliveMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ISerialNumberMonitor CreateSerialNumberMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new SerialNumberMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IVersionMonitor CreateVersionMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new VersionMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IKeyboardFailuresMonitor CreateKeyboardFailuresMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new KeyboardFailuresMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ILayoutHashMonitor CreateLayoutHashMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new LayoutsHashMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IAddConfigurationMonitor CreateAddConfigurationMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new AddConfigurationMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IDeleteConfigurationMonitor CreateDeleteConfigurationMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new DeleteConfigurationMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IApplyConfigurationMonitor CreateApplyConfigurationMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new ApplyConfigurationMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ICommunicationModeMonitor CreateCommunicationModeMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new CommunicationModeMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IConfigurationChangedMonitor CreateConfigurationChangedMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new ConfigurationChangedMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IParametersMonitor CreateParametersMonitor(Version keyboardProtocolVersion, IKeyboardCommandExecutor commandExecutor)
        {
            IKeyboardParameterParser parser = null;

            switch (keyboardProtocolVersion.Minor)
            {
                case 1:
                    parser = new Protocol.v1.Utils.KeyboardParameterConverter();
                    break;
                case 2:
                    parser = new Protocol.v2.Utils.KeyboardParameterConverter();
                    break;
                case 3:
                default:
                    parser = new Protocol.v3.Utils.KeyboardParameterConverter();
                    break;
            }

            return new ParametersMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter, parser);
        }
        public IKeyPressedMonitor CreateKeyPressedMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new KeyPressedMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IFactoryResetMonitor CreateFactoryResetMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new FactoryResetMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IUpdateMonitor CreateUpdateMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new UpdateMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IGetLayoutsMonitor CreateGetLayoutsMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new GetLayoutsMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IUpdateProgressMonitor CreateUpdateProgressMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new UpdateProgressMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IGetBatteryElectricalStatusMonitor CreateGetBatteryStatusMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new GetBatteryElectricalStatusMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ISetLedMonitor CreateSetLedMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new SetLedMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IExitElectricalTestsMonitor CreateExitElectricalTestsMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new ExitElectricalTestsMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ISetFrontLightPowerMonitor CreateSetFrontLightPowerMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new SetFrontLightPowerMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ICheckComponentsMonitor CreateCheckComponentsMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new CheckComponentsMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);

        }
        public IClearScreenMonitor CreateClearScreenMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new ClearScreenMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ISetTestBenchIdMonitor CreateSetTestBenchIdMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new SetTestBenchIdMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IGetTestBenchIdMonitor CreateGetTestBenchIdMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new GetTestBenchIdMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }

        public ISetProvisionningMonitor CreateSetProvisionningMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new SetProvisionningMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ISetAdvertisingMonitor CreateSetAdvertisingMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new SetAdvertisingMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IExitFunctionalTestsMonitor CreateExitFunctionalTestsMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new ExitFunctionalTestsMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IDisplayCheckerBoardMonitor CreateDisplayCheckerBoardMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new DisplayCheckerBoardMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public IGetPressedKeysMonitor CreateGetPressedKeysMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new GetPressedKeysMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
        public ICheckIfBatteryPresentMonitor CreateCheckIfBatteryPresentMonitor(IKeyboardCommandExecutor commandExecutor)
        {
            return new CheckIfBatteryPresentMonitor(_loggerFactory, _keyboardCommandFactory, commandExecutor, _errorConverter);
        }
    }
}
