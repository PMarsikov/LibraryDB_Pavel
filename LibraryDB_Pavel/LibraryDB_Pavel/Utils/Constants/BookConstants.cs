namespace LibraryDB_Pavel.Utils.Constants
{
    public static class BookConstants
    {
        public const char CsvSeparator = ';';
        public const string FileNameXml = "books_in_xml_";
        public const string FileNameExcel = "books_in_excel_";
        public const string FolderForExportedData = "\\exported_data\\";
        public const string ExcelPathConstant =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;'";
    }
}
