using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Name
{
    public class LayoutNameTransformer : ILayoutNameTransformer
    {
        private ILayoutNativeNameAdapter _nativeNameAdapter;

        public LayoutNameTransformer(ILayoutNativeNameAdapter nativeNameAdapter)
        {
            _nativeNameAdapter = nativeNameAdapter ?? throw new ArgumentNullException(nameof(nativeNameAdapter));
        }

        public string TransformNameIfNeeded(IEnumerable<ILayout> contextLayouts, OsLayoutId layoutId)
        {
            var nativeName = _nativeNameAdapter.RetrieveNativeName(layoutId);

            return TransformNameIfNeeded(contextLayouts, nativeName);
        }

        public string TransformNameIfNeeded(IEnumerable<ILayout> contextLayouts, string currentName)
        {
            if (!LayoutTitleExists(contextLayouts, currentName))
            {
                return currentName;
            }

            return IncrementTitle(contextLayouts, currentName);
        }

        private string IncrementTitle(IEnumerable<ILayout> contextLayouts, string title)
        {
            var index = 1;
            var newTitle = string.Empty;

            do
            {
                newTitle = $"{title} {index}";
                index += 1;

            } while (LayoutTitleExists(contextLayouts, newTitle));

            return newTitle;
        }

        public bool LayoutTitleExists(IEnumerable<ILayout> contextLayouts, string title) => contextLayouts.Any(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
}
