using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nemeio.UpdateInquiry.Builders;
using Nemeio.UpdateInquiry.Models;
using Nemeio.UpdateInquiry.Parser;

namespace Nemeio.UpdateInquiry.Repositories
{
    public class KeyboardTableStorageRepository : BaseStorageRepository, IKeyboardRepository
    {
        private const string TableName          = "Keyboards";
        private const string PartitionName      = "Nemeio";

        public KeyboardTableStorageRepository(IConfigurationRoot configurationRoot)
            : base(configurationRoot) { }

        public async Task<IEnumerable<Keyboard>> GetKeyboards()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);

            await table.CreateIfNotExistsAsync();

            var query = new TableQuery<KeyboardDbModel>();
            query.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionName));

            var databaseKeyboards = await table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            return databaseKeyboards.Results.Select(x => new Keyboard() 
            { 
                Id = x.RowKey,
                Environment = GetEnvironmentFromString(x.Environment)
            });
        }

        private UpdateEnvironment GetEnvironmentFromString(string environment)
        {
            try
            {
                return new EnvironmentParser().Parse(environment);
            }
            //  In this case we want to return Master environment by default
            catch(InvalidOperationException)
            {
                return UpdateEnvironment.Master;
            }
        }
    }
}
