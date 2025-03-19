using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Core.Tools.Retry;
using Nemeio.Core.Tools;
using Nemeio.Core.FileSystem;

namespace Nemeio.Keyboard.Communication.Linux.Watchers
{
    public sealed class LinuxKeyboardWatcherFactory : IKeyboardWatcherFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRetryHandler _retryHandler;
        private readonly IKeyboardVersionParser _versionParser;
        private readonly IFileSystem _fileSystem;

        public LinuxKeyboardWatcherFactory(ILoggerFactory loggerFactory, IRetryHandler retryHandler, IKeyboardVersionParser versionParser, IFileSystem fileSystem)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));
            _versionParser = versionParser ?? throw new ArgumentNullException(nameof(versionParser));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IEnumerable<IKeyboardWatcher> CreateWatchers()
        {
            return new List<IKeyboardWatcher>()
            {
                new LinuxSerialKeyboardWatcher(_loggerFactory, _retryHandler, _versionParser, _fileSystem)
            };
        }
    }
}
