using Nemeio.Core.Keyboard.Screens;

namespace Nemeio.Core.Layouts.Synchronization.Contexts
{
    public interface ISynchronizationContextFactory
    {
        ISynchronizationContext CreateSystemSynchronizationContext(IScreen screen);
        ISynchronizationContext CreateKeyboardSynchronizationContext(ISynchronizableNemeioProxy proxy);
    }
}
