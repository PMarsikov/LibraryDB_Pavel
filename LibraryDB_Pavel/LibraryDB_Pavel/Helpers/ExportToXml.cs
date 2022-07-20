using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using LibraryDB_Pavel.Model;
using LibraryDB_Pavel.Utils.Constants;

namespace LibraryDB_Pavel.Helpers
{
    public class ExportToXml
    {
        public static void SaveToXml(ObservableCollection<Book> Books, string path)
        {
            XDocument xdoc = new XDocument();
            XElement root = new XElement("TestProgram");
            foreach (var book in Books)
            {
                XElement record = new XElement("Record");
                record.Add(new XAttribute("id", book.Id));
                record.Add(new XElement("FirstName", book.AuthorFirstName));
                record.Add(new XElement("MiddleName", book.AuthorMiddleName));
                record.Add(new XElement("AuthorLastName", book.AuthorLastName));
                record.Add(new XElement("AuthorBirthDay", book.AuthorBirthDay));
                record.Add(new XElement("BookTitle", book.BookTitle));
                record.Add(new XElement("BookYear", book.BookYear));
                root.Add(record);
            }

            xdoc.Add(root);
            xdoc.Save(path);

        }
    }
}
