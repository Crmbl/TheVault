using System.Threading.Tasks;
using System.Windows;
using TheVault.Utils;
using TheVault.ViewModels;

namespace TheVault.Views
{
    /// <summary>
    /// Interaction logic for LoadingUserControl.xaml
    /// </summary>
    public partial class LoadingUserControl
    {
        public LoadingUserControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            DataContext = new LoadingViewModel();
            
            var viewModel = (LoadingViewModel) DataContext;
            viewModel.DecryptFinished = new RelayCommand(true, o => OnDecryptFinished());
            viewModel.DecryptFilesAsync();
        }

        private void OnDecryptFinished()
        {
            Task.Delay(1500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(
                    () => { Visibility = Visibility.Collapsed; });
            });
        }
    }
}
