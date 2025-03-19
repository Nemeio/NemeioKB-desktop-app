using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Services;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Keys.Executors
{
    public class KeyExecutorFactory : IKeyExecutorFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISystem _system;
        private readonly IBrowserFile _browserFile;
        private readonly IDialogService _dialogService;
        private readonly IErrorManager _errorManager;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public KeyExecutorFactory(ILoggerFactory loggerFactory, ISystem system, IBrowserFile browserFile, IDialogService dialogService, IErrorManager errorManager, IActiveLayoutChangeHandler activeLayoutChangeHandler)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _browserFile = browserFile ?? throw new ArgumentNullException(nameof(browserFile));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
        }

        public IEnumerable<KeyExecutor> Create(INemeio nemeio, IEnumerable<KeySubAction> subActions)
        {
            if (nemeio == null)
            {
                throw new ArgumentNullException(nameof(nemeio));
            }

            if (subActions == null)
            {
                throw new ArgumentNullException(nameof(subActions));
            }

            //  We consider that it can only have one layout switch
            var alreadyCreateLayoutExecutor = false;
            var unicodeActions = new List<string>();

            foreach (var action in subActions)
            {
                switch (action.Type)
                {
                    case KeyActionType.Unicode:
                    case KeyActionType.Special:
                        unicodeActions.Add(action.Data);
                        break;

                    case KeyActionType.Application:
                        yield return new ApplicationKeyExecutor(_loggerFactory, _browserFile, _dialogService, _errorManager, action.Data);
                        break;

                    case KeyActionType.Url:
                        yield return new UrlKeyExecutor(_loggerFactory, _browserFile, _dialogService, _errorManager, action.Data);
                        break;

                    case KeyActionType.Layout:
                        if (!alreadyCreateLayoutExecutor)
                        {
                            yield return new LayoutKeyExecutor(_loggerFactory, _activeLayoutChangeHandler, _errorManager, nemeio, action.Data);
                        }
                        break;

                    case KeyActionType.Back:
                    case KeyActionType.Forward:
                        yield return new BackForwardKeyExecutor(_loggerFactory, _activeLayoutChangeHandler, nemeio, action.Type == KeyActionType.Back, action.Data);
                        break;

                    default:
                        throw new InvalidOperationException($"Type <{action.Type}> not manage");
                }
            }

            if (unicodeActions != null && unicodeActions.Count > 0)
            {
                yield return new UnicodeKeyExecutor(_loggerFactory, _system, unicodeActions);
            }
        }
    }
}
