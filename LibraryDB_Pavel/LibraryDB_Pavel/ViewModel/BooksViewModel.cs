﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Linq;
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
        private BookEnums.BooksRows _filterPropertyName;
        private RelayCommand _openCommand;
        private RelayCommand _filterCommand;
        private RelayCommand _clearFilter;
        private RelayCommand _showMessage;
        private RelayCommand _exportToXmlCommand;
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

        public BookEnums.BooksRows FilterPropertyName
        {
            get => _filterPropertyName;
            set { _filterPropertyName = value; }
        }

        public string FilterValue
        {
            get => _filterValue;
            set
            {
                _filterValue = value;
                OnPropertyChanged(nameof(FilterValue));
            }
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

        public RelayCommand ExportToXmlCommand
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return //_openCommand ??
                    (_exportToXmlCommand = new RelayCommand(obj =>
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
                        var path = FilePath(BookConstants.FileNameXml, ".xml");
                        xdoc.Save(path);
                        MessageBox.Show(string.Format(MessagesConstants.DataExportedToXml, path));
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
                        try
                        {
                            if (_filterValue is null or "")
                            {
                                MessageBox.Show(MessagesConstants.ErrorInputMsg);
                                return;
                            }
                            var books = _dbContext.Books;
                            var filteredBooks = FilteredList(_filterPropertyName, books);
                            Books.Clear();
                            foreach (var book in filteredBooks)
                                Books.Add(book);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }));
            }
        }

        public RelayCommand ClearFilter
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return //_openCommand ??
                    (_clearFilter = new RelayCommand(obj =>
                    {
                        var books = _dbContext.Books;
                        Books.Clear();
                        foreach (var book in books)
                            Books.Add(book);
                        FilterValue = "";
                    }));
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

        private string FilePath(string fileName, string fileExtension)
        {
            string currentDirectory;
            try
            {
                currentDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var resultsFolder = currentDirectory + BookConstants.FolderForExportedData;
            var dateTime = DateTime.Now.ToFileTimeUtc();
            var dirInfo = new DirectoryInfo(resultsFolder);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            return resultsFolder + fileName + dateTime + fileExtension;
        }
    }
}
