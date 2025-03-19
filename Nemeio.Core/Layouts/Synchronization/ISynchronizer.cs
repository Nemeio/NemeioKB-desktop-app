using System;
using System.Threading.Tasks;
using Nemeio.Core.Layouts.Synchronization.Contexts.State;
using static Nemeio.Core.Layouts.Synchronization.Synchronizer;

namespace Nemeio.Core.Layouts.Synchronization
{
    public interface ISynchronizer
    {
        SynchornizerState State { get; }
        event EventHandler OnStateChanged;
        Task SynchronizeAsync();
        ISynchronizationContextState GetSynchronizationStateFor(Nemeio.Core.Keyboard.Nemeios.INemeio nemeio);
    }
}
