using System;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Transactions
{
    public class Transaction<T> where T : IBackupable<T>
    {
        private readonly ILogger _logger;
        private T _backup;

        public Transaction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Transaction<T>>();
        }

        public void Run(ref T original, Action transaction)
        {
            _backup = original.CreateBackup();

            try
            {
                transaction();
            }
            //  In this case we want to catch every error
            //  If there an error so transaction failed
            catch (Exception exception)
            {
                _logger.LogError(exception, "Transaction failed");

                original = _backup;

                throw;
            }
        }
    }
}
