using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            var viewModel = new MainViewModel();
            DataContext = viewModel;
            
            viewModel.OnEncryptFinished = new RelayCommand(true, OnEncryptFinished);
            viewModel.OnEncryptListChanged = new RelayCommand(true, _ => OnEncryptListChanged());
            viewModel.OnDecryptListChanged = new RelayCommand(true, _ => OnDecryptListChanged());
        }

        private void OnEncryptFinished(object param)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainViewModel)DataContext).ServerMessage = $"{param as string} files processed - 100%";
                ServerMessageBlock.Style = Application.Current.FindResource("EndServerBlock") as Style;
            });
            Task.Delay(1500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ServerMessageBlock.Style = Application.Current.FindResource("ServerBlock") as Style;
                    ((MainViewModel)DataContext).ServerMessage = "";
                    ((MainViewModel)DataContext).ShowProgressBar = false;
                });
            });
        }

        private void OnDecryptListChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!(DecryptedListView.View is GridView gridView)) 
                    return;
            
                foreach (var column in gridView.Columns)
                {
                    if (double.IsNaN(column.Width))
                        column.Width = column.ActualWidth;

                    column.Width = double.NaN;
                }
            });
        }

        private void OnEncryptListChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!(EncryptedListView.View is GridView gridView)) 
                    return;
            
                foreach (var column in gridView.Columns)
                {
                    if (double.IsNaN(column.Width))
                        column.Width = column.ActualWidth;

                    column.Width = double.NaN;
                }
            });
        }
    }
}
