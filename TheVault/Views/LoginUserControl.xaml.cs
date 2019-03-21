using System.Threading.Tasks;
using System.Windows;
using TheVault.Utils;
using TheVault.ViewModels;

namespace TheVault.Views
{
    /// <summary>
    /// Interaction logic for LoginUserControl.xaml
    /// </summary>
    public partial class LoginUserControl
    {
        public LoginUserControl()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();

            var viewModel = (LoginViewModel) DataContext;
            viewModel.ConnectCommand = new RelayCommand(true, _ => Connect());
        }

        //TODO improve login checkup
        private void Connect()
        {
            if (!(DataContext is LoginViewModel viewModel)) return;

            var password = PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(viewModel.Username) || string.IsNullOrWhiteSpace(password)) // FAIL
            {
                ConnectButton.Style = Application.Current.Resources["ErrorButton"] as Style;
                viewModel.SetErrorConnectString();
                Task.Delay(1000).ContinueWith(_ =>
                {
                    ConnectButton.Dispatcher.Invoke(() => ConnectButton.Style = Application.Current.FindResource("ConnectButton") as Style);
                    viewModel.SetDefaultConnectString();
                });
            }
            else // SUCCESS
            {
                PasswordBox.Password = "";
                viewModel.Username = "";
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
