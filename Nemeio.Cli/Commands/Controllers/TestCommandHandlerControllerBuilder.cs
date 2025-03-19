using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Keyboards.Commands.TestBench.BatteryElectricalStatus;
using Nemeio.Cli.Keyboards.Commands.TestBench.ElectricalTests;
using Nemeio.Cli.Keyboards.Commands.TestBench.Led;
using Nemeio.Cli.Keyboards.Commands.TestBench.FrontLightPower;
using Nemeio.Cli.Package.Update.Commands.Read;
using Nemeio.Cli.Keyboards.Commands.TestBench.CheckComponents;
using Nemeio.Cli.Keyboards.Commands.TestBench.ClearScreen;
using Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Set;
using Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Get;
using Nemeio.Cli.Keyboards.Commands.TestBench.PressedKeys;
using Nemeio.Cli.Keyboards.Commands.TestBench.SetProvisionning;
using Nemeio.Cli.Keyboards.Commands.TestBench.DisplayCheckerBoard;
using Nemeio.Cli.Keyboards.Commands.TestBench.SetAdvertising;
using Nemeio.Cli.Keyboards.Commands.TestBench.FunctionalTests;
using Nemeio.Cli.Keyboards.Commands.TestBench.CheckIfBatteryPresent;

namespace Nemeio.Cli.Commands.Controllers
{
    internal sealed class TestCommandHandlerControllerBuilder : ITestCommandHandlerControllerBuilder
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISetLedCommandHandler _setLedCommandHandler;
        private readonly IExitElectricalTestsCommandHandler _exitElectricalTestsCommandHandler;
        private readonly IGetBatteryElectricalStatusCommandHandler _getBatteryElectricalStatusCommandHandler;
        private readonly ICheckComponentsCommandHandler _checkComponentsCommandHandler;
        private readonly IClearScreenCommandHandler _clearScreenCommandHandler;
        private readonly ISetFrontLightPowerCommandHandler _setFrontLightPowerCommandHandler;
        private readonly ISetTestBenchIdCommandHandler _setTestBenchIdCommandHandler;
        private readonly IGetTestBenchIdCommandHandler _getTestBenchIdCommandHandler;
        private readonly IGetPressedKeysCommandHandler _getPressedKeysCommandHandler;
        private readonly ISetProvisionningCommandHandler _setProvisionningCommandHandler;
        private readonly IDisplayCheckerBoardCommandHandler _displayCheckerBoardCommandHandler;
        private readonly ISetAdvertisingCommandHandler _setAdvertisingCommandHandler;
        private readonly IExitFunctionalTestsCommandHandler _exitFunctionalTestsCommandHandler;
        private readonly ICheckIfBatteryPresentCommandHandler _checkIfBatteryPresentCommandHandler;
        private ITestCommandHandlerController _handlerController;






        public TestCommandHandlerControllerBuilder(ILoggerFactory loggerFactory, ISetLedCommandHandler setLedCommandHandler,
            IExitElectricalTestsCommandHandler exitElectricalTestsCommandHandler, ISetFrontLightPowerCommandHandler setFrontLightPowerCommandHandler,
            IGetBatteryElectricalStatusCommandHandler getBatteryElectricalStatusCommandHandler, IClearScreenCommandHandler clearScreenCommandHandler,
            ICheckComponentsCommandHandler checkComponentsCommandHandler, ISetTestBenchIdCommandHandler setTestBenchIdCommandHandler,
            ISetProvisionningCommandHandler setProvisionningCommandHandler, IGetTestBenchIdCommandHandler getTestBenchIdCommandHandler,
             IExitFunctionalTestsCommandHandler exitFunctionalTestsCommandHandler, ISetAdvertisingCommandHandler setAdvertisingCommandHandler,
             IDisplayCheckerBoardCommandHandler displayCheckerBoardCommandHandler, IGetPressedKeysCommandHandler getPressedKeysCommandHandler,
            ICheckIfBatteryPresentCommandHandler checkIfBatteryPresentCommandHandler)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _setLedCommandHandler = setLedCommandHandler ?? throw new ArgumentNullException(nameof(setLedCommandHandler));
            _setFrontLightPowerCommandHandler = setFrontLightPowerCommandHandler ?? throw new ArgumentNullException(nameof(setFrontLightPowerCommandHandler));
            _exitElectricalTestsCommandHandler = exitElectricalTestsCommandHandler ?? throw new ArgumentNullException(nameof(exitElectricalTestsCommandHandler));
            _getBatteryElectricalStatusCommandHandler = getBatteryElectricalStatusCommandHandler ?? throw new ArgumentNullException(nameof(getBatteryElectricalStatusCommandHandler));
            _clearScreenCommandHandler = clearScreenCommandHandler ?? throw new ArgumentNullException(nameof(clearScreenCommandHandler));
            _setTestBenchIdCommandHandler = setTestBenchIdCommandHandler ?? throw new ArgumentNullException(nameof(setTestBenchIdCommandHandler));
            _getTestBenchIdCommandHandler = getTestBenchIdCommandHandler ?? throw new ArgumentNullException(nameof(getTestBenchIdCommandHandler));
            _checkComponentsCommandHandler = checkComponentsCommandHandler ?? throw new ArgumentNullException(nameof(checkComponentsCommandHandler));
            _getPressedKeysCommandHandler = getPressedKeysCommandHandler ?? throw new ArgumentNullException(nameof(getPressedKeysCommandHandler));
            _setProvisionningCommandHandler = setProvisionningCommandHandler ?? throw new ArgumentNullException(nameof(setProvisionningCommandHandler));
            _displayCheckerBoardCommandHandler = displayCheckerBoardCommandHandler ?? throw new ArgumentNullException(nameof(displayCheckerBoardCommandHandler));
            _setAdvertisingCommandHandler = setAdvertisingCommandHandler ?? throw new ArgumentNullException(nameof(setAdvertisingCommandHandler));
            _exitFunctionalTestsCommandHandler = exitFunctionalTestsCommandHandler ?? throw new ArgumentNullException(nameof(exitFunctionalTestsCommandHandler));
            _checkIfBatteryPresentCommandHandler = checkIfBatteryPresentCommandHandler ?? throw new ArgumentNullException(nameof(checkIfBatteryPresentCommandHandler));
        }

        public ITestCommandHandlerController BuildOrGet()
        {
            if (_handlerController == null)
            {
                _handlerController = Build();
            }

            return _handlerController;
        }

        private ITestCommandHandlerController Build()
        {
            var handlers = new List<ICommandHandler>()
            {
                _setLedCommandHandler,
                _exitElectricalTestsCommandHandler,
                _setFrontLightPowerCommandHandler,
                _getBatteryElectricalStatusCommandHandler,
                _checkComponentsCommandHandler,
                _clearScreenCommandHandler,
                _setTestBenchIdCommandHandler,
                _getTestBenchIdCommandHandler,
                _setProvisionningCommandHandler,
                _displayCheckerBoardCommandHandler,
                _setAdvertisingCommandHandler,
                _exitFunctionalTestsCommandHandler,
                _getPressedKeysCommandHandler,
                _checkIfBatteryPresentCommandHandler,
            };

            var handlerController = new TestCommandHandlerController(_loggerFactory, handlers);

            return handlerController;
        }
    }
}
