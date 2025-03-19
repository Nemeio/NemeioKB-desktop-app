using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Systems.Hid
{
    public interface ISystemHidAdapter
    {
        void Init();
        void LoadKeys();
        void SystemLayoutChanged(OsLayoutId layoutId);
        void ExecuteKeys(IList<SystemHidKey> keys);
        void ReleaseKeys();
    }
}
