using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibraryDB_Pavel.Model;
using LibraryDB_Pavel.Utils.Constants;
using LibraryDB_Pavel.ViewModel;

namespace LibraryDB_Pavel
{
    public class FileService : IFileService
    {
        public IEnumerable<Book> Open(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);

            return lines.Select(line =>
            {
                string[] data = line.Split(BookConstants.CsvSeparator);
                Console.WriteLine(data.Length);
                return
                    new Book
                    {
                        AuthorFirstName = data[0],
                        AuthorLastName = data[1],
                        AuthorMiddleName = data[2],
                        AuthorBirthDay = data[3],
                        BookTitle = data[4],
                        BookYear = data[5]
                    };
            });
        }
    }
}
