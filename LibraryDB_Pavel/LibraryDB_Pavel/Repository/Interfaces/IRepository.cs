using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LibraryDB_Pavel.Repository.Interfaces
{
    internal interface IRepository<T> : IDisposable

    {
        public IEnumerable<T> GetObjects();
        public void Create(T entity);
    }
}
