using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibraryDB_Pavel.Model
{
    public class Book : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _authorFirstName;
        private string _authorLastName;
        private string _authorMiddleName;
        private string _authorBirthDay;
        private string _bookTitle;
        private string _bookYear;

        public string AuthorFirstName
        {
            get => _authorFirstName;
            set
            {
                _authorFirstName = value;
                OnPropertyChanged(nameof(AuthorFirstName));
            }
        }

        public string AuthorLastName
        {
            get => _authorLastName;
            set
            {
                _authorLastName = value;
                OnPropertyChanged(nameof(AuthorLastName));
            }
        }

        public string AuthorMiddleName
        {
            get => _authorMiddleName;
            set
            {
                _authorMiddleName = value;
                OnPropertyChanged(nameof(AuthorMiddleName));
            }
        }

        public string AuthorBirthDay
        {
            get => _authorBirthDay;
            set
            {
                _authorBirthDay = value;
                OnPropertyChanged(nameof(AuthorBirthDay));
            }
        }

        public string BookTitle
        {
            get => _bookTitle;
            set
            {
                _bookTitle = value;
                OnPropertyChanged(nameof(BookTitle));
            }
        }

        public string BookYear
        {
            get => _bookYear;
            set
            {
                _bookYear = value;
                OnPropertyChanged(nameof(BookYear));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
