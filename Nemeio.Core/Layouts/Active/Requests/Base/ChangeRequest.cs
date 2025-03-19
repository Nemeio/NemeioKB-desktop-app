using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests.Base
{
    public abstract class ChangeRequest : IChangeRequest
    {
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly ILogger _logger;
        protected readonly ISystem _system;
        protected readonly INemeio _nemeio;
        protected readonly ILayoutLibrary _library;
        private TaskCompletionSource<string> _completionSource;

        public ChangeRequest(ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio)
        {

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _completionSource = new TaskCompletionSource<string>();
            _nemeio = nemeio; // Nemeio can be null
            _logger = _loggerFactory.CreateLogger<ChangeRequest>();
        }

        public async Task<ILayout> ApplyLayoutAsync(IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            ILayout applyResult = null;
            ILayoutHolderNemeioProxy proxy = null;

            if (_nemeio != null)
            {
                proxy = KeyboardProxy.CastTo<LayoutHolderNemeioProxy>(_nemeio);
            }

            try
            {
                applyResult = await ApplyAsync(proxy, historic, lastSynchronized);

                _completionSource.TrySetResult(string.Empty);
            }
            catch (Exception exception)
            {
                _completionSource.TrySetException(exception);
            }

            return applyResult;
        }

        public abstract Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized);

        public async Task ExecuteAsync() => await _completionSource.Task;

        public async Task ApplyKeyboardLayout(ILayoutHolderNemeioProxy proxy, ILayout layout)
        {
            try
            {
                await proxy?.ApplyLayoutAsync(layout);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "ApplyKeyboardLayout Task");

            }
        }

        public async Task ApplySystemLayout(OsLayoutId id) => await _system.EnforceSystemLayoutAsync(id);

        public Task ApplySystemLayout(ILayout layout)
        {
            var osLayoutId = GetAttachedSystemLayoutId(layout);

            return ApplySystemLayout(osLayoutId);
        }

        protected OsLayoutId GetAttachedSystemLayoutId(ILayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var osLayoutId = layout.LayoutInfo.Hid
                ? layout.LayoutInfo.OsLayoutId
                : _library.Layouts.FirstOrDefault(x => x.LayoutId.ToString() == layout.AssociatedLayoutId.ToString())?.LayoutInfo.OsLayoutId;

            return osLayoutId;
        }

        private sealed class LayoutHolderNemeioProxy : KeyboardProxy, ILayoutHolderNemeioProxy
        {
            private readonly IConfigurationHolder _configurationHolder;
            private readonly IApplyConfigurationHolder _applyConfigurationHolder;

            public IList<LayoutIdWithHash> LayoutIdWithHashs => _configurationHolder.LayoutIdWithHashs;
            public LayoutId SelectedLayoutId => _configurationHolder.SelectedLayoutId;
            public IScreen Screen => _configurationHolder.Screen;

            public event EventHandler OnSelectedLayoutChanged;

            public LayoutHolderNemeioProxy(IKeyboard keyboard)
                : base(keyboard)
            {
                _configurationHolder = keyboard as IConfigurationHolder;
                _configurationHolder.OnSelectedLayoutChanged += ConfigurationHolder_OnSelectedLayoutChanged;
                _applyConfigurationHolder = keyboard as IApplyConfigurationHolder;
            }

            ~LayoutHolderNemeioProxy()
            {
                _configurationHolder.OnSelectedLayoutChanged -= ConfigurationHolder_OnSelectedLayoutChanged;
            }

            public Task ApplyLayoutAsync(ILayout layout) => _applyConfigurationHolder.ApplyLayoutAsync(layout);
            private void ConfigurationHolder_OnSelectedLayoutChanged(object sender, EventArgs e) => OnSelectedLayoutChanged?.Invoke(sender, e);
            public Task StartSynchronizationAsync() => _configurationHolder.StartSynchronizationAsync();
            public Task EndSynchronizationAsync() => _configurationHolder.EndSynchronizationAsync();
        }
    }
}
