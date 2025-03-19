using Newtonsoft.Json;

namespace Nemeio.Cli.Application.Format
{
    public sealed class JsonOutputFormatter : IOutputFormatter
    {
        public string Format<T>(T obj) where T : class
        {
            var json = JsonConvert.SerializeObject(obj);

            return json;
        }
    }
}
