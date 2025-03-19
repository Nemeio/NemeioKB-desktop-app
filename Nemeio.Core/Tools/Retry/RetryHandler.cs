using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nemeio.Core.Tools.Retry
{
    public class RetryHandler : IRetryHandler
    {
        private ILogger _logger;

        public RetryHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RetryHandler>();
        }

        public void Execute(SyncRetryAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _logger.LogInformation($"Start retry <{action.Name}>");

            for (int i = 0; i <= action.RetryCount; ++i)
            {
                try
                {
                    if (i != 0)
                    {
                        _logger.LogInformation($"<Retry <{i}/{action.RetryCount}>");
                    }

                    action.Execute();

                    _logger.LogInformation($"End retry");

                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"<Retry <{i}/{action.RetryCount}> failed");

                    if (i == action.RetryCount)
                    {
                        throw new RetryFailedException(exception);
                    }
                }
            }
        }

        public async Task ExecuteAsync(AsyncRetryAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _logger.LogInformation($"Start retry <{action.Name}>");

            for (int i = 0; i <= action.RetryCount; ++i)
            {
                try
                {
                    if (i != 0)
                    {
                        _logger.LogInformation($"<Retry <{i}/{action.RetryCount}>");
                    }

                    await action.ExecuteAsync();

                    _logger.LogInformation($"End retry");

                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"<Retry <{i}/{action.RetryCount}> failed");

                    if (i == action.RetryCount)
                    {
                        throw new RetryFailedException(exception);
                    }
                }
            }
        }
    }
}
