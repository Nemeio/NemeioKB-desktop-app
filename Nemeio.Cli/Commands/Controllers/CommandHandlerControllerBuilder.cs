using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Cli.Keyboards.Commands.Configurations.Add;
using Nemeio.Cli.Keyboards.Commands.Configurations.Apply;
using Nemeio.Cli.Keyboards.Commands.Configurations.Change;
using Nemeio.Cli.Keyboards.Commands.Configurations.Delete;
using Nemeio.Cli.Keyboards.Commands.Configurations.List;
using Nemeio.Cli.Keyboards.Commands.Crashes;
using Nemeio.Cli.Keyboards.Commands.FactoryReset;
using Nemeio.Cli.Keyboards.Commands.Parameters.Get;
using Nemeio.Cli.Keyboards.Commands.Parameters.Set;
using Nemeio.Cli.Keyboards.Commands.Update;
using Nemeio.Cli.Keyboards.Commands.Version;
using Nemeio.Cli.Package.Update.Commands.Read;

namespace Nemeio.Cli.Commands.Controllers
{
    internal sealed class CommandHandlerControllerBuilder : ICommandHandlerControllerBuilder
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IUpdateCommandHandler _updateCommandHandler;
        private readonly IVersionCommandHandler _versionCommandHandler;
        private readonly IFactoryResetCommandHandler _factoryResetCommandHandler;
        private readonly IDeleteLayoutCommandHandler _deleteLayoutCommandHandler;
        private readonly IListLayoutCommandHandler _listLayoutCommandHandler;
        private readonly IApplyLayoutCommandHandler _applyLayoutCommandHandler;
        private readonly IAddLayoutCommandHandler _addLayoutCommandHandler;
        private readonly IChangeLayoutListenerCommandHandler _changeLayoutCommandHandler;
        private readonly IGetParametersCommandHandler _getParametersCommandHandler;
        private readonly ISetParameterCommandHandler _setParameterCommandHandler;
        private readonly IReadUpdatePackageCommandHandler _readUpdatePackageCommandHandler;
        private readonly ICrashesCommandHandler _crashesCommandHandler;

        private ICommandHandlerController _handlerController;

        public CommandHandlerControllerBuilder(ILoggerFactory loggerFactory, IUpdateCommandHandler updateCommandHandler, IVersionCommandHandler versionCommandHandler, IFactoryResetCommandHandler factoryResetCommandHandler, IDeleteLayoutCommandHandler deleteLayoutCommandHandler, IListLayoutCommandHandler listLayoutCommandHandler, IApplyLayoutCommandHandler applyLayoutCommandHandler, IAddLayoutCommandHandler addLayoutCommandHandler, IChangeLayoutListenerCommandHandler changeLayoutCommandHandler, IGetParametersCommandHandler getParametersCommandHandler, ISetParameterCommandHandler setParameterCommandHandler, IReadUpdatePackageCommandHandler readUpdatePackageCommandHandler, ICrashesCommandHandler crashesCommandHandler)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _updateCommandHandler = updateCommandHandler ?? throw new ArgumentNullException(nameof(updateCommandHandler));
            _versionCommandHandler = versionCommandHandler ?? throw new ArgumentNullException(nameof(versionCommandHandler));
            _factoryResetCommandHandler = factoryResetCommandHandler ?? throw new ArgumentNullException(nameof(factoryResetCommandHandler));
            _getParametersCommandHandler = getParametersCommandHandler ?? throw new ArgumentNullException(nameof(getParametersCommandHandler));
            _deleteLayoutCommandHandler = deleteLayoutCommandHandler ?? throw new ArgumentNullException(nameof(deleteLayoutCommandHandler));
            _listLayoutCommandHandler = listLayoutCommandHandler ?? throw new ArgumentNullException(nameof(listLayoutCommandHandler));
            _applyLayoutCommandHandler = applyLayoutCommandHandler ?? throw new ArgumentNullException(nameof(applyLayoutCommandHandler));
            _addLayoutCommandHandler = addLayoutCommandHandler ?? throw new ArgumentNullException(nameof(addLayoutCommandHandler));
            _changeLayoutCommandHandler = changeLayoutCommandHandler ?? throw new ArgumentNullException(nameof(changeLayoutCommandHandler));
            _setParameterCommandHandler = setParameterCommandHandler ?? throw new ArgumentNullException(nameof(setParameterCommandHandler));
            _readUpdatePackageCommandHandler = readUpdatePackageCommandHandler ?? throw new ArgumentNullException(nameof(readUpdatePackageCommandHandler));
            _crashesCommandHandler = crashesCommandHandler ?? throw new ArgumentNullException(nameof(crashesCommandHandler));
        }

        public ICommandHandlerController BuildOrGet()
        {
            if (_handlerController == null)
            {
                _handlerController = Build();
            }

            return _handlerController;
        }

        private ICommandHandlerController Build()
        {
            var handlers = new List<ICommandHandler>()
            {
                _updateCommandHandler,
                _versionCommandHandler,
                _factoryResetCommandHandler,
                _deleteLayoutCommandHandler,
                _listLayoutCommandHandler,
                _applyLayoutCommandHandler,
                _addLayoutCommandHandler,
                _changeLayoutCommandHandler,
                _getParametersCommandHandler,
                _setParameterCommandHandler,
                _readUpdatePackageCommandHandler,
                _crashesCommandHandler
            };

            var handlerController = new CommandHandlerController(_loggerFactory, handlers);

            return handlerController;
        }
    }
}
