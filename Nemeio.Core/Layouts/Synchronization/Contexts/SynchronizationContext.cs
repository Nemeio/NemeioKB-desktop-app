using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Layouts.Synchronization.Contexts
{
    public abstract class SynchronizationContext : ISynchronizationContext
    {
        protected bool _needSync;
        protected readonly ILogger _logger;

        public SynchronizationContext(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SynchronizationContext>();
        }

        public void NeedSynchronization() => _needSync = true;

        public abstract Task SyncAsync();

        public async Task SyncIfNeededAsync()
        {
            if (_needSync)
            {
                await SyncAsync();

                _needSync = false;
            }
        }
    }
}
