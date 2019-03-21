using TheVault.Utils;

namespace TheVault.ViewModels
{
    public class FileViewModel : BaseViewModel
    {
        #region Instance variables

        private bool _isSelected;

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
                if (!value) SelectionChanged.Execute(null);

                NotifyPropertyChanged("IsSelected");
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

        #endregion //Properties

        #region Constructor

        public FileViewModel(bool isSelected, string fileName, string path)
        {
            IsSelected = isSelected;
            FileName = fileName;
            Path = path;
        }

        #endregion //Constructors
    }
}
