using System;
using System.Linq;
using System.Windows;
using LibraryDB_Pavel.Repository;
using LibraryDB_Pavel.Utils.Enums;
using LibraryDB_Pavel.ViewModel;

namespace LibraryDB_Pavel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = new BooksViewModel(new DialogService(), new FileService(), new DbBookContext());
            BooksCbList.ItemsSource = Enum.GetValues(typeof(BookEnums.BooksRows)).Cast<BookEnums.BooksRows>();
        }
    }
}
