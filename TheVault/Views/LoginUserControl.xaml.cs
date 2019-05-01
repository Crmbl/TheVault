using System.Security.Cryptography;
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

            #if DEBUG
            //Get extensions from files
            var lines = System.IO.File.ReadAllLines($"{System.Environment.CurrentDirectory}\\settings");
            var basePath = lines[0].EndsWith("\\") ? lines[0] : $"{lines[0]}\\";
            var originPath = $"{basePath}{lines[1]}";
            FileUtil.GetFileExtensions(new System.IO.DirectoryInfo(originPath), $"{basePath}extensions.txt");
            #endif
        }

        private void Connect()
        {
            if (!(DataContext is LoginViewModel viewModel)) return;

            var password = PasswordBox.Password;
            var hashPass = "";
            var hashUser = "";
            using (var md5Hash = MD5.Create())
            {
                hashPass = MD5Util.GetMd5Hash(md5Hash, password);
                hashUser = MD5Util.GetMd5Hash(md5Hash, viewModel.Username);
            }

            if (string.IsNullOrWhiteSpace(viewModel.Username) || string.IsNullOrWhiteSpace(password)
                || hashPass != viewModel.PassCheck.ToLower() || hashUser != viewModel.UserCheck.ToLower())
            {
                ConnectButton.Style = Application.Current.Resources["ErrorButton"] as Style;
                viewModel.SetErrorConnectString();
                Task.Delay(1000).ContinueWith(_ =>
                {
                    ConnectButton.Dispatcher.Invoke(() => ConnectButton.Style = Application.Current.FindResource("ConnectButton") as Style);
                    viewModel.SetDefaultConnectString();
                });
            }
            else
            {
                PasswordBox.Password = "";
                viewModel.Username = "";
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
