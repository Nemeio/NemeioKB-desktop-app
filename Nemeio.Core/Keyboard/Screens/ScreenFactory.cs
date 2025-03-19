using System;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.Images.Formats;
using Nemeio.Core.Images.Renderer;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Settings;

namespace Nemeio.Core.Keyboard.Screens
{
    public sealed class ScreenFactory : IScreenFactory
    {
        private readonly IKeyboardMapFactory _mapFactory;
        private readonly IJpegRenderer _jpegRenderer;
        private readonly IOneBppRenderer _oneRenderer;
        private readonly ISettingsHolder _settingsHolder;

        public ScreenFactory(IKeyboardMapFactory mapFactory, IJpegRenderer jpegRenderer, IOneBppRenderer oneRenderer, ISettingsHolder settingsHolder)
        {
            _mapFactory = mapFactory ?? throw new ArgumentNullException(nameof(mapFactory));
            _jpegRenderer = jpegRenderer ?? throw new ArgumentNullException(nameof(jpegRenderer));
            _oneRenderer = oneRenderer ?? throw new ArgumentNullException(nameof(oneRenderer));
            _settingsHolder = settingsHolder ?? throw new ArgumentNullException(nameof(settingsHolder));
        }

        public IScreen CreateEinkScreen()
        {
            var map = _mapFactory.CreateEinkMap();
            var imageFormat = new JpegFormat(_settingsHolder.Settings?.JpegCompressionLevelSetting);
            var jpegImageBuilder = new JpegImageBuilder(imageFormat, _jpegRenderer);

            var screen = new EinkScreen(map, jpegImageBuilder);

            return screen;
        }

        public IScreen CreateHolitechScreen()
        {
            var map = _mapFactory.CreateHolitechMap();
            var imageFormat = new OneBppFormat();
            var oneBppImageBuilder = new OneBppImageBuilder(imageFormat, _oneRenderer);

            var screen = new HolitechScreen(map, oneBppImageBuilder);

            return screen;
        }

        public IScreen CreateScreen(ScreenType type)
        {
            switch (type)
            {
                case ScreenType.Holitech:
                    return CreateHolitechScreen();
                case ScreenType.Eink:
                    return CreateEinkScreen();
                default:
                    throw new InvalidOperationException($"Type <{type}> is not supported");
            }
        }
    }
}
