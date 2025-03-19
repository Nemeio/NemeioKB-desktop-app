using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Name
{
    public interface ILayoutNativeNameAdapter
    {
        string RetrieveNativeName(OsLayoutId osLayoutId);
    }
}
