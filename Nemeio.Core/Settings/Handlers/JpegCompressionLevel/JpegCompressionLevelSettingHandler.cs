using System;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Requests.Factories;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Synchronization;

namespace Nemeio.Core.Settings.Handlers.JpegCompressionLevel
{
    public sealed class JpegCompressionLevelSettingHandler : SettingsHandler, IJpegCompressionLevelSettingHandler
    {
        private readonly ILayoutLibrary _layoutLibrary;
        private readonly ISynchronizer _synchronizer;
        private readonly ILayoutImageGenerator _layoutGenService;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;
        private readonly IKeyboardController _keyboardController;

        public JpegCompressionLevelSettingHandler(ISettingsHolder settingsHolder, ILayoutLibrary layoutLibrary, ISynchronizer synchronizer, ILayoutImageGenerator layoutGenService, IActiveLayoutChangeHandler activeLayoutChangeHandler, IKeyboardController keyboardController)
            : base(settingsHolder)
        {
            _layoutLibrary = layoutLibrary ?? throw new ArgumentNullException(nameof(layoutLibrary));
            _synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            _layoutGenService = layoutGenService ?? throw new ArgumentNullException(nameof(layoutGenService));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
        }

        public override async Task OnSettingsUpdatedAsync(ISettings settings)
        {
            if (_layoutLibrary.Layouts.Count == 0)
            {
                return;
            }

            var selectedLayoutIdBeforeSync = _activeLayoutChangeHandler.Synchronizer.LastSynchronizedLayout?.LayoutId ?? _layoutLibrary.Layouts.First().LayoutId;

            for (var i = 0; i < _layoutLibrary.Layouts.Count; i++)
            {
                var layout = _layoutLibrary.Layouts[i];
                var imageRequest = new ImageRequest(
                    info: layout.LayoutInfo,
                    imageInfo: layout.LayoutImageInfo,
                    keys: layout.Keys,
                    screen: layout.LayoutImageInfo.Screen,
                    adjustment: new ImageAdjustment(
                        layout.LayoutImageInfo.XPositionAdjustment,
                        layout.LayoutImageInfo.YPositionAdjustement
                    )
                );

                layout.Image = _layoutGenService.RenderLayoutImage(imageRequest);

                await _layoutLibrary.UpdateLayoutAsync(layout);
            }

            await _synchronizer.SynchronizeAsync();
            await _activeLayoutChangeHandler.RequestMenuChangeAsync(_keyboardController.Nemeio, selectedLayoutIdBeforeSync);
        }
    }
}
