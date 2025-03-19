using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.PackageUpdater;

namespace Nemeio.Core.Keyboard.Nemeios
{
    public interface INemeio : IKeyboard, IBatteryHolder, IFactoryResetHolder, IKeyboardFailuresHolder, IParametersHolder, ICommunicationModeHolder
    {
        Task InitializeAsync();
        Task DisconnectAsync();
    }
}
