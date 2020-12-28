using System.Windows;
using System.Linq;
using System.Windows.Media;

namespace WpfGuiView
{
    /// <summary>
    /// Interaction logic for FileDiffPopUpWindow.xaml
    /// </summary>
    public partial class PopupWindow: Window
    {
        public PopupWindow(ViewModel.FileViewModel fvm)
        {
            InitializeComponent();
            DataContext = fvm;
            prevPar.Click += new RoutedEventHandler(OnPrevClick);
            nextPar.Click += new RoutedEventHandler(OnNextClick);
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
            var fvm = (ViewModel.FileViewModel)DataContext;
            if (fvm.ChangeBlocksNum == 0)
                return;
            var selectedIndex = fvm.SelectedChangeBlock.ID;
            var paragraphNumber = fvm.ParagraphView.ChangeBlocksPositions[selectedIndex];
            var prevIndex = (selectedIndex - shift + fvm.ChangeBlocksNum) % fvm.ChangeBlocksNum;
            var prevNumber = fvm.ParagraphView.ChangeBlocksPositions[prevIndex];

            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BringIntoView();
            documentViwer.Document.Blocks.ElementAt(prevNumber).BorderThickness = new Thickness(0);
            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BorderThickness = new Thickness(20, 0, 0, 0);
            documentViwer.Document.Blocks.ElementAt(paragraphNumber).BorderBrush = Brushes.DarkGray;
        }
    }
}