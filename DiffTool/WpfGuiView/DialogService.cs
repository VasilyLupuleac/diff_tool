using System.Windows;
public interface IDialogService
{
    string OpenFileDialog();
}

public class DialogService : IDialogService
{
    public string OpenFileDialog()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog();

        var result = dlg.ShowDialog();

        if (result == true)
            return dlg.FileName;

        return null;
    }
}