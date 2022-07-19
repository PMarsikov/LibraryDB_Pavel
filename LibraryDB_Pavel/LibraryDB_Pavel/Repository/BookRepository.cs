using System;
using System.Collections.Generic;
using System.Linq;
using LibraryDB_Pavel.Model;
using LibraryDB_Pavel.Repository.Interfaces;


namespace LibraryDB_Pavel.Repository
{
    internal class BookRepository : IRepository<Book>
    {
        private readonly DbBookContext _dbContext;

        public BookRepository(DbBookContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Book> GetObjects()
        {
            try
            {
                return _dbContext.Books.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Enumerable.Empty<Book>();
            }
           
        }

        public void Create(Book entity)
        {
            _dbContext.Books.Add(entity);
            _dbContext.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}