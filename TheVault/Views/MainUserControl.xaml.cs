using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            DataContext = new MainViewModel();

            var viewModel = (MainViewModel)DataContext;
            viewModel.OpenOriginFolder = new RelayCommand(true, c => OpenOriginFolder());
            viewModel.OpenDestinationFolder = new RelayCommand(true, c => OpenDestinationFolder());

            //if (!File.Exists($"{Environment.CurrentDirectory}\\settings"))
            //    throw new Exception("ERROR ERROR ERROR ALERT ALERT!!!");
            //TODO add button to set the folders (in settings file) and refresh the viewModel.Lists
            //var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            //viewModel.OriginPath = lines.First();
            //viewModel.DestinationPath = lines[1];
            //viewModel.VaultPath = lines[2];
            //viewModel.PassValue = lines[3];
            //viewModel.SaltValue = lines.Last();
            
            ////TODO need decryptloading page while decrypting all files in Vault folder to Origin folder

            //var files = new DirectoryInfo(viewModel.OriginPath).GetFiles("*", SearchOption.AllDirectories);
            //foreach(var file in files) //TODO REMOVE HARDCODED VALUE
            //    viewModel.DecryptedFiles.Add(new VaultFile(false, file.Name, file.DirectoryName.Remove(0, 25)));
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
