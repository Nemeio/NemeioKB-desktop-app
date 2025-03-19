using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.FactoryReset
{
    public interface IFactoryResetHolder
    {
        Task WantFactoryResetAsync();
        Task ConfirmFactoryResetAsync();
        Task CancelFactoryResetAsync();
    }
}
