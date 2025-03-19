using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In
{
    public class LanguageInDto
    {
        /// <summary>
        /// Language code (e.g. fr-FR / en-US)
        /// </summary>
        [RegularExpression(@"^[a-z]{2}-[A-Z]{2}$")]
        [JsonProperty("language", Required = Required.Always)]
        public string Language { get; set; }
    }
}
