namespace Nemeio.Core.Keyboard.Screens
{
    public interface IScreenFactory
    {
        IScreen CreateEinkScreen();
        IScreen CreateHolitechScreen();
        IScreen CreateScreen(ScreenType type);
    }
}
