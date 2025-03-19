using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Name
{
    public interface ILayoutNameTransformer
    {
        string TransformNameIfNeeded(IEnumerable<ILayout> contextLayouts, OsLayoutId layoutId);
        string TransformNameIfNeeded(IEnumerable<ILayout> contextLayouts, string currentName);
        bool LayoutTitleExists(IEnumerable<ILayout> contextLayouts, string title);
    }
}
