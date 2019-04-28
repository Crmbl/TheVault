using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TheVault.Objects;
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

        public void Init(List<FolderObject> mapping)
        {
            var viewModel = new MainViewModel(mapping);
            DataContext = viewModel;
            
            StartServer.Style = FindResource("StartServer") as Style;
            viewModel.OnEncryptFinished = new RelayCommand(true, OnEncryptFinished);
            viewModel.OnEncryptListChanged = new RelayCommand(true, _ => OnEncryptListChanged());
            viewModel.OnDecryptListChanged = new RelayCommand(true, _ => OnDecryptListChanged());
            viewModel.OpenDialogCmd = new RelayCommand(true, _ => OnOpenDialog());
            viewModel.OpenDialogDeviceCmd = new RelayCommand(true, _ => OnOpenDialogDevice());
            viewModel.DecryptListItemChangedCmd = new RelayCommand(true, viewModel.SortDecryptButtonChanged);
            viewModel.EncryptListItemChangedCmd = new RelayCommand(true, viewModel.SortEncryptButtonChanged);
            viewModel.CloseDialogCmd = new RelayCommand(true, param =>
            {
                Dialog.IsOpen = false;
                viewModel.OnCloseDialog(param);
            });
            viewModel.CloseDialogDeviceCmd = new RelayCommand(true, param =>
            {
                DialogDevice.IsOpen = false;
                viewModel.OnCloseDialogDevice(param);
            });
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

        private void OnOpenDialog()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!Dialog.IsOpen)
                    Dialog.IsOpen = true;
            });
        }
        
        private void OnOpenDialogDevice()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!DialogDevice.IsOpen)
                    DialogDevice.IsOpen = true;
            });
        }

        private void OnDecryptedListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var item = listBox?.SelectedItems.Count > 0 ? listBox.SelectedItems[0] : null;
            (DataContext as MainViewModel)?.DecryptListItemChangedCmd.Execute(item);
        }
            
        private void OnEncryptedListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var item = listBox?.SelectedItems.Count > 0 ? listBox.SelectedItems[0] : null;
            (DataContext as MainViewModel)?.EncryptListItemChangedCmd.Execute(item);
        }

        private void StartServer_OnClick(object sender, RoutedEventArgs e)
        {
            if ((StartServer.Content as string) == "KILL SERVER")
                StartServer.Style = FindResource("StartServer") as Style;
            else
                StartServer.Style = FindResource("KillServer") as Style;
        }
    }
}
