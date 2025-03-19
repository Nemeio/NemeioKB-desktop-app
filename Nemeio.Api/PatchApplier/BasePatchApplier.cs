using Microsoft.Extensions.Logging;
using Nemeio.Core.Transactions;

namespace Nemeio.Api.PatchApplier
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Input class type</typeparam>
    /// <typeparam name="O">Output class type</typeparam>
    public abstract class BasePatchApplier<T, O> where O : IBackupable<O>
    {
        protected readonly ILogger _logger;
        protected readonly Transaction<O> _transaction;

        public BasePatchApplier(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BasePatchApplier<T, O>>();
            _transaction = new Transaction<O>(loggerFactory);
        }

        public abstract O Patch(T input, O currentValue);
    }
}
