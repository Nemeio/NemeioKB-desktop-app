using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.SetLed
{
    public interface ICheckComponentsHolder
    {
        Task CheckComponent(byte componentId);
    }
}
