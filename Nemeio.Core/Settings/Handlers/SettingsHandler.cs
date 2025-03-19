using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nemeio.Core.Settings.Handlers
{
    public abstract class SettingsHandler
    {
        private readonly SemaphoreSlim _settingsEventSemaphore;
        protected readonly ISettingsHolder _settingsHolder;

        protected SettingsHandler(ISettingsHolder settingsHolder)
        {
            _settingsEventSemaphore = new SemaphoreSlim(1, 1);
            _settingsHolder = settingsHolder ?? throw new ArgumentNullException(nameof(settingsHolder));
            _settingsHolder.OnSettingsUpdated += SettingsHolder_OnSettingsUpdated;
        }

        public abstract Task OnSettingsUpdatedAsync(ISettings settings);

        private async void SettingsHolder_OnSettingsUpdated(object sender, EventArgs e)
        {
            await _settingsEventSemaphore.WaitAsync();

            try
            {
                await OnSettingsUpdatedAsync(_settingsHolder.Settings);
            }
            finally
            {
                _settingsEventSemaphore.Release();
            }
        }
    }
}
