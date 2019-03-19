using TheVault.Utils;

namespace TheVault.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Instance variables

        private string _originPath;

        private string _destinationPath;

        private string _serverMessage;

        private RelayCommand _openOriginFolder;

        private RelayCommand _openDestinationFolder;

        #endregion //Instance variables

        #region Properties

        public string OriginPath
        {
            get => _originPath;
            set
            {
                if (_originPath == value) return;
                _originPath = value;
                NotifyPropertyChanged("OriginPath");
            }
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                if (_destinationPath == value) return;
                _destinationPath = value;
                NotifyPropertyChanged("DestinationPath");
            }
        }

        public string ServerMessage
        {
            get => _serverMessage;
            set
            {
                if (_serverMessage == value) return;
                _serverMessage = value;
                NotifyPropertyChanged("ServerMessage");
            }
        }

        public RelayCommand OpenOriginFolder
        {
            get => _openOriginFolder;
            set
            {
                if (_openOriginFolder == value) return;
                _openOriginFolder = value;
                NotifyPropertyChanged("ConnectCommand");
            }
        }

        public RelayCommand OpenDestinationFolder
        {
            get => _openDestinationFolder;
            set
            {
                if (_openDestinationFolder == value) return;
                _openDestinationFolder = value;
                NotifyPropertyChanged("OpenDestinationFolder");
            }
        }

        #endregion //Properties

        public MainViewModel()
        {
            //TODO REMOVE
            OriginPath = @"C:\Users\SCHAEFAX\Documents\Perso\Testing\Origin";
            DestinationPath = @"C:\Users\SCHAEFAX\Documents\Perso\Testing\Destination";
            ServerMessage = "Lorem ipsum sit amet bla blabla";
        }
    }
}
