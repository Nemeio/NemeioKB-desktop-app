using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Extensions;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Keys.Executors
{
    public class UnicodeKeyExecutor : KeyExecutor
    {
        private readonly ILogger _logger;
        private readonly ISystem _system;

        private IEnumerable<string> _keys;

        public UnicodeKeyExecutor(ILoggerFactory loggerFactory, ISystem system, IEnumerable<string> data)
            : base(string.Empty) 
        {
            _logger = loggerFactory.CreateLogger<UnicodeKeyExecutor>();
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _keys = data ?? throw new ArgumentNullException(nameof(data));
        }

        public override async Task ExecuteAsync()
        {
            await Task.Yield();

            var keys = _keys.ToArray();

            _logger.LogInformation($"SendSequence {keys.ToReadeableString()} on thread : {Thread.CurrentThread.ManagedThreadId}");

            _system.PressKeys(keys);
        }
    }
}
