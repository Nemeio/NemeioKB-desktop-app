using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Import
{
    public interface ILayoutImporter
    {
        ILayout ImportLayout(LayoutExport importLayout);
    }
}
