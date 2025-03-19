using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.Blacklist;
using Nemeio.Infrastructure.DbModels;

namespace Nemeio.Infrastructure.Repositories
{
    public class BlacklistDbRepository : DbRepository, IBlacklistDbRepository
    {
        public BlacklistDbRepository(ILoggerFactory loggerFactory, IDatabaseAccessFactory dbAccessFactory)
            : base(loggerFactory.CreateLogger<BlacklistDbRepository>(), dbAccessFactory) { }

        public Blacklist FindBlacklistByName(string name)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Blacklists.FirstOrDefault(o => o.Name == name);

                return data?.ToDomainModel();
            }
        }

        public Blacklist FindBlacklistById(int id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Blacklists.FirstOrDefault(o => o.Id == id);

                return data?.ToDomainModel();
            }
        }

        public IEnumerable<Blacklist> ReadSystemBlacklists()
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Blacklists.Where(o => o.IsSystem).ToList();

                return data.Select(o => o.ToDomainModel()).ToList();
            }
        }

        public IEnumerable<Blacklist> ReadBlacklists()
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Blacklists.Where(o => !o.IsSystem).ToList();

                return data.Select(o => o.ToDomainModel()).ToList();
            }
        }

        public Blacklist SaveBlacklist(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Blacklists.FirstOrDefault(o => o.Name == name);

                if (data == null)
                {
                    data = new BlacklistDbModel()
                    {
                        Name = name,
                        IsSystem = false
                    };

                    dbAccess.DbContext.Add(data);
                    dbAccess.DbContext.SaveChanges();
                }

                return data.ToDomainModel();
            }
        }

        public bool DeleteBlacklist(int id)
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var data = dbAccess.DbContext.Blacklists.FirstOrDefault(o => o.Id == id);
                if (data == null || data.IsSystem)
                {
                    return false;
                }

                dbAccess.DbContext.Remove(data);
                dbAccess.DbContext.SaveChanges();

                return true;
            }
        }
    }
}
