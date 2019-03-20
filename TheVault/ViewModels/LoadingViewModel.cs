namespace TheVault.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        #region Instance variables

        private int _filesToTreat;

        private string _message;

        private int _progressValue;

        #endregion //Instance variables

        #region Properties

        public int FilesToTreat
        {
            get => _filesToTreat;
            set
            {
                if (_filesToTreat == value) return;
                _filesToTreat = value;
                NotifyPropertyChanged("FilesToTreat");
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value) return;
                _progressValue = value;
                NotifyPropertyChanged("ProgressValue");
            }
        }

        #endregion //Properties
    }
}
