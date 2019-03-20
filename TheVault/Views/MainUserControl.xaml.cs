using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TheVault.Objects;
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
        }

        public void Init()
        {
            Application.Current.MainWindow.Height = 900;
            Application.Current.MainWindow.Width = 1200;

            DataContext = new MainViewModel();

            var viewModel = (MainViewModel)DataContext;
            viewModel.OpenOriginFolder = new RelayCommand(true, c => OpenOriginFolder());
            viewModel.OpenDestinationFolder = new RelayCommand(true, c => OpenDestinationFolder());

            //TODO add button to set the folders (in settings file) and refresh the viewModel.Lists
            var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            viewModel.OriginPath = $"{lines[0]}\\{lines[1]}";
            viewModel.DestinationPath = $"{lines[0]}\\{lines[2]}";
            viewModel.VaultPath = $"{lines[0]}\\{lines[3]}";
            viewModel.PassValue = lines[4];
            viewModel.SaltValue = lines[5];

            var files = new DirectoryInfo(viewModel.OriginPath).GetFiles("*", SearchOption.AllDirectories);
            foreach(var file in files)
                viewModel.DecryptedFiles.Add(new VaultFile(false, file.Name, file.DirectoryName.Remove(0, lines[0].Length)));
        }

        #region Methods

        private void OpenOriginFolder()
        {
            var viewModel = (MainViewModel) DataContext;
            if (!string.IsNullOrWhiteSpace(viewModel.OriginPath) && Directory.Exists(viewModel.OriginPath))
                Process.Start(viewModel.OriginPath);
        }

        private void OpenDestinationFolder()
        {
            var viewModel = (MainViewModel) DataContext;
            if (!string.IsNullOrWhiteSpace(viewModel.DestinationPath) && Directory.Exists(viewModel.DestinationPath))
                Process.Start(viewModel.DestinationPath);
        }

        #endregion //Methods
    }
}
