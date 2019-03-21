using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
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

        private RelayCommand _openOriginFolderCmd;

        private RelayCommand _openDestinationFolderCmd;

        private RelayCommand _refreshOriginFolderCmd;
        
        private List<FileViewModel> _cryptedFiles;

        private List<FileViewModel> _decryptedFiles;

        private bool _allSelected;

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

        public RelayCommand OpenOriginFolderCmd
        {
            get => _openOriginFolderCmd;
            set
            {
                if (_openOriginFolderCmd == value) return;
                _openOriginFolderCmd = value;
                NotifyPropertyChanged("OpenOriginFolderCmd");
            }
        }

        public RelayCommand OpenDestinationFolderCmd
        {
            get => _openDestinationFolderCmd;
            set
            {
                if (_openDestinationFolderCmd == value) return;
                _openDestinationFolderCmd = value;
                NotifyPropertyChanged("OpenDestinationFolderCmd");
            }
        }

        public RelayCommand RefreshOriginFolderCmd
        {
            get => _refreshOriginFolderCmd;
            set
            {
                if (_refreshOriginFolderCmd == value) return;
                _refreshOriginFolderCmd = value;
                NotifyPropertyChanged("RefreshOriginFolderCmd");
            }
        }
        
        public List<FileViewModel> CryptedFiles
        {
            get => _cryptedFiles;
            set
            {
                if (_cryptedFiles == value) return;
                _cryptedFiles = value;
                NotifyPropertyChanged("CryptedFiles");
            }
        }

        public List<FileViewModel> DecryptedFiles
        {
            get => _decryptedFiles;
            set
            {
                if (_decryptedFiles == value) return;
                _decryptedFiles = value;
                NotifyPropertyChanged("DecryptedFiles");
            }
        }

        public bool AllSelected
        {
            get => _allSelected;
            set
            {
                if (_allSelected == value) return;
                _allSelected = value;
                NotifyPropertyChanged("AllSelected");
                AllSelectedChanged();
            }
        }

        public bool IsItemSelectionChanged { get; set; }

        #endregion //Properties

        #region Constructors

        public MainViewModel()
        {
            DecryptedFiles = new List<FileViewModel>();
            CryptedFiles = new List<FileViewModel>();

            AllSelected = false;
            var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            var basePath = lines[0];
            OriginPath = $"{basePath}\\{lines[1]}";
            DestinationPath = $"{basePath}\\{lines[2]}";
            VaultPath = $"{basePath}\\{lines[3]}";
            PassValue = lines[4];
            SaltValue = lines[5];

            OpenOriginFolderCmd = new RelayCommand(true, _ => OpenOriginFolder());
            OpenDestinationFolderCmd = new RelayCommand(true, _ => OpenDestinationFolder());
            RefreshOriginFolderCmd = new RelayCommand(true, _ => GetOriginFolder(basePath));

            GetOriginFolder(basePath);
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Height = 900;
                Application.Current.MainWindow.Width = 1200;
                Application.Current.MainWindow.Closing += OnClose;
            }

            //TODO REMOVE
            ServerMessage = "Lorem ipsum sit amet bla blabla";
        }

        #endregion //Constructors

        #region Private methods

        private void OnClose(object sender, CancelEventArgs e)
        {
            var originFolder = new DirectoryInfo(OriginPath);
            foreach (var file in originFolder.EnumerateFiles())
                file.Delete();
            foreach (var folder in originFolder.EnumerateDirectories())
                folder.Delete(true);

            var destinationFolder = new DirectoryInfo(DestinationPath);
            foreach (var file in destinationFolder.EnumerateFiles())
                file.Delete();
            foreach (var folder in destinationFolder.EnumerateDirectories())
                folder.Delete(true);
        }
        
        private void OpenOriginFolder()
        {
            if (!string.IsNullOrWhiteSpace(OriginPath) && Directory.Exists(OriginPath))
                Process.Start(OriginPath);
        }

        private void OpenDestinationFolder()
        {
            if (!string.IsNullOrWhiteSpace(DestinationPath) && Directory.Exists(DestinationPath))
                Process.Start(DestinationPath);
        }

        private void AllSelectedChanged()
        {
            if (IsItemSelectionChanged)
            {
                IsItemSelectionChanged = false;
                return;
            }

            if (AllSelected)
                DecryptedFiles.ForEach(f => f.IsSelected = true);
            else
                DecryptedFiles.ForEach(f => f.IsSelected = false);
        }

        private void ItemSelectionChanged()
        {
            if (!AllSelected) return;
            IsItemSelectionChanged = true;
            AllSelected = false;
        }

        private void GetOriginFolder(string basePath)
        {
            var files = new DirectoryInfo(OriginPath).GetFiles("*", SearchOption.AllDirectories);
            DecryptedFiles = new List<FileViewModel>();
            foreach (var file in files)
            {
                var fileViewModel = new FileViewModel(false, file.Name, file.DirectoryName?.Remove(0, basePath.Length));
                fileViewModel.SelectionChanged = new RelayCommand(true, _ => ItemSelectionChanged());   
                DecryptedFiles.Add(fileViewModel);
            }
        }

        #endregion //Private methods
    }
}
