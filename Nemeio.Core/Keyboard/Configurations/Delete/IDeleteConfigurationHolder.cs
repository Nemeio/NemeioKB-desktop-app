using System.Threading.Tasks;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Configurations.Delete
{
    public interface IDeleteConfigurationHolder
    {
        Task DeleteLayoutAsync(LayoutId layoutId);
    }
}
