using System.Threading.Tasks;

namespace Nemeio.Core.Tools.Retry
{
    public interface IRetryHandler
    {
        void Execute(SyncRetryAction action);
        Task ExecuteAsync(AsyncRetryAction action);
    }
}
