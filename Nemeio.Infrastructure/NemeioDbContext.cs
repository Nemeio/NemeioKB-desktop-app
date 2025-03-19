using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nemeio.Infrastructure.DbModels;

namespace Nemeio.Infrastructure
{
    public class NemeioDbContext : DbContext
    {
        private DbConnection _dbConnection;

        public DbSet<ApplicationSettingsDbModel> ApplicationSettings { get; set; }

        public DbSet<BlacklistDbModel> Blacklists { get; set; }

        public DbSet<CategoryDbModel> Categories { get; set; }

        public DbSet<LayoutDbModel> Layouts { get; set; }

        public NemeioDbContext()
        {
            _dbConnection = new SqliteConnection("DataSource=:memory:");
        }

        public NemeioDbContext(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_dbConnection);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LayoutDbModel>().HasKey(ba => new { ba.Id, ba.Screen });

            modelBuilder.SeedData();
        }
    }
}