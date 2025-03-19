using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Systems
{
    public interface ISystemLayoutLoaderAdapter
    {
        IEnumerable<OsLayoutId> Load();
    }
}
