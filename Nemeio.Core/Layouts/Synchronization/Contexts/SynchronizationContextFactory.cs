using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Synchronization.Contexts
{
    public class SynchronizationContextFactory : ISynchronizationContextFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILayoutFactory _factory;
        private readonly ILayoutLibrary _library;
        private readonly ISystemLayoutLoaderAdapter _systemLayoutLoaderAdapter;
        private readonly ISystem _system;
        public SynchronizationContextFactory(ILoggerFactory loggerFactory, ISystem system, ILayoutFactory factory, ILayoutLibrary library, ISystemLayoutLoaderAdapter loaderAdapter)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _systemLayoutLoaderAdapter = loaderAdapter ?? throw new ArgumentNullException(nameof(loaderAdapter));
        }

        public ISynchronizationContext CreateSystemSynchronizationContext(IScreen screen) => new SystemSynchronizationContext(_loggerFactory, _system, _factory, _library, _systemLayoutLoaderAdapter, screen);

        public ISynchronizationContext CreateKeyboardSynchronizationContext(ISynchronizableNemeioProxy proxy) => new KeyboardSynchronizationContext(_loggerFactory, _library, proxy);
    }
}
