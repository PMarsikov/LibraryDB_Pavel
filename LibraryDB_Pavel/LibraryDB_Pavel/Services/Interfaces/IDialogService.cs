namespace LibraryDB_Pavel.ViewModel
{
    public interface IDialogService
    {
        string FilePath { get; set; }
        bool OpenFileDialog();
    }
}
