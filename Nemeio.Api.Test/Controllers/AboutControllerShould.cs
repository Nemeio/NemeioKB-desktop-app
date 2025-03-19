using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nemeio.Api.Dto.Out;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    public class AboutControllerShould : BaseControllerShould
    {
        [Test]
        public async Task AboutController_GetApplicationArchitecture_WorksOk([Values] PlatformArchitecture architecture)
        {
            base.Setup();

            _fakeInformationService.OverrideApplicationArchitecture(architecture);

            var url = $"{GetServerUrl()}/api/about/architecture";
            var response = await _client.GetAsync(url);

            _fakeInformationService.GetApplicationArchitectureCalled.Should().BeTrue();

            await CheckRequestIsSuccess<ArchitectureOutDto>(response);
        }

        [Test]
        public async Task AboutController_GetApplicationVersion_WorksOk()
        {
            base.Setup();

            _fakeInformationService.OverrideApplicationVersion(new VersionProxy("0.1.29.4"));

            var url = $"{GetServerUrl()}/api/about/version";
            var response = await _client.GetAsync(url);
            
            _fakeInformationService.GetApplicationVersionCalled.Should().BeTrue();
            
            await CheckRequestIsSuccess<VersionOutDto>(response);
        }

        [Test]
        public async Task AboutController_GetSystemFont_WorksOk()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/about/fonts";

            var response = await _client.GetAsync(url);

            await CheckRequestIsSuccess<FontsOutDto>(response);
        }

        [Test]
        public async Task AboutController_GetSupportedLanguages_WorksOk()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/about/languages";
            var supportedLanguagesCount = _languageManager.GetSupportedLanguages().Count();
            var response = await _client.GetAsync(url);

            var result = await CheckRequestIsSuccess<IList<LanguageOutDto>>(response);
            result.Result.Count().Should().Be(supportedLanguagesCount);
        }
    }
}
