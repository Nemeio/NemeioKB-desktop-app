using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In
{
    public class AugmentedImageStatusInDto
    {
        /// <summary>
        /// Augmented image status.
        /// True = Enabled
        /// False = Disabled
        /// </summary>
        [JsonProperty("status")]
        public bool Status { get; set; }
    }
}
