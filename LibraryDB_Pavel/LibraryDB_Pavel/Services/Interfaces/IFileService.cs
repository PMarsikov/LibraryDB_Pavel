using System.Collections.Generic;
using LibraryDB_Pavel.Model;

namespace LibraryDB_Pavel.ViewModel
{
    public interface IFileService
    {
        IEnumerable<Book> Open(string filename);
    }
}
