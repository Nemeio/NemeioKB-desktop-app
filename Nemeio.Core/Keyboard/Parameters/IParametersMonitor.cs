namespace Nemeio.Core.Keyboard.Parameters
{
    public interface IParametersMonitor
    {
        IKeyboardParameterParser Parser { get; }
        KeyboardParameters GetParameters();
        void SetParameters(KeyboardParameters parameters);
    }
}
