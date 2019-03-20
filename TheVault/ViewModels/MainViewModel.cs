using System.Collections.Generic;
using TheVault.Objects;
using TheVault.Utils;

namespace TheVault.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Instance variables

        private string _originPath;

        private string _destinationPath;

        private string _vaultPath;

        private string _serverMessage;

        private string _passValue;

        private string _saltValue;

        private RelayCommand _openOriginFolder;

        private RelayCommand _openDestinationFolder;

        private List<VaultFile> _cryptedFiles;

        private List<VaultFile> _decryptedFiles;

        private bool _decryptAll;

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

        public string VaultPath
        {
            get => _vaultPath;
            set
            {
                if (_vaultPath == value) return;
                _vaultPath = value;
                NotifyPropertyChanged("VaultPath");
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

        public string SaltValue
        {
            get => _saltValue;
            set
            {
                if (_saltValue == value) return;
                _saltValue = value;
                NotifyPropertyChanged("SaltValue");
            }
        }

        public string PassValue
        {
            get => _passValue;
            set
            {
                if (_passValue == value) return;
                _passValue = value;
                NotifyPropertyChanged("PassValue");
            }
        }

        public RelayCommand OpenOriginFolder
        {
            get => _openOriginFolder;
            set
            {
                if (_openOriginFolder == value) return;
                _openOriginFolder = value;
                NotifyPropertyChanged("OpenOriginFolder");
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

        public List<VaultFile> CryptedFiles
        {
            get => _cryptedFiles;
            set
            {
                if (_cryptedFiles == value) return;
                _cryptedFiles = value;
                NotifyPropertyChanged("CryptedFiles");
            }
        }

        public List<VaultFile> DecryptedFiles
        {
            get => _decryptedFiles;
            set
            {
                if (_decryptedFiles == value) return;
                _decryptedFiles = value;
                NotifyPropertyChanged("DecryptedFiles");
            }
        }

        public bool DecryptedAll
        {
            get => _decryptAll;
            set
            {
                if (_decryptAll == value) return;
                _decryptAll = value;
                NotifyPropertyChanged("DecryptedAll");
            }
        }

        #endregion //Properties

        public MainViewModel()
        {
            //TODO REMOVE
            ServerMessage = "Lorem ipsum sit amet bla blabla";
            DecryptedFiles = new List<VaultFile>();
            CryptedFiles = new List<VaultFile>();
        }
    }
}
