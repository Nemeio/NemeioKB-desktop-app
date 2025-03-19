using System;
using Nemeio.UpdateInquiry.Models;

namespace Nemeio.UpdateInquiry.Parser
{
    public class PlatformParser : IParser<Platform>
    {
        public Platform Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            return (Platform)Enum.Parse(typeof(Platform), data);
        }

        public string Parse(Platform data)
        {
            throw new NotImplementedException();
        }
    }
}
