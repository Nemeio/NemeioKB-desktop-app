using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreServices;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Tools.Stoppable;
using Nemeio.Core.Tools.Watchers;

namespace Nemeio.Platform.Mac.Tools.Watchers
{
    public class MacFileWatcher : Stoppable, IWatcher, IDisposable
    {
        private readonly string _filePath;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly FSEventStream _eventStream;

        public event EventHandler OnChanged;

        public MacFileWatcher(ILoggerFactory loggerFactory, IFileSystem fileSystem, string filePath)
            : base()
        {
            _logger = loggerFactory.CreateLogger<MacFileWatcher>();
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _cancellationTokenSource = new CancellationTokenSource();

            if (!fileSystem.FileExists(filePath))
            {
                throw new ArgumentNullException($"<{filePath}> must exists");
            }

            _filePath = filePath;

            var latency = TimeSpan.FromSeconds(3);

            _eventStream = new FSEventStream(new[] { _filePath }, latency, FSEventStreamCreateFlags.FileEvents);
            _eventStream.Events += EventStream_Events; ;
            _eventStream.ScheduleWithRunLoop(NSRunLoop.Current);
            _eventStream.Start();

        }

        private void EventStream_Events(object sender, FSEventStreamEventsArgs args)
        {
            _logger.LogInformation($"EventStream_Events <{args.Events.Length}>");

            if (_fileSystem.IsReady(_filePath))
            {
                _logger.LogInformation($"Something change on file <{_filePath}>");

                OnChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_eventStream != null)
            {
                _eventStream.Stop();
                _eventStream.Dispose();
            }
        }
    }
}
