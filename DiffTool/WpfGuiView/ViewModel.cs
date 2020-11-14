using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Model;
using System;

namespace WpfGuiView.ViewModel
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private bool _showFiles;
        private FileDifferenceModel _diff;
        private int _selectedIndex;
        IDialogService _selectFile1;
        IDialogService _selectFile2;
        

        public string Path1 
        {
            get
            {
                if (_diff.Path1 is null)
                    return "Select file";
                return _diff.Path1;
            }
            set 
            { 
                _diff.Path1 = value;
                OnPropertyChanged("Path1");
            }
        }

        public string Path2
        {
            get
            {
                if (_diff.Path2 is null)
                    return "Select file";
                return _diff.Path2;
            }
            set
            {
                _diff.Path2 = value;
                OnPropertyChanged("Path2");
            }
        }

        public ICommand OpenFileDialog1Command
        {
            get { return new RelayCommand(o => { Path1 = _selectFile1.OpenFileDialog(); }); }
        }

        public ICommand OpenFileDialog2Command
        {
            get { return new RelayCommand(o => { Path2 = _selectFile2.OpenFileDialog(); }); }
        }
        public ApplicationViewModel(IDialogService dialogService1, IDialogService dialogService2)
        {
            _selectFile1 = dialogService1;
            _selectFile2 = dialogService2;
            _diff = new FileDifferenceModel();
        }
        public void SelectFile1()
        {
            Path1 = _selectFile1.OpenFileDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
