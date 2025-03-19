namespace Nemeio.Core.Services
{
    public interface IBrowserFile
    {
        string FindApplicationUrl();

        void OpenApplications(string[] path);

        void OpenApplication(string path);

        void OpenUrl(string url);

        void OpenUrls(string[] urls);
    }
}
