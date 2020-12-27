using System.Windows;
public interface IDialogService
{
    string OpenDialog();
}

public class FileDialogService : IDialogService
{
    public string OpenDialog()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog();

        var result = dlg.ShowDialog();

        if (result == true)
            return dlg.FileName;

        return null;
    }
}

public class FolderDialogService : IDialogService
{
    public string OpenDialog()
    {
        var dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

        var result = dlg.ShowDialog();

        if (result == true)
            return dlg.SelectedPath;

        return null;
    }
}


public class SaveFileDialogService : IDialogService
{
    public string OpenDialog()
    {
        var dlg = new Microsoft.Win32.SaveFileDialog();

        var result = dlg.ShowDialog();

        if (result == true)
            return dlg.FileName;

        return null;
    }
}