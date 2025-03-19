using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.DataModels;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Presentation.Menu.Battery;
using Nemeio.Presentation.Menu.Configurator;
using Nemeio.Presentation.Menu.Connection;
using Nemeio.Presentation.Menu.Layouts;
using Nemeio.Presentation.Menu.Quit;
using Nemeio.Presentation.Menu.Synchronization;
using Nemeio.Presentation.Menu.Version;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Presentation.Menu
{
    public sealed class Menu
    {
        private sealed class SerialNumberHolderNemeioProxy : KeyboardProxy, ISerialNumberHolder
        {
            private readonly ISerialNumberHolder _serialNumberHolder;

            public NemeioSerialNumber SerialNumber => _serialNumberHolder.SerialNumber;

            public SerialNumberHolderNemeioProxy(Core.Keyboard.Nemeio nemeio)
                : base(nemeio)
            {
                _serialNumberHolder = nemeio as ISerialNumberHolder;
            }
        }

        private sealed class VersionableNemeioProxy : KeyboardProxy, IVersionHolder
        {
            private readonly IVersionHolder _versionHolder;

            public FirmwareVersions Versions => _versionHolder.Versions;

            public VersionableNemeioProxy(Core.Keyboard.Nemeio nemeio)
                : base(nemeio)
            {
                _versionHolder = nemeio as IVersionHolder;
            }
        }

        private class LayoutHolderNemeioProxy : KeyboardProxy, IConfigurationHolder
        {
            private readonly IConfigurationHolder _configurationHolder;

            public IList<LayoutIdWithHash> LayoutIdWithHashs => _configurationHolder.LayoutIdWithHashs;
            public LayoutId SelectedLayoutId => _configurationHolder.SelectedLayoutId;
            public IScreen Screen => _configurationHolder.Screen;

            public event EventHandler OnSelectedLayoutChanged;

            public LayoutHolderNemeioProxy(Core.Keyboard.Nemeio nemeio)
                : base(nemeio)
            {
                _configurationHolder = nemeio as IConfigurationHolder;
                _configurationHolder.OnSelectedLayoutChanged += ConfigurationHolder_OnSelectedLayoutChanged;
            }

            public Task StartSynchronizationAsync() => _configurationHolder.StartSynchronizationAsync();
            public Task EndSynchronizationAsync() => _configurationHolder.EndSynchronizationAsync();

            private void ConfigurationHolder_OnSelectedLayoutChanged(object sender, EventArgs e) => OnSelectedLayoutChanged?.Invoke(sender, e);
        }

        private readonly BatterySectionBuilder _batterySectionBuilder;
        private readonly ConnectionSectionBuilder _connectionSectionBuilder;
        private readonly VersionSectionBuilder _versionSectionBuilder;
        private readonly LayoutsSectionBuilder _layoutsSectionBuilder;
        private readonly SynchronizationSectionBuilder _synchronizationSectionBuilder;

        private readonly ILanguageManager _languageManager;
        private readonly IPackageUpdaterMessageFactory _messageFactory;

        public QuitSection Quit { get; private set; }
        public BatterySection Battery { get; private set; }
        public ConnectionSection Connection { get; private set; }
        public LayoutsSection Layouts { get; private set; }
        public VersionSection Versions { get; private set; }
        public SynchronizationSection Synchronization { get; private set; }
        public ConfiguratorSection Configurator { get; private set; }

        private VersionInformation VersionInformation { get; set; } = new VersionInformation();

        public Menu(ILanguageManager languageManager, IPackageUpdaterMessageFactory messageFactory)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));

            _batterySectionBuilder = new BatterySectionBuilder(_languageManager);
            _connectionSectionBuilder = new ConnectionSectionBuilder(_languageManager);
            _versionSectionBuilder = new VersionSectionBuilder(_languageManager, _messageFactory);
            _layoutsSectionBuilder = new LayoutsSectionBuilder(_languageManager);
            _synchronizationSectionBuilder = new SynchronizationSectionBuilder(_languageManager);
        }

        public Menu Build(IApplicationMenu applicationMenu)
        {
            if (applicationMenu == null)
            {
                throw new ArgumentNullException(nameof(applicationMenu));
            }

            RefreshSynchronization(applicationMenu);
            RefreshConnection(applicationMenu);
            RefreshBattery(applicationMenu);
            RefreshVersion(applicationMenu);
            RefreshConfigurator();
            RefreshQuit();
            RefreshLayouts(applicationMenu);

            return this;
        }

        private void RefreshSynchronization(IApplicationMenu applicationMenu)
        {
            if (!applicationMenu.Syncing.Value)
            {
                Synchronization = _synchronizationSectionBuilder.Build(null);
            }
            else
            {
                Synchronization = _synchronizationSectionBuilder.Build(applicationMenu.SyncProgression.Value);
            }
        }

        private void RefreshConnection(IApplicationMenu applicationMenu)
        {
            if (applicationMenu.Keyboard.Value == null)
            {
                VersionInformation.Stm32Version = null;
                VersionInformation.BluetoothLEVersion = null;
                VersionInformation.IteVersion = null;
                VersionInformation.KeyboardIsPlugged = false;

                Connection = null;
            }
            else
            {
                var versionableProxy = KeyboardProxy.CastTo<VersionableNemeioProxy>(applicationMenu.Keyboard.Value);
                if (versionableProxy != null && versionableProxy.Versions != null)
                {
                    VersionInformation.Stm32Version = versionableProxy.Versions.Stm32;
                    VersionInformation.BluetoothLEVersion = versionableProxy.Versions.Nrf;
                    VersionInformation.IteVersion = versionableProxy.Versions.Ite.ToString();
                    VersionInformation.KeyboardIsPlugged = true;
                }

                var serialNumberHolderProxy = KeyboardProxy.CastTo<SerialNumberHolderNemeioProxy>(applicationMenu.Keyboard.Value);
                if (serialNumberHolderProxy != null && serialNumberHolderProxy.SerialNumber != null)
                {
                    Connection = _connectionSectionBuilder.Build(applicationMenu.Keyboard.Value);
                }
            }
        }

        private void RefreshBattery(IApplicationMenu applicationMenu)
        {
            Battery = _batterySectionBuilder.Build(applicationMenu.BatteryInformations.Value);
        }

        private void RefreshLayouts(IApplicationMenu applicationMenu)
        {
            var keyboardIsReady = false;

            if (applicationMenu.Keyboard.Value != null)
            {
                var proxy = KeyboardProxy.CastTo<LayoutHolderNemeioProxy>(applicationMenu.Keyboard.Value);
                if (proxy != null)
                {
                    keyboardIsReady = proxy.State == NemeioState.Ready;
                }
            }

            var layoutSection = _layoutsSectionBuilder.Build(
                applicationMenu.Layouts.Value,
                applicationMenu.SelectedLayout.Value,
                applicationMenu.Keyboard.Value != null,
                keyboardIsReady,
                applicationMenu.Syncing.Value,
                applicationMenu.ApplicationAugmentedHidEnable.Value
            );

            Layouts = layoutSection;
        }

        private void RefreshQuit()
        {
            var quitTitle = _languageManager.GetLocalizedValue(StringId.CommonButtonQuit);

            Quit = new QuitSection(quitTitle);
        }

        private void RefreshConfigurator()
        {
            var configuratorTitle = _languageManager.GetLocalizedValue(StringId.ConfiguratorButtonOpen);

            Configurator = new ConfiguratorSection(configuratorTitle);
        }

        private void RefreshVersion(IApplicationMenu applicationMenu)
        {
            if (applicationMenu.ApplicationVersion.Value != null)
            {
                VersionInformation.ApplicationVersion = new VersionProxy(applicationMenu.ApplicationVersion.Value);
            }

            Versions = _versionSectionBuilder.Build(VersionInformation);
        }
    }
}
