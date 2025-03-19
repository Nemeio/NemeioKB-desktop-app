using Nemeio.Core.Errors;

namespace Nemeio.Core.Keyboard.Communication.Errors
{
    public interface IKeyboardErrorConverter
    {
        ErrorCode Convert(KeyboardErrorCode keyboardErrorCode);
    }
}
