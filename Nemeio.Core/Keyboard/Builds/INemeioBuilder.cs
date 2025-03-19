using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.Builds
{
    public interface INemeioBuilder
    {
        Task<Nemeio> BuildAsync(Keyboard keyboard);
    }
}
