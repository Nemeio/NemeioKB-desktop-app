using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.Parameters
{
    public interface IParametersHolder
    {
        KeyboardParameters Parameters { get; }
        Task RefreshParametersAsync();
        Task UpdateParametersAsync(KeyboardParameters parameters);
    }
}
