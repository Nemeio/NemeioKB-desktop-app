using Nemeio.Core.Icon;

namespace Nemeio.Presentation.Menu.Icon
{
    public class ApplicationIcon
    {
        public ApplicationIconType Type { get; private set; }

        public ApplicationIcon(ApplicationIconType type)
        {
            Type = type;
        }
    }
}
