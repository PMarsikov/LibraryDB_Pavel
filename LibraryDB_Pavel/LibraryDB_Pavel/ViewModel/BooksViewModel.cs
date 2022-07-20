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
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

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
            _dbContext = dbContext;
            this._bookRepository = new BookRepository(new DbBookContext());
            Books = _bookRepository.GetObjects().ToObservableCollection();
            this.dialogService = dialogService;
            this.fileService = fileService;

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
                                try
                                {
                                    var books =
                                        fileService.Open(dialogService.FilePath);
                                    foreach (var book in books)
                                    {
                                        var ppp = book;
                                        Books.Add(book);
                                        _bookRepository.Create(book);

                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    MessageBox.Show(MessagesConstants.WrongCsvFormatMsg);
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
                        MessageBox.Show(string.Format(MessagesConstants.DataExportedToFile, path));
                    }));
            }
        }

        public RelayCommand ExportToExcelCommand
        {
            get
            {
                // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
                return //_openCommand ??
                    (_exportToXmlCommand = new RelayCommand(obj =>
                    {
                        var path = FilePath(BookConstants.FileNameExcel, ".xlsx");
                        var data = ToDataTable(Books.ToList());
                        ToExcelFile(data, path);
                        MessageBox.Show(string.Format(MessagesConstants.DataExportedToFile, path));
                    }));
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var type = (prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType)
                    : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }

            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static void ToExcelFile(DataTable dataTable, string filePath, bool overwriteFile = true)
        {
            if (File.Exists(filePath) && overwriteFile)
                File.Delete(filePath);

            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = string.Format(BookConstants.ExcelPathConstant, filePath);
                connection.Open();
                using (var command = new OleDbCommand())
                {
                    command.Connection = connection;
                    var columnNames = (from DataColumn dataColumn in dataTable.Columns select dataColumn.ColumnName)
                        .ToList();
                    var tableName = !string.IsNullOrWhiteSpace(dataTable.TableName)
                        ? dataTable.TableName
                        : Guid.NewGuid().ToString();
                    command.CommandText =
                        $"CREATE TABLE [{tableName}] ({string.Join(",", columnNames.Select(c => $"[{c}] VARCHAR").ToArray())});";
                    command.ExecuteNonQuery();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var rowValues = (from DataColumn column in dataTable.Columns
                            select (row[column] != null && row[column] != DBNull.Value)
                                ? row[column].ToString()
                                : string.Empty).ToList();
                        command.CommandText =
                            $"INSERT INTO [{tableName}]({string.Join(",", columnNames.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
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
