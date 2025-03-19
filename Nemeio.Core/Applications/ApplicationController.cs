using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Sessions;

namespace Nemeio.Core.Applications
{
    public class ApplicationController : IApplicationController
    {
        /// <summary>
        /// Major number = Hardware version
        /// Minor number = Protocol version
        /// </summary>
        public const string CommunicationProtocolVersion = "2.3";

        private readonly ILogger _logger;
        private readonly ISystem _system;
        private readonly ILanguageManager _languageManager;
        private readonly IPackageUpdater _packageUpdater;
        private readonly IKeyboardController _keyboardController;
        private readonly INemeioHttpService _httpService;
        private readonly ISettingsHolder _settingsHolder;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public bool Started { get; private set; }

        public ApplicationController(ILoggerFactory loggerFactory, ISystem system, ILanguageManager languageManager, IPackageUpdater packageUpdater, IKeyboardController keyboardController, ISettingsHolder settingsHolder, INemeioHttpService httpService, IActiveLayoutChangeHandler activeLayoutChangeHandler)
        {
            _logger = loggerFactory.CreateLogger<ApplicationController>();
            _logger.LogInformation("ApplicationController.Constructor()");

            _system = system ?? throw new ArgumentNullException(nameof(system));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(keyboardController));
            _packageUpdater = packageUpdater ?? throw new ArgumentNullException(nameof(keyboardController));
            _settingsHolder = settingsHolder ?? throw new ArgumentNullException(nameof(settingsHolder));
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
        }

        public void Start()
        {
            _system.OnSessionStateChanged += System_OnSessionStateChanged;

            Task.Run(() => _languageManager.Start())
                .ContinueWith((task) => StartFeatures());

            _keyboardController.RunAsync();
            _packageUpdater.CheckUpdatesAsync();

            Started = true;
        }

        private void StartFeatures()
        {
            //  We don't want to wait there tasks

            var autoStartWebServerEnabled = _settingsHolder.Settings?.AutoStartWebServerSetting.Value ?? false;
            if (autoStartWebServerEnabled)
            {
                _httpService.StartListenToRequests();
            }


            
        }

        public void ShutDown()
        {
            if (Started)
            {
                _logger.LogInformation($"Application exit");

                if (_languageManager != null)
                {
                    _languageManager.Stop();
                }

                if (_keyboardController != null && _keyboardController.Nemeio != null)
                {
                    _keyboardController.Nemeio.Stop();
                }

                if (_system != null)
                {
                    _system.OnSessionStateChanged -= System_OnSessionStateChanged;
                    _system.Stop();
                }

                if (_httpService != null)
                {
                    _httpService.StopListeningToRequestsAsync();
                }
                    
                if (_activeLayoutChangeHandler != null)
                {
                    _activeLayoutChangeHandler.StopAsync();
                }
            }
            else
            {
                _logger.LogWarning($"Try to shutdown application but already stopped");
            }
        }

        #region Events

        private async void System_OnSessionStateChanged(object sender, EventArgs e)
        {
            if (_packageUpdater == null)
            {
                return;
            }

            if (_system.SessionState == SessionState.Open && _packageUpdater.State != PackageUpdateState.UpdatePending)
            {
                await _packageUpdater.CheckUpdatesAsync();
            }
        }

        #endregion
    }
}
