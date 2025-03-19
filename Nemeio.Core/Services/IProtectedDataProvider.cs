namespace Nemeio.Core.Services
{
    public interface IProtectedDataProvider
    {
        void SavePassword(string storeName, string password);

        string GetPassword(string storeName);
    }
}
