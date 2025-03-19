using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools;

namespace Nemeio.Core.Systems.Hid
{
    public interface ISystemHidInteractor : IStoppable
    {
        void Run();
        void PostHidStringKeys(string[] keys);
        void SystemLayoutChanged(OsLayoutId id);
        void StopSendKey();
    }
}
