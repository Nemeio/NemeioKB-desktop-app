using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active;

namespace Nemeio.Core.Keyboard.Sessions
{
    public interface INemeioLayoutHolderProxy : IKeyboard, ICommunicationModeHolder, ILayoutHolderNemeioProxy { }
}
