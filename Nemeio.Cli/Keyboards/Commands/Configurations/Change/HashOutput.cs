using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Change
{
    internal sealed class HashOutput
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        public HashOutput(string id)
        {
            Id = id;
        }
    }
}
