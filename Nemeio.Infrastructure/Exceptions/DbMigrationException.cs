using System;
using System.Runtime.Serialization;

namespace Nemeio.Infrastructure.Exceptions
{
    [Serializable]
    public class DbMigrationException : Exception
    {
        public DbMigrationException()
        {
        }

        public DbMigrationException(string message) : base(message)
        {
        }

        public DbMigrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DbMigrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
