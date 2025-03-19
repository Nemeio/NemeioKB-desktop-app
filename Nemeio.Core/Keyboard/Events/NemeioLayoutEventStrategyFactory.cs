using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.Sessions.Strategies;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Sessions
{
    public sealed class NemeioLayoutEventStrategyFactory : INemeioLayoutEventStrategyFactory
    {
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public NemeioLayoutEventStrategyFactory(IActiveLayoutChangeHandler activeLayoutChangeHandler)
        {
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
        }

        public INemeioLayoutEventStrategy CreateStrategy(INemeio nemeio, ILoggerFactory loggerFactory)
        {
            INemeioLayoutEventStrategy strategy = null;

            var proxy = KeyboardProxy.CastTo<NemeioLayoutHolderProxy>(nemeio);
            if (proxy != null)
            {
                strategy = new NemeioLayoutEventStrategy(loggerFactory, nemeio, proxy, _activeLayoutChangeHandler);
            }
            else
            {
                strategy = new EmptyKeyboardSessionEventStrategy();
            }

            return strategy;
        }

        private sealed class NemeioLayoutHolderProxy : KeyboardProxy, INemeioLayoutHolderProxy
        {
            private readonly IConfigurationHolder _configurationHolder;
            private readonly IApplyConfigurationHolder _applyConfigurationHolder;
            private readonly ICommunicationModeHolder _communicationModeHolder;

            public IList<LayoutIdWithHash> LayoutIdWithHashs=> _configurationHolder.LayoutIdWithHashs;
            public LayoutId SelectedLayoutId=> _configurationHolder.SelectedLayoutId;
            public IScreen Screen => _configurationHolder.Screen;

            public event EventHandler OnSelectedLayoutChanged;

            public NemeioLayoutHolderProxy(IKeyboard keyboard)
                : base(keyboard)
            {
                _configurationHolder = keyboard as IConfigurationHolder;
                _communicationModeHolder = keyboard as ICommunicationModeHolder;
                _configurationHolder.OnSelectedLayoutChanged += ConfigurationHolder_OnSelectedLayoutChanged;
                _applyConfigurationHolder = keyboard as IApplyConfigurationHolder;
            }

            ~NemeioLayoutHolderProxy()
            {
                _configurationHolder.OnSelectedLayoutChanged -= ConfigurationHolder_OnSelectedLayoutChanged;
            }

            public Task ApplyLayoutAsync(ILayout layout) => _applyConfigurationHolder.ApplyLayoutAsync(layout);
            private void ConfigurationHolder_OnSelectedLayoutChanged(object sender, EventArgs e) => OnSelectedLayoutChanged?.Invoke(sender, e);
            public Task StartSynchronizationAsync() => _configurationHolder.StartSynchronizationAsync();
            public Task EndSynchronizationAsync() => _configurationHolder.EndSynchronizationAsync();
            public Task SetHidModeAsync() => _communicationModeHolder.SetHidModeAsync();
            public Task SetAdvancedModeAsync() => _communicationModeHolder.SetAdvancedModeAsync();
        }
    }
}
