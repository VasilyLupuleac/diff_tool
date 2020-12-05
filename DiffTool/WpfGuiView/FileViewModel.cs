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
        private bool _showFiles;
        private FileDifferenceModel _diff;
        private int _selectedIndex;
        private readonly IDialogService _selectFile1;
        private readonly IDialogService _selectFile2;
        

        private string _path1;
        private string _path2;

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
                var list = new List<ChangeBlockView>();
                if (_diff is null)
                    return list;
                for (int i = 0; i < _diff.ChangeBlocks.Count(); i++)
                    list.Add(new ChangeBlockView(_diff.ChangeBlocks[i], i));
                return list;
            }
        }

        public ChangeBlockView SelectedChangeBlock
        {
            get
            {
                if (_diff is null)
                    return null;
                try {
                    return ChangeBlocks[_selectedIndex];
                }
                catch(ArgumentOutOfRangeException _)
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

        public ICommand NextCommand => new RelayCommand(_ => 
        {
            if (_diff is null || ChangeBlocks.Count() == 0)
                return;
             _selectedIndex = (_selectedIndex + 1) % _diff.ChangeBlocks.Count;
            OnPropertyChanged("SelectedChangeBlock"); 
         });

        public ICommand PrevCommand => new RelayCommand(_ =>
        {
            if (_diff is null || ChangeBlocks.Count() == 0)
                return;
            if (_selectedIndex == 0)
                _selectedIndex = _diff.ChangeBlocks.Count;
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
        public FileViewModel(IDialogService dialogService1, IDialogService dialogService2)
        {
            _path1 = null;
            _path2 = null;
            _selectFile1 = dialogService1;
            _selectFile2 = dialogService2;
            _selectedIndex = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
