using System.Threading.Tasks;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Configurations.Apply
{
    public interface IApplyConfigurationHolder
    {
        Task ApplyLayoutAsync(ILayout layout);
    }
}
