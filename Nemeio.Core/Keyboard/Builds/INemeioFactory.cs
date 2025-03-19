namespace Nemeio.Core.Keyboard.Builds
{
    public interface INemeioFactory
    {
        Nemeio CreateRunner(Keyboard keyboard);
        Nemeio CreateUpdater(Keyboard keyboard);
        Nemeio CreateVersionChecker(Keyboard keyboard);
    }
}
