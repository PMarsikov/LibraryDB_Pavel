using LibraryDB_Pavel.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryDB_Pavel.Repository.Interfaces
{
    public interface IDbBookContext
    {
        public DbSet<Book> Books { get; set; }
    }
}
