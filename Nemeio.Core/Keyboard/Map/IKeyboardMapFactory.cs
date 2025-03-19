namespace Nemeio.Core.Keyboard.Map
{
    public interface IKeyboardMapFactory
    {
        KeyboardMap CreateHolitechMap();
        KeyboardMap CreateEinkMap();
    }
}
