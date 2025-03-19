using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nemeio.Api.Dto.Out;
using Nemeio.Core.Errors;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    public class ErrorControllerShould : BaseControllerShould
    {
        [Test]
        [TestCaseSource("GetApiErrors")]
        public async Task ErrorController_01_01_PostTestError_WorksOk(ErrorCode error)
        {
            base.Setup();

            var errorCode = (int)error;

            var url = $"{GetServerUrl()}/api/dev/error/{errorCode}";
            var response = await _client.PostAsync(url, new StringContent(string.Empty));

            await CheckRequestIsError<BaseOutDto>(response, error);
        }

        public static Array GetApiErrors() => Enum.GetValues(typeof(ErrorCode));
    }
}
