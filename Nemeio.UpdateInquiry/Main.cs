using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Nemeio.UpdateInquiry.Builders;
using Nemeio.UpdateInquiry.Parser;
using Nemeio.UpdateInquiry.Repositories;

namespace Nemeio.UpdateInquiry
{
    public static class Main
    {
        [FunctionName("updateInformation")]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("-- Update function triggered");

                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var selectedEnvironnment = await GetSelectedEnvironmentAsync(config, req, log);
                var updateBuilder = new UpdateBuilder(selectedEnvironnment, config);
                var outPut = updateBuilder.Build();

                return new OkObjectResult(outPut);
            }
            catch (Exception exception)
            {
                log.LogError(exception, "Get updates failed");

                return new BadRequestObjectResult(exception);
            }
        }

        [FunctionName("getBinaries")]
        public static IActionResult GetBinaries(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tests/binaries")] HttpRequest httpRequest,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("-- Get binaries triggered");

            const string EnvironmentKey = "environment";
            const string PlatformKey = "platform";
            const string ComponentKey = "component";

            var configuration = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var outputBuilder = new BinaryListBuilder(configuration);

            if (httpRequest.Query.ContainsKey(EnvironmentKey))
            {
                var environment = httpRequest
                    .Query[EnvironmentKey]
                    .ToString()
                    .ToLower();

                outputBuilder = outputBuilder.SetEnvironment(environment);
            }

            if (httpRequest.Query.ContainsKey(PlatformKey))
            {
                var platform = httpRequest
                    .Query[PlatformKey]
                    .ToString()
                    .ToLower();

                outputBuilder = outputBuilder.SetPlatform(platform);
            }

            if (httpRequest.Query.ContainsKey(ComponentKey))
            {
                var component = httpRequest
                    .Query[ComponentKey]
                    .ToString()
                    .ToLower();

                outputBuilder = outputBuilder.SetComponent(component);
            }

            var output = outputBuilder.Build();

            return new OkObjectResult(output);
        }

        private static async Task<UpdateEnvironment> GetSelectedEnvironmentAsync(IConfigurationRoot configurationRoot, HttpRequest httpRequest, ILogger log)
        {
            const string KeyboardIdQueryParameter = "keyboardId";

            try
            {
                var keyboardId = httpRequest.Query[KeyboardIdQueryParameter].ToString();
                keyboardId = keyboardId.ToLower();

                var keyboardRepository = new KeyboardTableStorageRepository(configurationRoot);
                var keyboards = await keyboardRepository.GetKeyboards();

                var foundKeyboard = keyboards.First(x => x.Id.ToLower().Equals(keyboardId));

                return foundKeyboard.Environment;
            }
            catch (InvalidOperationException)
            {
                //  Default case : we return master environment

                return UpdateEnvironment.Master;
            }
        }
    }
}
