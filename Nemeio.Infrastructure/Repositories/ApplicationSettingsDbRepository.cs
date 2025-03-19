using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.AppSettings;
using Nemeio.Infrastructure.DbModels;

namespace Nemeio.Infrastructure.Repositories
{
    public class ApplicationSettingsDbRepository : DbRepository, IApplicationSettingsDbRepository
    {
        public ApplicationSettingsDbRepository(ILoggerFactory loggerFactory, IDatabaseAccessFactory dbAccessFactory)
            : base(loggerFactory.CreateLogger<ApplicationSettingsDbRepository>(), dbAccessFactory) { }

        public ApplicationSettings ReadApplicationSettings()
        {
            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var applicationSettings = dbAccess.DbContext.ApplicationSettings.SingleOrDefault();
                if (applicationSettings == null)
                    throw new ArgumentException("Unable to read ApplicationSettings");
                return applicationSettings.ToDomainModel();
            }
        }

        public void SaveApplicationSettings(ApplicationSettings appSettings)
        {
            if (appSettings == null)
            {
                throw new InvalidOperationException("ApplicationSettings cannot be null.");
            }

            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                var applicationSettings = dbAccess.DbContext.ApplicationSettings.SingleOrDefault() ?? new ApplicationSettingsDbModel();
                applicationSettings.Language = appSettings.Language?.Name ?? null;
                applicationSettings.AugmentedImageEnable = appSettings.AugmentedImageEnable;
                applicationSettings.ShowGrantPrivilegeWindow = appSettings.ShowGrantPrivilegeWindow;
                applicationSettings.UpdateTo = appSettings.UpdateTo == null ? null : appSettings.UpdateTo.ToString();
                applicationSettings.LastRollbackManifestString = appSettings.LastRollbackManifestString;

                if (applicationSettings.Id == 0)
                {
                    dbAccess.DbContext.Add(applicationSettings);
                }
                else
                {
                    dbAccess.DbContext.Update(applicationSettings);
                }

                dbAccess.DbContext.SaveChanges();
            }
        }
    }
}
