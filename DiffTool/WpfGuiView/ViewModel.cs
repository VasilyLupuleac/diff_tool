
namespace WpfGuiView.ViewModel
{
    public class ViewModel
    {
        public FileViewModel FileVM { get; private set; }
        public DirViewModel DirVM { get; private set; }

        public ViewModel(IDialogService fileService1,
                         IDialogService fileService2,
                         IDialogService dirService1,
                         IDialogService dirService2,
                         IDialogService saveFileService)
        {
            FileVM = new FileViewModel(fileService1, fileService2, saveFileService);
            DirVM = new DirViewModel(dirService1, dirService2);
        }
    }
}