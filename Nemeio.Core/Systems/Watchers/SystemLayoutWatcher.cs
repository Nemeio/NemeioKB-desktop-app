using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Systems.Watchers
{
    public class SystemLayoutWatcher : ISystemLayoutWatcher
    {
        public static readonly TimeSpan CheckInterval = new TimeSpan(0, 0, 5);

        private readonly ISystemLayoutLoaderAdapter _loaderAdapter;
        private readonly Timer _timer;
        private readonly ILogger _logger;

        private IList<string> _systemLayouts;

        public event EventHandler LayoutChanged;

        public SystemLayoutWatcher(ILoggerFactory loggerFactory, ISystemLayoutLoaderAdapter loaderAdapter)
        {
            _logger = loggerFactory.CreateLogger<SystemLayoutWatcher>();
            _loaderAdapter = loaderAdapter ?? throw new ArgumentNullException(nameof(loaderAdapter));
            _systemLayouts = new List<string>(Load());

            _timer = new Timer();
            _timer.Interval = CheckInterval.TotalMilliseconds;
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        ~SystemLayoutWatcher()
        {
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Stop();
        }

        public IEnumerable<string> Load() => _loaderAdapter.Load().Select(x => x.ToString());

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            await Task.Run(() =>
            {
                try
                {

                    var newLoad = Load();
                    var neworder = string.Join(" ", newLoad.ToList());
                    var previousorder = string.Join(" ", _systemLayouts.ToList());
                    var hasOrderChanged = neworder != previousorder;
                    var hasDeletedSystemLayout = _systemLayouts.Count() > newLoad.Count();
                    var hasAddedSystemLayout = newLoad.Count() > _systemLayouts.Count();

                    if (hasAddedSystemLayout || hasDeletedSystemLayout || hasOrderChanged)
                    {
                        _systemLayouts = newLoad.ToList();

                        LayoutChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Impossible to check system layout changes");
                }
                finally
                {
                    _timer.Start();
                }
            });
        }
    }
}
