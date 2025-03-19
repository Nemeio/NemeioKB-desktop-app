using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.JsonModels;
using Nemeio.Tools.Testing.Update.Core.System;
using Nemeio.Tools.Testing.Update.Core.Update.Environment;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Nemeio.Tools.Testing.Update.Application.Update
{
    public class InstallerRepository : IInstallerRepository
    {
        private readonly ISystemInformation _systemInformation;

        public InstallerRepository(ISystemInformation systemInformation)
        {
            _systemInformation = systemInformation ?? throw new ArgumentNullException(nameof(systemInformation));
        }

        public async Task<IEnumerable<Installer>> GetInstallers(Uri serverUri, NemeioEnvironment environment)
        {
            var client = new RestClient(serverUri)
                .UseNewtonsoftJson();

            var request = new RestRequest($"/tests/binaries?environment={GetEnvironment(environment)}&platform={GetPlatform()}&component={GetComponent()}", Method.GET);
            var response = await client.ExecuteAsync<InstallersInDto>(request);
            if (response != null && response.IsSuccessful)
            {
                return response
                    .Data
                    .Installers
                    .Select(installer => installer.ToModel());
            }

            return null;
        }

        private string GetComponent()
        {
            var system = _systemInformation.GetRunningSystem();
            switch (system)
            {
                case Nemeio.Core.Services.OperatingSystem.Windows:
                    return "win";
                case Nemeio.Core.Services.OperatingSystem.Osx:
                    return "mac";
                default:
                    throw new InvalidOperationException("Not supported component");
            }
        }

        private string GetEnvironment(NemeioEnvironment environment)
        {
            switch (environment)
            {
                case NemeioEnvironment.Develop:
                    return "develop";
                case NemeioEnvironment.Testing:
                    return "testing";
                case NemeioEnvironment.Master:
                    return "master";
                default:
                    throw new InvalidOperationException("Not supported environment");
            }
        }

        private string GetPlatform()
        {
            var platform = _systemInformation.GetCurrentPlatform();
            switch (platform)
            {
                case PlatformArchitecture.x64:
                    return "x64";
                case PlatformArchitecture.x86:
                    return "x86";
                default:
                    throw new InvalidOperationException("Not supported platform");
            }
        }
    }
}
