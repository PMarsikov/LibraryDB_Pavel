using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibraryDB_Pavel.Model;

namespace LibraryDB_Pavel.ViewModel
{
    public class BooksViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Book> Books { get; set; }

        public BooksViewModel()
        {
            Books = new ObservableCollection<Book>
            {
                new()
                {
                    AuthorFirstName = "FName1", AuthorLastName = "LName1", AuthorMiddleName = "MName1",
                    AuthorBirthDay = "01.02.1980", BookTitle = "Book Name 1", BookYear = "2000"
                },
                new()
                {
                    AuthorFirstName = "FName2", AuthorLastName = "LName2", AuthorMiddleName = "MName2",
                    AuthorBirthDay = "01.02.1981", BookTitle = "Book Name 2", BookYear = "2001"
                },
                new()
                {
                    AuthorFirstName = "FName3", AuthorLastName = "LName3", AuthorMiddleName = "MName3",
                    AuthorBirthDay = "01.02.1982", BookTitle = "Book Name 3", BookYear = "2002"
                },
                new()
                {
                    AuthorFirstName = "FName2", AuthorLastName = "LName2", AuthorMiddleName = "MName2",
                    AuthorBirthDay = "01.02.1982", BookTitle = "Book Name 2 new", BookYear = "2001"
                },
            };
        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                       {
                           Book book = new()
                           {
                               AuthorFirstName = "AAA",
                               AuthorLastName = "BBB",
                               AuthorMiddleName = "CCC",
                               AuthorBirthDay = "DDD",
                               BookTitle = "MMM",
                               BookYear = "2001"
                           };

                           Books.Insert(0, book);
                       }));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
