using System.Threading.Tasks;

namespace Nemeio.Core.Layouts.Synchronization.Contexts
{
    public interface ISynchronizationContext
    {
        Task SyncAsync();
        Task SyncIfNeededAsync();
        void NeedSynchronization();
    }
}
