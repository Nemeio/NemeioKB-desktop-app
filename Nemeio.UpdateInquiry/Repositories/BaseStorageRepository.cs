using Microsoft.Extensions.Configuration;

namespace Nemeio.UpdateInquiry.Repositories
{
    public abstract class BaseStorageRepository
    {
        private const string AzureWebJobsStorageConnectionString = "AzureWebJobsStorage";

        protected string _connectionString;

        public BaseStorageRepository(IConfigurationRoot configurationRoot)
        {
            _connectionString = configurationRoot[AzureWebJobsStorageConnectionString];
        }
    }
}
