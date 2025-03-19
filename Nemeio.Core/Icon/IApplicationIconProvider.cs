namespace Nemeio.Core.Icon
{
    public interface IApplicationIconProvider
    {
        ApplicationIconType GetIconFromCurrentState();

        string GetIconResourceFromCurrentState();
    }
}
