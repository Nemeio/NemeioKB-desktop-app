using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.State;

namespace Nemeio.Core.Layouts.Synchronization
{
    public interface ISynchronizableNemeioProxy : IConfigurationHolder, IAddConfigurationHolder, IDeleteConfigurationHolder, IStateHolder { }
}
