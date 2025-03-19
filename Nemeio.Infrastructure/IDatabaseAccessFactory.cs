namespace Nemeio.Infrastructure
{
    public interface IDatabaseAccessFactory
    {
        DbAccess CreateDatabaseAccess();
    }
}
