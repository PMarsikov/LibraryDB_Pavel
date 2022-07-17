using LibraryDB_Pavel.Configs;
using LibraryDB_Pavel.Model;
using LibraryDB_Pavel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace LibraryDB_Pavel.Repository
{
    internal class DbBookContext : DbContext, IDbBookContext
    {
        private readonly Settings _settings;

        public DbBookContext(DbContextOptions<DbBookContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }

        public DbBookContext()
        {
            _settings = new Configs.Configs().GetConfig();
            //Database.EnsureDeleted();
            Database.EnsureCreated();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_settings.DbConnection);
        }
    }
}