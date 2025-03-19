using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Import
{
    public class LayoutImporter : ILayoutImporter
    {
        private readonly ILayoutLibrary _layoutLibrary;
        private readonly ILayoutFactory _layoutFactory;

        public LayoutImporter(ILayoutLibrary layoutLibrary, ILayoutFactory layoutFactory)
        {
            _layoutLibrary = layoutLibrary ?? throw new ArgumentNullException(nameof(layoutLibrary));
            _layoutFactory = layoutFactory ?? throw new ArgumentNullException(nameof(layoutFactory));
        }

        public ILayout ImportLayout(LayoutExport importLayout)
        {
            if (importLayout == null)
            {
                throw new ArgumentNullException(nameof(importLayout));
            }

            if (string.IsNullOrWhiteSpace(importLayout.Title))
            {
                throw new CoreException(ErrorCode.CoreImportLayoutTitleEmpty);
            }

            //  Check if title is already used!
            if (LayoutTitleExists(importLayout.Title))
            {
                throw new CoreException(ErrorCode.CoreImportLayoutTitleAlreadyUsed);
            }

            LayoutId associatedLayoutId;
            try
            {
                associatedLayoutId = new LayoutId(importLayout.AssociatedLayoutId);
            }
            catch (Exception)
            {
                throw new CoreException(ErrorCode.CoreImportLayoutInvalidAssociatedLayoutId);
            }

            ILayout associatedLayout;

            try
            {
                associatedLayout = _layoutLibrary
                    .Layouts
                    .First(lyt => lyt.LayoutId == associatedLayoutId)
                    .CreateBackup();
            }
            catch (Exception)
            {
                throw new CoreException(ErrorCode.CoreImportLayoutMissingAssociatedLayout);
            }

            // Remap edited keys.
            var remappedKeys = RemapImportedKeys(associatedLayout.Keys, importLayout.Keys);
            importLayout.Keys = remappedKeys.ToList();

            var layout = _layoutFactory.CreateFromExport(importLayout);

            return layout;
        }

        private IEnumerable<Key> RemapImportedKeys(IReadOnlyCollection<Key> associatedKeys, IReadOnlyCollection<Key> importedKeys)
        {
            foreach (var key in associatedKeys)
            {
                var editedKey = importedKeys.SingleOrDefault(k => k.Index == key.Index && k.Edited);
                if (editedKey != null)
                {
                    yield return editedKey;
                }
                else
                {
                    yield return key;
                }
            }
        }

        private bool LayoutTitleExists(string title) => _layoutLibrary.Layouts.Any(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
}
