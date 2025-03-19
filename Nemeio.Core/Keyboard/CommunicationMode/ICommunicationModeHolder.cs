using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.CommunicationMode
{
    public interface ICommunicationModeHolder
    {
        Task SetHidModeAsync();
        Task SetAdvancedModeAsync();
    }
}
