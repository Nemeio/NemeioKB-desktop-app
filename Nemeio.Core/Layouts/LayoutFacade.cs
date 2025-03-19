using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nemeio.Core.Images;
using Nemeio.Core.Images.Formats;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Layouts.Import;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts
{
    public class LayoutFacade : ILayoutFacade
    {
        private readonly ILayoutLibrary _library;
        private readonly ILayoutImporter _importer;
        private readonly ILayoutExportService _exporter;
        private readonly ILayoutImageGenerator _genService;
        private readonly ISynchronizer _synchronizer;
        private readonly IAugmentedLayoutImageProvider _augmentedLayoutImageProvider;
        private readonly ILayoutFactory _factory;

        public LayoutFacade(ILayoutLibrary library, ILayoutFactory factory, ISynchronizer synchronizer, ILayoutImporter importer, ILayoutExportService exporter, ILayoutImageGenerator genService, IAugmentedLayoutImageProvider augmentedLayoutImageProvider)
        {
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
            _genService = genService ?? throw new ArgumentNullException(nameof(genService));
            _augmentedLayoutImageProvider = augmentedLayoutImageProvider ?? throw new ArgumentNullException(nameof(augmentedLayoutImageProvider));
        }

        #region CRUD

        public async Task AddLayoutAsync(ILayout layout) => await CrudOperation(layout, _library.AddLayoutAsync);

        public async Task UpdateLayoutAsync(ILayout layout) => await CrudOperation(layout, _library.UpdateLayoutAsync);

        public async Task RemoveLayoutAsync(LayoutId layoutId)
        {
            if (layoutId == null)
            {
                throw new ArgumentNullException(nameof(layoutId));
            }

            var selectedLayout = _library
                .Layouts
                .First(layout => layout.LayoutId == layoutId);

            await RemoveLayoutAsync(selectedLayout);
        }

        public async Task RemoveLayoutAsync(ILayout layout) => await CrudOperation(layout, _library.RemoveLayoutAsync);

        private async Task CrudOperation(ILayout layout, Func<ILayout, Task> crudAction)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            await crudAction(layout);

            await _synchronizer.SynchronizeAsync();
        }

        public async Task<ILayout> DuplicateLayoutAsync(ILayout layout, string title)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var duplicatedLayout = _factory.Duplicate(layout, title);

            await AddLayoutAsync(duplicatedLayout);

            return duplicatedLayout;
        }

        public async Task SetDefaultLayoutAsync(LayoutId id)
        {
            var defaultLayoutId = _library.Layouts.FirstOrDefault(layout => layout.IsDefault && layout.Enable);

            var targetedLayout = _library.Layouts.FirstOrDefault(lyt => lyt.LayoutId.Equals(id));
            if (targetedLayout == null || defaultLayoutId == null || defaultLayoutId.Equals(targetedLayout.LayoutId))
            {
                return;
            }

            await ResetDefaultLayoutAsync();

            if (!targetedLayout.IsDefault)
            {
                targetedLayout.IsDefault = true;

                await UpdateLayoutAsync(targetedLayout);
            }
        }

        public async Task ResetDefaultLayoutAsync()
        {
            foreach (var layout in _library.Layouts)
            {
                if (layout.IsDefault)
                {
                    layout.IsDefault = false;

                    await UpdateLayoutAsync(layout);
                }
            }
        }

        #endregion

        #region Import / Export

        public async Task<ILayout> ImportLayoutAsync(LayoutExport layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var importedLayout = _importer.ImportLayout(layout);

            await AddLayoutAsync(importedLayout);

            return importedLayout;
        }

        public async Task<LayoutExport> ExportLayoutAsync(ILayout layout)
        {
            await Task.Yield();

            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var export = _exporter.Export(layout.LayoutId);

            return export;
        }

        #endregion

        #region Augmented Layouts

        public async Task RefreshAugmentedLayoutAsync()
        {
            var hidLayouts = _library.Layouts
                .Where(layout =>
                        layout.LayoutInfo.Hid &&
                        layout.LayoutInfo.AugmentedHidEnable &&
                        _augmentedLayoutImageProvider.AugmentedLayoutImageExists(layout))
                .ToList();

            foreach (var layout in hidLayouts)
            {
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

                layout.Image = _genService.RenderLayoutImage(imageRequest);

                await _library.UpdateLayoutAsync(layout);
            }

            await _synchronizer.SynchronizeAsync();
        }

        #endregion
    }
}
