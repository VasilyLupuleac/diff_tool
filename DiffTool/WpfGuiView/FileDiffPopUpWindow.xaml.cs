using System.Windows;

namespace WpfGuiView
{
    /// <summary>
    /// Interaction logic for FileDiffPopUpWindow.xaml
    /// </summary>
    public partial class PopupWindow: Window
    {
        public PopupWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.FileViewModel(new FileDialogService(),
                                                      new FileDialogService(),
                                                      new SaveFileDialogService());
        }
    }
}