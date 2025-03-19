using Nemeio.Core.Tools.Dispatcher;

namespace Nemeio.Core.Services
{
    public interface IApplicationService
    {
        IDispatcher GetMainThreadDispatcher();
        void StopApplication();
    }
}
