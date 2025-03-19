using System;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Systems
{
    public interface ISystemActiveLayoutAdapter
    {
        event EventHandler OnSystemActionLayoutChanged;
        OsLayoutId GetCurrentSystemLayoutId();
        OsLayoutId GetDefaultSystemLayoutId();
    }
}
