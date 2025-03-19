using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Sessions;

namespace Nemeio.Core.Layouts.Active.Requests.Factories
{
    public sealed class ChangeRequestFactory : IChangeRequestFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISystem _system;
        private readonly ILayoutLibrary _library;
        private readonly ILanguageManager _languageManager;
        private readonly IApplicationLayoutManager _applicationLayoutManager;

        public ChangeRequestFactory(ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, ILanguageManager languageManager, IApplicationLayoutManager applicationLayoutManager)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _applicationLayoutManager = applicationLayoutManager ?? throw new ArgumentNullException(nameof(applicationLayoutManager));
        }

        public IChangeRequest CreateApplicationShutdownRequest(INemeio nemeio)
        {
            var request = new ApplicationShutdownChangeRequest(_loggerFactory, _system, _library, nemeio);

            return request;
        }

        public IChangeRequest CreateForegroundApplicationRequest(INemeio nemeio, Application application)
        {
            var request = new ForegroundApplicationChangeRequest(application, _loggerFactory, _system, _library, nemeio, _languageManager, _applicationLayoutManager);

            return request;
        }

        public IChangeRequest CreateHidSystemRequest(INemeio nemeio)
        {
            var request = new HidSystemLayoutChangeRequest(_loggerFactory, _system, _library, nemeio);

            return request;
        }

        public IChangeRequest CreateHistoricRequest(INemeio nemeio, bool isBack)
        {
            var request = new HistoricChangeRequest(isBack, _loggerFactory, _system, _library, nemeio);

            return request;
        }

        public IChangeRequest CreateKeyboardSelectionRequest(INemeio nemeio)
        {
            var request = new KeyboardSelectionChangeRequest(_loggerFactory, _system, _library, nemeio);

            return request;
        }

        public IChangeRequest CreateKeyPressRequest(INemeio nemeio, ILayout layout)
        {
            var request = new KeyPressLayoutChangeRequest(layout,_loggerFactory, _system, _library, nemeio);

            return request;
        }

        public IChangeRequest CreateKeyPressRequest(INemeio nemeio, LayoutId id)
        {
            var layout = _library.Layouts.First(x => x.LayoutId.Equals(id));
            var request = CreateKeyPressRequest(nemeio, layout);

            return request;
        }

        public IChangeRequest CreateMenuRequest(INemeio nemeio, ILayout layout)
        {
            var request = new MenuLayoutChangeRequest(layout,_loggerFactory, _system, _library, nemeio);

            return request;
        }

        public IChangeRequest CreateMenuRequest(INemeio nemeio, LayoutId id)
        {
            var layout = _library.Layouts.First(x => x.LayoutId.Equals(id));
            var request = CreateMenuRequest(nemeio, layout);

            return request;
        }

        public IChangeRequest CreateSessionRequest(INemeio nemeio, SessionState state)
        {
            var request = new SessionStateChangeRequest(state,_loggerFactory, _system, _library, nemeio);

            return request;
        }
    }
}
