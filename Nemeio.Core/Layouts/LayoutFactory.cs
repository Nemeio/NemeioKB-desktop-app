using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;


namespace Nemeio.Core.Layouts
{
    public class LayoutFactory : ILayoutFactory
    {
        private readonly ILayoutLibrary _library;
        private readonly ILayoutImageGenerator _genService;
        private readonly ILayoutNameTransformer _layoutNameTransformer;
        private readonly IScreenFactory _screenFactory;

        public LayoutFactory(ILayoutLibrary library, ILayoutImageGenerator genService, ILayoutNameTransformer layoutNameTransformer, IScreenFactory screenFactory)
        {
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _genService = genService ?? throw new ArgumentNullException(nameof(genService));
            _layoutNameTransformer = layoutNameTransformer ?? throw new ArgumentNullException(nameof(layoutNameTransformer));
            _screenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
        }

        public ILayout CreateHid(OsLayoutId id, IScreen screen, string name)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var layoutsKeys = _genService.CreateLayoutKeys(screen, id);
            layoutsKeys.ForEach(x => x.Font = null);

            var layoutInfo = new LayoutInfo(
                id,
                isFactory: false,
                isHid: true,
                linkPath: new List<string>(),
                linkEnable: false,
                isTemplate: true,
                augmentedHidEnable: true
            );

            var layoutDefaultImageType = LayoutImageType.Classic;
            var layoutDefaultFont = FontProvider.GetDefaultFont();

            var layoutImageInfo = new LayoutImageInfo(
                HexColor.Black,
                layoutDefaultFont,
                screen,
                layoutDefaultImageType
            );

            var imageRequest = new ImageRequest(
                info: layoutInfo,
                imageInfo: layoutImageInfo,
                keys: layoutsKeys,
                screen: screen,
                adjustment: new ImageAdjustment(
                    layoutImageInfo.XPositionAdjustment,
                    layoutImageInfo.YPositionAdjustement
                )
            );

            var layoutImage = _genService.RenderLayoutImage(imageRequest);

            string[] splitTitle;
            if (System.AppContext.TryGetSwitch("ISMACOS", out bool ISMACOS) && ISMACOS)
            {
                splitTitle = new string[2] { name, name };
            }
            else
            {
                splitTitle = name.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            var layout = new Layout(
                layoutInfo,
                layoutImageInfo,
                layoutImage,
                NemeioConstants.DefaultCategoryId,
                0,  //  Not needed anymore
                splitTitle.Length > 0 ? splitTitle[0] : String.Empty,
                splitTitle.Length > 1 ? splitTitle[1] : String.Empty,
                DateTime.Now,
                DateTime.Now,
                layoutsKeys.ToList()
            );           

            return layout;
        }

        public IEnumerable<ILayout> CreateHids(IEnumerable<OsLayoutId> ids, IScreen screen)
        {
            var layouts = new List<ILayout>();

            foreach (var id in ids)
            {
                var title = _layoutNameTransformer.TransformNameIfNeeded(layouts, id);
                var layout = CreateHid(id, screen, title);

                layouts.Add(layout);
            }

            return layouts;
        }

        public ILayout CreateFromExport(LayoutExport export)
        {
            if (export == null)
            {
                throw new ArgumentNullException(nameof(export));
            }

            //  All imported or exported layout are custom.
            //  Augmented layout are compatible only with Hid layouts.
            const bool augmentedHidEnabled = false;

            var screen = _screenFactory.CreateScreen(export.Screen);
            var layoutInfo = new LayoutInfo(
                new OsLayoutId(string.Empty),
                isFactory: false,
                isHid: false,
                linkPath: export.LinkApplicationPaths,
                linkEnable: export.LinkApplicationEnable,
                isTemplate: false,
                augmentedHidEnable: augmentedHidEnabled
            );

            var layoutImageInfo = new LayoutImageInfo(
                export.IsDarkMode ? HexColor.Black : HexColor.White,
                export.Font,
                screen,
                export.ImageType
            );

            //  WRN [KSB] We can import layout all the time (with or without keyboard plugged)
            //  So for the moment, we force Holitech screen with one bpp
            var imageRequest = new ImageRequest(
                info: layoutInfo,
                imageInfo: layoutImageInfo,
                keys: export.Keys,
                screen: screen,
                adjustment: null
            );

            var layoutImage = _genService.RenderLayoutImage(imageRequest);

            var layout = new Layout(
                layoutInfo,
                layoutImageInfo,
                layoutImage,
                NemeioConstants.DefaultCategoryId,
                0,
                export.Title,
                export.Subtitle,
                DateTime.Now,
                DateTime.Now,
                export.Keys.ToList(),
                LayoutId.NewLayoutId,
                new LayoutId(export.AssociatedLayoutId),
                false,
                false
            );

            return layout;
        }

        public ILayout Duplicate(ILayout fromLayout, string withTitle)
        {
            if (fromLayout == null)
            {
                throw new ArgumentNullException(nameof(fromLayout));
            }

            if (fromLayout.LayoutInfo.Hid)
            {
                throw new ForbiddenActionException("Cannot duplicate an HID layout");
            }

            var newLayoutTitle = string.Empty;
            var newLayout = fromLayout.CreateBackup();

            if (!string.IsNullOrWhiteSpace(withTitle))
            {
                //  Check if title is already used!
                if (_layoutNameTransformer.LayoutTitleExists(_library.Layouts, withTitle))
                {
                    throw new InvalidDataException($"Title <{withTitle}> is already used!");
                }

                newLayoutTitle = withTitle;
            }
            else
            {
                newLayoutTitle = _layoutNameTransformer.TransformNameIfNeeded(_library.Layouts, newLayout.Title);
            }

            newLayout.LayoutId = LayoutId.NewLayoutId;
            newLayout.Enable = false;
            newLayout.Title = newLayoutTitle;
            newLayout.CalculateImageHash();

            return newLayout;
        }
    }
}
