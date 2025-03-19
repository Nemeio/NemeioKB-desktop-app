using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Configurations.Add
{
    public interface IAddConfigurationMonitor
    {
        void SendConfiguration(ILayout layout, bool isFactory = false);
    }
}
