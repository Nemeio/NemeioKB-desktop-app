using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.SetLed
{
    public interface ISetLedHolder
    {
        Task WantFactoryResetAsync();
        Task ConfirmFactoryResetAsync();
        Task CancelFactoryResetAsync();
    }
}
