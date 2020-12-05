using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using Model;
using System;

namespace WpfGuiView.ViewModel
{
    public class DirViewModel : INotifyPropertyChanged
    {
        private DirectoryDifferenceModel _diff;
        private readonly IDialogService _selectDir1;
        private readonly IDialogService _selectDir2;
        public ObservableCollection<FileTreeNode> Node;


        private string _path1;
        private string _path2;

        public string Path1
        {
            get
            {
                return _path1 ?? "Select folder";
            }
            set
            {
                _path1 = value;
                OnPropertyChanged("Path1");
            }
        }

        public string Path2
        {
            get
            {
                return _path2 ?? "Select folder";
            }
            set
            {
                _path2 = value;
                OnPropertyChanged("Path2");
            }
        }

        public IEnumerable<string> AddedFiles => _diff.AddedFiles;
        public IEnumerable<string> DeletedFiles => _diff.DeletedFiles;
        public IEnumerable<string> AddedDirectories => _diff.AddedDirectories;
        public IEnumerable<string> DeletedDirectories => _diff.DeletedDirectories;
        public List<string> ChangedFiles => new List<string>(_diff.ChangedFiles.Select(x => x.Path1));
        public ICommand OpenFolderDialog1Command => new RelayCommand(_ => { Path1 = _selectDir1.OpenDialog(); });
        public string Name => _diff.Name;

        public ICommand OpenFolderDialog2Command => new RelayCommand(_ => { Path2 = _selectDir2.OpenDialog(); });
        public ICommand CalculateDifferenceCommand => new RelayCommand(_ =>
        {
            if (_path1 is null || _path2 is null)
                return;
            _diff = new DirectoryDifferenceModel(Path1, Path2);
            Node = new ObservableCollection<FileTreeNode>();
            Node.Add(new FileTreeNode(_diff, null));
            OnPropertyChanged("ChangedFiles");
            OnPropertyChanged("ChangedDirectories");
            OnPropertyChanged("AddedFiles");
            OnPropertyChanged("DeletedFiles");
            OnPropertyChanged("AddedDirectories");
            OnPropertyChanged("DeletedDirectories");
        });
        public DirViewModel(IDialogService dialogService1, IDialogService dialogService2)
        {
            _path1 = null;
            _path2 = null;
            _selectDir1 = dialogService1;
            _selectDir2 = dialogService2;
            _diff = new DirectoryDifferenceModel();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
