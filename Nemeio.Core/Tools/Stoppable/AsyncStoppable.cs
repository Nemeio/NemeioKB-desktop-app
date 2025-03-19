using System.Threading.Tasks;

namespace Nemeio.Core.Tools.Stoppable
{
    public abstract class AsyncStoppable : Stoppable, IAsyncStoppable
    {
        public AsyncStoppable(bool autoStart = true)
            : base(autoStart) { }

        public virtual async Task StopAsync()
        {
            await Task.Yield();

            Stop();
        }
    }
}
