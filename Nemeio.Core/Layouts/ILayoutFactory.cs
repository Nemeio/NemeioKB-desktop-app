using System.Collections.Generic;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts
{
    public interface ILayoutFactory
    {
        ILayout CreateHid(OsLayoutId id, IScreen screen, string name);
        IEnumerable<ILayout> CreateHids(IEnumerable<OsLayoutId> ids, IScreen screen);
        ILayout CreateFromExport(LayoutExport export);
        ILayout Duplicate(ILayout fromLayout, string withTitle);
    }
}
