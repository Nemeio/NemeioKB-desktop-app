using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In
{
    public class ShowGrantPrivilegeWindow
    {
        /// <summary>
        /// GrantPrivilege Window status.
        /// True = Enabled
        /// False = Disabled
        /// </summary>
        [JsonProperty("status")]
        public bool Status { get; set; }
    }
}
