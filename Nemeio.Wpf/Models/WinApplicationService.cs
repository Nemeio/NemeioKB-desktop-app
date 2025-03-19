using System.Windows;
using Nemeio.Core.Services;
using Nemeio.Core.Tools.Dispatcher;

namespace Nemeio.Wpf.Models
{
    public class WinApplicationService : IApplicationService
    {
        private readonly IDispatcher _mainDispatcher;

        public WinApplicationService()
        {
            _mainDispatcher = new WinMainThreadDispatcher();
        }

        public IDispatcher GetMainThreadDispatcher() => _mainDispatcher;

        public void StopApplication()
        {
            _mainDispatcher.Invoke(() => Application.Current.Shutdown());
        }
    }
}
