using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using LibraryDB_Pavel.Model;
using LibraryDB_Pavel.Extensions;
using LibraryDB_Pavel.Repository;
using LibraryDB_Pavel.Repository.Interfaces;


namespace LibraryDB_Pavel.ViewModel
{
    public class BooksViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Book> Books { get; set; }
        private readonly IRepository<Book> _bookRepository;
        IFileService fileService;
        IDialogService dialogService;

        public BooksViewModel(IDialogService dialogService, IFileService fileService)
        {
            this._bookRepository = new BookRepository(new DbBookContext());
            Books = _bookRepository.GetObjects().ToObservableCollection();
            this.dialogService = dialogService;
            this.fileService = fileService;
        }

        private RelayCommand _openCommand;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
