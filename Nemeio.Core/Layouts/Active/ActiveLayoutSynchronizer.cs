using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Layouts.Active.Exceptions;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active
{
    public class ActiveLayoutSynchronizer : IActiveLayoutSynchronizer
    {
        private readonly ILogger _logger;
        private readonly IActiveLayoutHistoric _historic;
        private readonly SemaphoreSlim _lockSync = new SemaphoreSlim(1, 1);

        public event EventHandler ActiveLayoutChanged;

        public bool Syncing { get; private set; } = false;
        public ILayout LastSynchronizedLayout { get; private set; } = null;

        public ActiveLayoutSynchronizer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ActiveLayoutSynchronizer>();
            _historic = new ActiveLayoutHistoric();
        }

        public async Task PostRequestAsync(IChangeRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _lockSync.WaitAsync();

            Syncing = true;

            try
            {
                await SyncAsync(request);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Synchronize active layout failed");

                throw new ActiveLayoutSynchronizationFailed($"Sync active layout failed", exception);
            }
            finally
            {
                Syncing = false;

                _lockSync.Release();
            }
        }

        public void ResetHistoric() => _historic.Reset();

        private async Task SyncAsync(IChangeRequest request)
        {
            _logger.LogInformation($"Start sync active layout with request <{request.GetType().Name}>");

            var synchronizedLayout = await request.ApplyLayoutAsync(_historic, LastSynchronizedLayout);
            if (synchronizedLayout != null)
            {
                LastSynchronizedLayout = synchronizedLayout;

                _logger.LogInformation($"Layout <{LastSynchronizedLayout.Title}> has been set");
                _logger.LogInformation($"Layout historic: {_historic}");

                ActiveLayoutChanged?.Invoke(this, EventArgs.Empty);
            }

            _logger.LogInformation($"End sync active layout");
        }
    }
}
