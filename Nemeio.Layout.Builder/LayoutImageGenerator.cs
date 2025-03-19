using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nemeio.Core.Applications;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Resources;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.Layout.Builder.Builders;
using Nemeio.LayoutGen.Models;
using ConfiguratorKey = Nemeio.Core.DataModels.Configurator.Key;

namespace Nemeio.Layout.Builder
{
    public abstract class LayoutImageGenerator : ILayoutImageGenerator
    {
        private readonly object _renderHidLayoutLock = new object();

        private readonly IFontProvider _fontProvider;
        private readonly IAugmentedLayoutImageProvider _augmentedLayoutImageProvider;
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
        protected readonly IResourceLoader _resourceLoader;

        public LayoutImageGenerator(IFontProvider fontProvider, IApplicationSettingsProvider applicationSettingsManager, IAugmentedLayoutImageProvider augmentedLayoutImageProvider, IResourceLoader resourceLoader)
        {
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
            _augmentedLayoutImageProvider = augmentedLayoutImageProvider ?? throw new ArgumentNullException(nameof(augmentedLayoutImageProvider));
            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
            _resourceLoader = resourceLoader ?? throw new ArgumentNullException(nameof(resourceLoader));
        }

        public IEnumerable<ConfiguratorKey> CreateLayoutKeys(IScreen screen, OsLayoutId layoutId)
        {
            lock (_renderHidLayoutLock)
            {
                var nemeioMap = new NemeioMap(screen.Map);

                var keyboardBuilder = CreateKeyboardBuilder();
                var jsonBuilder = new JsonKeysBuilder()
                        .SetDeviceMap(nemeioMap)
                        .SetFontProvider(_fontProvider)
                        .SetOsKeyboardBuilder(CreateKeyboardBuilder())
                        .SetOsLayoutId(layoutId);

                return jsonBuilder.Build().OrderBy(x => x.Index);
            }
        }

        public byte[] RenderLayoutImage(ImageRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            lock (_renderHidLayoutLock)
            {
                var imageType = request.ImageInfo.ImageType;
                var osLayoutId = request.Info.OsLayoutId;

                if (osLayoutId != OsLayoutId.Empty && _applicationSettingsManager.AugmentedImageEnable && request.Info.AugmentedHidEnable && _augmentedLayoutImageProvider.AugmentedLayoutImageExists(osLayoutId, imageType))
                {
                    return _augmentedLayoutImageProvider.GetAugmentedLayoutImage(osLayoutId, imageType);
                }
                else
                {
                    var renderInformations = new LayoutRenderInfo(
                        imageType,
                        request.Keys,
                        request.ImageInfo.Font,
                        request.ImageInfo.Color.IsBlack(),
                        request.Adjustment
                    );

                    var result = request.Screen.Builder.Render(renderInformations, request.Screen.Map);

                    return result;
                }
            }
        }

        public Stream LoadEmbeddedResource(string name) => _resourceLoader.GetResourceStream(name);

        public abstract ISystemKeyboardBuilder CreateKeyboardBuilder();
    }
}
