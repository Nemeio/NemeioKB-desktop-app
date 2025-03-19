using System;
using Nemeio.UpdateInquiry.Models;

namespace Nemeio.UpdateInquiry.Parser
{
    public class ComponentParser : IParser<Component>
    {
        private const string Windows = "win";
        private const string Mac = "mac";

        public Component Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            switch (data)
            {
                case Windows:
                    return Component.WindowsInstaller;
                case Mac:
                    return Component.MacInstaller;
                default:
                    throw new InvalidOperationException("Not supported component");
            }
        }

        public string Parse(Component data)
        {
            throw new NotImplementedException();
        }
    }
}
