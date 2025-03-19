using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Core
{
    public interface IKeyboardParameterParser
    {
        KeyboardParameters Parse(byte[] payload);
        byte[] ToByteArray(KeyboardParameters parameters);
    }
}
