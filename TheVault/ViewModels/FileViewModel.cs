using TheVault.Utils;

namespace TheVault.ViewModels
{
    public class FileViewModel : BaseViewModel
    {
        #region Instance variables

        private bool _isSelected;
        
        private bool _isEnabled;

        private RelayCommand _selectionChanged;

        #endregion //Instance variables

        #region Properties

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                SelectionChanged.Execute(null);
                NotifyPropertyChanged("IsSelected");
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                NotifyPropertyChanged("IsEnabled");
            }
        }

        public RelayCommand SelectionChanged
        {
            get => _selectionChanged;
            set
            {
                if (_selectionChanged == value) return;
                _selectionChanged = value;
                NotifyPropertyChanged("SelectionChanged");
            }
        }

        public string FileName { get; set; }

        public string Path { get; set; }

        public string SizeMb { get; set; }

        #endregion //Properties

        #region Constructor

        public FileViewModel(bool isEnabled, string fileName, string path)
        {
            IsSelected = false;
            IsEnabled = isEnabled;
            FileName = fileName;
            Path = path;
        }

        public FileViewModel(string fileName, string size)
        {
            FileName = fileName;
            SizeMb = size;
        }

        #endregion //Constructors
    }
}
