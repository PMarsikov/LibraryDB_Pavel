using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using LibraryDB_Pavel.Model;
using LibraryDB_Pavel.Extensions;
using LibraryDB_Pavel.Repository;
using LibraryDB_Pavel.Repository.Interfaces;
using LibraryDB_Pavel.Utils.Constants;
using LibraryDB_Pavel.Utils.Enums;
using Microsoft.EntityFrameworkCore;


namespace LibraryDB_Pavel.ViewModel
{
    public class BooksViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Book> Books { get; set; }
        private readonly IRepository<Book> _bookRepository;
        private string _filterValue;
        private BookEnums.BooksRows _mySelectedItem;
        private RelayCommand _openCommand;
        private RelayCommand _filterCommand;
        private RelayCommand _showMessage;
        private readonly IDbBookContext _dbContext;
        IFileService fileService;
        IDialogService dialogService;

        public BooksViewModel(IDialogService dialogService, IFileService fileService, IDbBookContext dbContext)
        {
            this._bookRepository = new BookRepository(new DbBookContext());
            Books = _bookRepository.GetObjects().ToObservableCollection();
            this.dialogService = dialogService;
            this.fileService = fileService;
            _dbContext = dbContext;
        }

        public BookEnums.BooksRows MySelectedItem
        {
            get => _mySelectedItem;
            set { _mySelectedItem = value; }
        }

        public string FilterValue
        {
            get => _filterValue;
            set { _filterValue = value; }
        }

        public RelayCommand OpenCommand
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return //_openCommand ??
                    (_openCommand = new RelayCommand(obj =>

                    {
                        try
                        {
                            if (dialogService.OpenFileDialog())
                            {
                                var books =
                                    fileService.Open(dialogService.FilePath);
                                foreach (var book in books)
                                {
                                    _bookRepository.Create(book);
                                    Books.Add(book);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //  dialogService.ShowMessage(ex.Message);
                        }
                    }));
            }
        }

        public RelayCommand FilterCommand
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return //_openCommand ??
                    (_filterCommand = new RelayCommand(obj =>
                    {
                        var books = _dbContext.Books;
                        var selectedBooks = FilteredList(_mySelectedItem, books);
                        Books.Clear();
                        foreach (var book in selectedBooks)
                            Books.Add(book);
                    }));
            }
        }

        private IOrderedQueryable<Book> FilteredList(BookEnums.BooksRows property, DbSet<Book> books)
        {
            switch (property)
            {
                case BookEnums.BooksRows.AuthorMiddleName:
                {
                    return books.Where(book => book.AuthorMiddleName.ToUpper().Contains(_filterValue.ToUpper()))
                        .OrderBy(book => book);
                }
                case BookEnums.BooksRows.AuthorLastName:
                {
                    return books.Where(book => book.AuthorLastName.ToUpper().Contains(_filterValue.ToUpper()))
                        .OrderBy(book => book);
                }
                case BookEnums.BooksRows.BookTitle:
                {
                    return books.Where(book => book.BookTitle.ToUpper().Contains(_filterValue.ToUpper()))
                        .OrderBy(book => book);
                }
                case BookEnums.BooksRows.BookYear:
                {
                    return books.Where(book => book.BookYear.ToUpper().Contains(_filterValue.ToUpper()))
                        .OrderBy(book => book);
                }
                default:
                    return books.Where(book => book.AuthorFirstName.ToUpper().Contains(_filterValue.ToUpper()))
                        .OrderBy(book => book);
            }
        }

        public RelayCommand ShowMessage
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return //_openCommand ??
                    (_openCommand = new RelayCommand(obj =>
                    {
                        MessageBox.Show(string.Format(MessagesConstants.HelpFileFormat, BookConstants.CsvSeparator),
                            MessagesConstants.HelpFileFormatWindowTitle);
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
