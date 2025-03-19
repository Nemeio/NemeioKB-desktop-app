using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.Errors;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out
{
    public class ErrorOutDto<T>
    {
        [JsonProperty(ErrorJsonKeys.ErrorCode)]
        public ErrorCode ErrorCode { get; set; }

        [JsonProperty(ErrorJsonKeys.ErrorDescription)]
        public string ErrorDescription { get; set; }

        [JsonProperty(ErrorJsonKeys.Result)]
        public T Result { get; set; }
    }
}
