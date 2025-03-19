using System.Threading.Tasks;

namespace Nemeio.Core.Tools.Stoppable
{
    public interface IAsyncStoppable :IStoppable
    {
        Task StopAsync();
    }
}
