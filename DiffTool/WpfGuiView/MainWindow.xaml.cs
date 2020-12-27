using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfGuiView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.ViewModel(new FileDialogService(),
                                                  new FileDialogService(),
                                                  new FolderDialogService(),
                                                  new FolderDialogService(),
                                                  new SaveFileDialogService());
            prevPar.Click += new RoutedEventHandler(BringSelectedParagraphIntoView);
            nextPar.Click += new RoutedEventHandler(BringSelectedParagraphIntoView);
        }

        private void BringSelectedParagraphIntoView(object sender, RoutedEventArgs e)
        {
            var vm = (ViewModel.ViewModel)mainWindow.DataContext;
            var fvm = vm.FileVM;
            var selectedIndex = fvm.SelectedChangeBlock.ID;
            var paragraphNumber = fvm.ParagraphView.ChangeBlocksPositions[selectedIndex];
            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BringIntoView();
        }
    }
}
