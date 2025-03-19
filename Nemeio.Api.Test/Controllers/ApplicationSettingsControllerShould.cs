using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Nemeio.Api.Dto.In;
using Nemeio.Api.Dto.Out;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    [TestFixture]
    internal sealed class ApplicationSettingsControllerShould : BaseControllerShould
    {
        [Test]
        public async Task ApplicationSettingsController_GetApplicationLanguage_WorksOk()
        {
            base.Setup();

            const string languageCode = "fr-FR";

            Mock.Get(_languageManager)
                .Setup(x => x.GetCurrentCultureInfo())
                .Returns(new CultureInfo(languageCode));

            var url = $"{GetServerUrl()}/api/settings/language";

            var response = await _client.GetAsync(url);

            var dto = await CheckRequestIsSuccess<LanguageOutDto>(response);

            dto.Result.Code.Should().Be(languageCode);
        }

        [TestCase("fr-FR")]
        [TestCase("en-US")]
        public async Task ApplicationSettingsController_SetApplicationLanguage_WhenValueIsKnown_WorksOk(string languageCode)
        {
            base.Setup();

            CultureInfo cultureInfoSet = null;

            Mock.Get(_languageManager)
                .Setup(x => x.SetCurrentCultureInfo(It.IsAny<CultureInfo>()))
                .Callback<CultureInfo>((info) => cultureInfoSet = info);

            var url = $"{GetServerUrl()}/api/settings/language";

            var inDto = new LanguageInDto()
            {
                Language = languageCode
            };
            var json = JsonConvert.SerializeObject(inDto);

            using (var jsonContent = CreateJsonContent(json))
            {
                var response = await _client.PostAsync(url, jsonContent);

                _ = await CheckRequestIsSuccess<BaseOutDto>(response);

                cultureInfoSet.Should().NotBeNull();
                cultureInfoSet.TwoLetterISOLanguageName.Should().Be(cultureInfoSet.TwoLetterISOLanguageName);
            }
        }

        [Test]
        public async Task ApplicationSettingsController_SetApplicationLanguage_WhenValueIsNotKnown_ReturnFail()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/settings/language";

            var inDto = new LanguageInDto()
            {
                Language = "zz-ZZ"
            };
            var json = JsonConvert.SerializeObject(inDto);

            using (var jsonContent = CreateJsonContent(json))
            {
                var response = await _client.PostAsync(url, jsonContent);

                await CheckRequestIsError<BaseOutDto>(response, Core.Errors.ErrorCode.ApiInvalidParameters);
            }
        }
    }
}
