using System.Threading.Tasks;
using System.Windows;
using TheVault.Utils;
using TheVault.ViewModels;

namespace TheVault.Views
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl
    {
        public MainUserControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            DataContext = new MainViewModel();
            ((MainViewModel) DataContext).OnEncryptFinished = new RelayCommand(true, param => OnEncryptFinished(param));
        }

        private void OnEncryptFinished(object param)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                (DataContext as MainViewModel).ServerMessage = $"{param as string} files processed - 100%";
                ServerMessageBlock.Style = Application.Current.FindResource("EndServerBlock") as Style;
            });
            Task.Delay(1500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ServerMessageBlock.Style = Application.Current.FindResource("ServerBlock") as Style;
                    (DataContext as MainViewModel).ServerMessage = "";
                });
            });
        }
    }
}
