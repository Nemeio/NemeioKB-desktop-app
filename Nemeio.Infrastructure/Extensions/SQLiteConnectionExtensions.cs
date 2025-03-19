
using Microsoft.Data.Sqlite;

namespace Nemeio.Infrastructure.Extensions
{
    public static class SqliteConnectionExtensions
    {
        public static void Secure(this SqliteConnection connection, string password)
        {
            string cleanedPassword = DbPasswordProvider.CleanPassword(password);
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA key = '{cleanedPassword}';"; //NOSONAR The pasword has been cleaned
            command.ExecuteNonQuery();
        }
    }
}
