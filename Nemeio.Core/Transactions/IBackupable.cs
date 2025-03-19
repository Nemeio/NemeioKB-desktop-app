namespace Nemeio.Core.Transactions
{
    public interface IBackupable<T>
    {
        T CreateBackup();
    }
}
