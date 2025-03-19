using Microsoft.EntityFrameworkCore;
using Nemeio.Core;
using Nemeio.Infrastructure.DbModels;

namespace Nemeio.Infrastructure
{
    internal static class DataSeeder
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // Blacklist
            modelBuilder.Entity<BlacklistDbModel>().HasData(new BlacklistDbModel { Id = 1, Name = "nemeio", IsSystem = true });
            modelBuilder.Entity<BlacklistDbModel>().HasData(new BlacklistDbModel { Id = 2, Name = "explorer", IsSystem = true });

            // Categories
            modelBuilder.Entity<CategoryDbModel>().HasData(new CategoryDbModel { Id = NemeioConstants.DefaultCategoryId, ConfiguratorIndex = 0, Title = "Default", IsDefault = true });

            // Application settings
            modelBuilder.Entity<ApplicationSettingsDbModel>().HasData(new ApplicationSettingsDbModel() { Id = 1, Language = null, AugmentedImageEnable = true, ShowGrantPrivilegeWindow = true });
        }
    }
}
