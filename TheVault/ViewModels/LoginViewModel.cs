using TheVault.Utils;

namespace TheVault.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Instance Variables

        private string _username;

        private string _connectButtonContent;

        private RelayCommand _connectCommand;

        #endregion //Instance Variables

        #region Properties

        public string Username
        {
            get => _username;
            set
            {
                if (_username == value) return;
                _username = value;
                NotifyPropertyChanged("Username");
            }
        }

        public string ConnectButtonContent
        {
            get => _connectButtonContent;
            set
            {
                if (_connectButtonContent == value) return;
                _connectButtonContent = value;
                NotifyPropertyChanged("ConnectButtonContent");
            }
        }

        public RelayCommand ConnectCommand
        {
            get => _connectCommand;
            set
            {
                if (_connectCommand == value) return;
                _connectCommand = value;
                NotifyPropertyChanged("ConnectCommand");
            }
        }

        #endregion //Properties

        /// <summary>
        /// Default constructor of viewModel.
        /// </summary>
        public LoginViewModel()
        {
            SetDefaultConnectString();
            //TODO REMOVE (and remove Password value in xaml)
            Username = "TEST";
        }

        #region Methods

        public void SetDefaultConnectString()
        {
            this.ConnectButtonContent = "CONNECT";
        }

        public void SetErrorConnectString()
        {
            this.ConnectButtonContent = "ERROR";
        }

        #endregion //Methods
    }
}
