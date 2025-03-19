using System.Threading.Tasks;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Configurations.Add
{
    public interface IAddConfigurationHolder
    {
        Task AddLayoutAsync(ILayout layout);
    }
}
