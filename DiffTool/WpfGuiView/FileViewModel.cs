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
    public class FileViewModel : INotifyPropertyChanged
    {
        private FileDifferenceModel _diff;
        private int _selectedIndex;
        private int _changeBlocksNum;
        private List<ChangeBlockView> _changeBlocks;

        private readonly IDialogService _selectFile1;
        private readonly IDialogService _selectFile2;
        private readonly IDialogService _saveFile;

        public IDialogService SaveFileService => _saveFile;


        private string _path1;
        private string _path2;


        private FileParagraphView _paragraphView;
        public FileParagraphView ParagraphView => _paragraphView;

        public int ChangeBlocksNum => _changeBlocksNum;
        public string Path1
        {
            get
            {
                return _path1 ?? "Select file";
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
                return _path2 ?? "Select file";
            }
            set
            {
                _path2 = value;
                OnPropertyChanged("Path2");
            }
        }
        public List<ChangeBlockView> ChangeBlocks
        {
            get
            {
                if (!(_changeBlocks is null))
                    return _changeBlocks;
                var list = new List<ChangeBlockView>();
                if (_diff is null)
                    return list;
                for (int i = 0; i < _changeBlocksNum; i++)
                    list.Add(new ChangeBlockView(_diff.ChangeBlocks[i], i));
                _changeBlocks =  list;
                return _changeBlocks;
            }
        }

        public ChangeBlockView SelectedChangeBlock
        {
            get
            {
                if (_diff is null)
                    return null;
                try
                {
                    return ChangeBlocks[_selectedIndex];
                }
                catch (ArgumentOutOfRangeException _)
                {
                    return null;
                }

            }
            set
            {
                if (value is null)
                {
                    _selectedIndex = 0;
                    return;
                }
                _selectedIndex = value.ID;
                OnPropertyChanged("SelectedChangeBlock");
            }
        }
        public ICommand OpenFileDialog1Command => new RelayCommand(_ => { Path1 = _selectFile1.OpenDialog(); });

        public ICommand OpenFileDialog2Command => new RelayCommand(_ => { Path2 = _selectFile2.OpenDialog(); });

        public ICommand SaveFileDialogCommand => new RelayCommand(_ => { _diff.Save(_saveFile.OpenDialog()); });

        public ICommand NextCommand => new RelayCommand(_ =>
        {
            if (_diff is null || ChangeBlocks.Count() == 0)
                return;
            _selectedIndex = (_selectedIndex + 1) % _changeBlocksNum;
            OnPropertyChanged("SelectedChangeBlock");
        });

        public ICommand PrevCommand => new RelayCommand(_ =>
        {
            if (_diff is null || ChangeBlocks.Count() == 0)
                return;
            if (_selectedIndex == 0)
                _selectedIndex = _changeBlocksNum;
            _selectedIndex--;
            OnPropertyChanged("SelectedChangeBlock");
        });

        public ICommand CalculateDifferenceCommand => new RelayCommand(_ =>
        {
            if (_path1 is null || _path2 is null)
                return;
            _diff = new FileDifferenceModel(Path1, Path2);
            OnPropertyChanged("ChangeBlocks");
            OnPropertyChanged("SelectedChangeBlock");
        });

        public ICommand CalculateDifferenceAndParagraphsCommand => new RelayCommand(_ =>
        {
            if (_path1 is null || _path2 is null)
                return;
            _diff = new FileDifferenceModel(Path1, Path2);
            _paragraphView = new FileParagraphView(_diff);
            _changeBlocks = null;
            _changeBlocksNum = _diff.ChangeBlocks.Count;
            OnPropertyChanged("ChangeBlocks");
            OnPropertyChanged("SelectedChangeBlock");
            OnPropertyChanged("ParagraphView");
        });

        public FileViewModel(IDialogService dialogService1, IDialogService dialogService2, IDialogService saveDialogService)
        {
            _path1 = null;
            _path2 = null;
            _selectFile1 = dialogService1;
            _selectFile2 = dialogService2;
            _saveFile = saveDialogService;
            _selectedIndex = 0;
            _changeBlocksNum = 0;
            _changeBlocks = new List<ChangeBlockView>();
        }

        public FileViewModel(FileDifferenceModel diff, IDialogService dialogService1, IDialogService dialogService2, IDialogService saveDialogService)
        {
            _path1 = diff.Path2;
            _path2 = diff.Path1;
            _diff = diff;
            _selectFile1 = dialogService1;
            _selectFile2 = dialogService2;
            _saveFile = saveDialogService;
            _selectedIndex = 0;
            _changeBlocksNum = diff.ChangeBlocks.Count;
            _changeBlocks = null;
            _paragraphView = new FileParagraphView(_diff);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}