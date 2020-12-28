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
            prevPar.Click += new RoutedEventHandler(OnPrevClick);
            nextPar.Click += new RoutedEventHandler(OnNextClick);
        }

        private void OpenFileDifference(object sender, RoutedEventArgs e)
        {
            var node = (ViewModel.FileTreeNode)tree.SelectedItem;
            if (node.Type != ViewModel.NodeType.ChangedFile)
                return;
            var diff = node.Diff;
            var save = ((ViewModel.ViewModel)DataContext).FileVM.SaveFileService;
            var fvm = new ViewModel.FileViewModel(diff, null, null, save);

            var popup = new PopupWindow(fvm);
            popup.ShowDialog();
        }

        private void OnPrevClick(object sender, RoutedEventArgs e)
        {
            ShiftParagraphSelection(-1);
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            ShiftParagraphSelection(1);
        }

        private void ShiftParagraphSelection(int shift)
        {
            var vm = (ViewModel.ViewModel)DataContext;
            var fvm = vm.FileVM;
            if (fvm.ChangeBlocksNum == 0)
                return;
            var selectedIndex = fvm.SelectedChangeBlock.ID;
            var paragraphNumber = fvm.ParagraphView.ChangeBlocksPositions[selectedIndex];
            var prevIndex = (selectedIndex - shift + fvm.ChangeBlocksNum) % fvm.ChangeBlocksNum;
            var prevNumber = fvm.ParagraphView.ChangeBlocksPositions[prevIndex];

            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BringIntoView();
            documentViwer.Document.Blocks.ElementAt(prevNumber).BorderThickness = new Thickness(0);
            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BorderThickness = new Thickness(20, 0, 0 ,0);
            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BorderBrush = Brushes.DarkGray;
        }
    }
}
