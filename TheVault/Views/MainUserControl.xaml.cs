using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TheVault.Utils;
using TheVault.ViewModels;

namespace TheVault.Views
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        public MainUserControl()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            var viewModel = (MainViewModel) DataContext;
            viewModel.OpenOriginFolder = new RelayCommand(true, c => OpenOriginFolder());
            viewModel.OpenDestinationFolder = new RelayCommand(true, c => OpenDestinationFolder());
        }

        //private void StartServer(object sender, RoutedEventArgs e)
        //{
        //    AsynchronousSocketListener.StartListening();
        //}

        #region Methods

        private void OpenOriginFolder()
        {
            var viewModel = (MainViewModel) DataContext;
            if (!string.IsNullOrWhiteSpace(viewModel.OriginPath))
                Process.Start(viewModel.OriginPath);
        }

        private void OpenDestinationFolder()
        {
            var viewModel = (MainViewModel) DataContext;
            if (!string.IsNullOrWhiteSpace(viewModel.DestinationPath))
                Process.Start(viewModel.DestinationPath);
        }

        #endregion //Methods
    }
}
