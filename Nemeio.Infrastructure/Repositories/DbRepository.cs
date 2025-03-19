using Microsoft.Extensions.Logging;

namespace Nemeio.Infrastructure
{
    public abstract class DbRepository
    {
        protected readonly ILogger _logger;

        protected readonly IDatabaseAccessFactory _databaseAccessFactory;

        public DbRepository(ILogger logger, IDatabaseAccessFactory dbAccessFactory)
        {
            _logger = logger;
            _databaseAccessFactory = dbAccessFactory;
        }
    }
}
