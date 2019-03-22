using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MediaToolkit;
using MediaToolkit.Model;
using Newtonsoft.Json;
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

        private RelayCommand _openOriginFolderCmd;

        private RelayCommand _openDestinationFolderCmd;

        private RelayCommand _refreshOriginFolderCmd;

        private RelayCommand _encryptCmd;

        private RelayCommand _onEncryptFinished;

        private List<FileViewModel> _encryptedFiles;

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

        public RelayCommand OnEncryptFinished
        {
            get => _onEncryptFinished;
            set
            {
                if (_onEncryptFinished == value) return;
                _onEncryptFinished = value;
                NotifyPropertyChanged("OnEncryptFinished");
            }
        }

        public RelayCommand EncryptCmd
        {
            get => _encryptCmd;
            set
            {
                if (_encryptCmd == value) return;
                _encryptCmd = value;
                NotifyPropertyChanged("EncryptCmd");
            }
        }

        public List<FileViewModel> EncryptedFiles
        {
            get => _encryptedFiles;
            set
            {
                if (_encryptedFiles == value) return;
                _encryptedFiles = value;
                NotifyPropertyChanged("EncryptedFiles");
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

        public bool OriginFolderEmpty => !DecryptedFiles.Any();

        public bool DestinationFolderEmpty => !EncryptedFiles.Any();

        public bool IsItemSelectionChanged { get; set; }
        public string BasePath { get; set; }
        public FileSystemWatcher DestinationWatcher { get; set; }
        public FileSystemWatcher OriginWatcher { get; set; }

        #endregion //Properties

        #region Constructors

        public MainViewModel()
        {
            DecryptedFiles = new List<FileViewModel>();
            EncryptedFiles = new List<FileViewModel>();

            AllSelected = false;
            var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            BasePath = lines[0].EndsWith("\\") ? lines[0] : $"{lines[0]}\\";
            OriginPath = $"{BasePath}{lines[1]}";
            DestinationPath = $"{BasePath}{lines[2]}";
            VaultPath = $"{BasePath}{lines[3]}";
            PassValue = lines[4];
            SaltValue = lines[5];

            OpenOriginFolderCmd = new RelayCommand(true, _ => OpenOriginFolder());
            OpenDestinationFolderCmd = new RelayCommand(true, _ => OpenDestinationFolder());
            RefreshOriginFolderCmd = new RelayCommand(true, _ =>
            {
                GetOriginFolder();
                GetDestinationFolder();
                AllSelected = false;
            });
            EncryptCmd = new RelayCommand(true, _ => EncryptSelected());

            GetOriginFolder();
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Height = 900;
                Application.Current.MainWindow.Width = 1200;
                Application.Current.MainWindow.Closing += OnClose;
            }

            OriginWatcher = new FileSystemWatcher
            {
                Path = OriginPath,
                Filter = "*",
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                            | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            OriginWatcher.Changed += OnChanged;
            OriginWatcher.Created += OnChanged;
            OriginWatcher.Deleted += OnChanged;
            OriginWatcher.Renamed += OnRenamed;
            OriginWatcher.EnableRaisingEvents = true;
            
            DestinationWatcher = new FileSystemWatcher
            {
                Path = DestinationPath,
                Filter = "*",
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            DestinationWatcher.Changed += OnChanged;
            DestinationWatcher.Created += OnChanged;
            DestinationWatcher.Deleted += OnChanged;
            DestinationWatcher.Renamed += OnRenamed;
            DestinationWatcher.EnableRaisingEvents = true;
        }

        #endregion //Constructors

        #region Events

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.FullPath.Contains(OriginPath))
                GetOriginFolder();
            else if (e.FullPath.Contains(DestinationPath))
                GetDestinationFolder();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (e.FullPath.Contains(OriginPath))
                GetOriginFolder();
            else if (e.FullPath.Contains(DestinationPath))
                GetDestinationFolder();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            DestinationWatcher.EnableRaisingEvents = false;
            DestinationWatcher.Dispose();
            OriginWatcher.EnableRaisingEvents = false;
            OriginWatcher.Dispose();

            // TODO Get all the files from destinationFolder and cut/paste them in Vault folder
            // TODO Make difference from Vault and Origin and encrypt the missing ones
            // TODO Delete everything from Origin and Destination Folders

            //var originFolder = new DirectoryInfo(OriginPath);
            //foreach (var file in originFolder.EnumerateFiles())
            //    file.Delete();
            //foreach (var folder in originFolder.EnumerateDirectories())
            //    folder.Delete(true);

            var destinationFolder = new DirectoryInfo(DestinationPath);
            foreach (var file in destinationFolder.EnumerateFiles())
                file.Delete();
            foreach (var folder in destinationFolder.EnumerateDirectories())
                folder.Delete(true);
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

        #endregion //Events

        #region Private methods

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

        private void GetOriginFolder()
        {
            var files = new DirectoryInfo(OriginPath).GetFiles("*", SearchOption.AllDirectories);
            DecryptedFiles = new List<FileViewModel>();
            foreach (var file in files)
            {
                var fileViewModel = new FileViewModel(false, file.Name, file.DirectoryName?.Remove(0, BasePath.Length));
                fileViewModel.SelectionChanged = new RelayCommand(true, _ => ItemSelectionChanged());   
                DecryptedFiles.Add(fileViewModel);
            }

            NotifyPropertyChanged("OriginFolderEmpty");
        }

        //TODO Columns won't have the good width, always too small :(
        private void GetDestinationFolder()
        {
            var files = new DirectoryInfo(DestinationPath).GetFiles("*", SearchOption.AllDirectories);
            EncryptedFiles = new List<FileViewModel>();
            foreach (var file in files)
            {
                var fileSize = file.Length / 1024 > 1024 ? $"{file.Length / 1024 / 1024} Mo" : $"{file.Length / 1024} Ko";
                var fileViewModel = new FileViewModel(file.Name, fileSize);
                EncryptedFiles.Add(fileViewModel);
            }

            NotifyPropertyChanged("DestinationFolderEmpty");

            //TODO put this in event in MainUserControl ! should work for both listview
            //foreach (var column in EncryptedListView.)
            //{
            //    if (double.IsNaN(column.Width))
            //    {
            //        column.Width = column.ActualWidth;
            //    }

            //    column.Width = double.NaN;
            //}
        }

        #endregion //Private methods

        #region Async methods

        private async void EncryptSelected()
        {
            await Task.Run(() =>
            {
                var mapping = new List<FolderObject>();
                foreach (var file in DecryptedFiles.Where(f => f.IsSelected))
                {
                    var filePath = $"{file.Path.Remove(0, OriginPath.Length - BasePath.Length)}";
                    var fBytes = File.ReadAllBytes($"{OriginPath}{filePath}\\{file.FileName}");
                    var encryptedFile = EncryptionUtil.EncryptBytes(fBytes, PassValue, SaltValue);
                    var ext = file.FileName.Split('.').Last().Insert(0, ".");
                    var fileName = file.FileName;

                    var i = 0;
                    while (File.Exists($"{DestinationPath}\\{EncryptionUtil.Encipher(fileName, 10)}"))
                        fileName = fileName.Replace($"{(i == 0 ? string.Empty : i.ToString())}{ext}", $"{++i}{ext}");

                    var cipheredName = EncryptionUtil.Encipher(fileName, 10);
                    File.WriteAllBytes($"{DestinationPath}\\{cipheredName}", encryptedFile);
                    ServerMessage = file.FileName;

                    var fName = file.Path.Contains("\\") ? file.Path.Split('\\').Last() : file.Path;
                    var folderObject = mapping.FirstOrDefault(f => f.Name == fName);
                    if (folderObject == null)
                    {
                        folderObject = new FolderObject(fName, file.Path);
                        mapping.Add(folderObject);
                    }
                    
                    folderObject.Files.Add(new FileObject
                    {
                        OriginName = file.FileName,
                        UpdatedName = fileName
                    });
                }

                GetMediaSize(mapping);
                OnEncryptFinished.Execute(DecryptedFiles.Count(f => f.IsSelected));
                GetDestinationFolder();
            });
        }
        
        private async void GetMediaSize(List<FolderObject> mapping)
        {
            await Task.Run(() =>
            {
                foreach (var folder in mapping)
                {
                    foreach (var file in folder.Files)
                    {
                        var fullPathToFile = $"{BasePath}{folder.FullPath}\\{file.OriginName}";
                        try
                        {
                            using (Stream stream = File.OpenRead(fullPathToFile))
                            {
                                using (var srcImg = Image.FromStream(stream, false, false))
                                {
                                    file.Width = srcImg.Width.ToString();
                                    file.Height = srcImg.Height.ToString();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            var inputFile = new MediaFile { Filename = fullPathToFile };
                            using (var engine = new Engine()) { engine.GetMetadata(inputFile); }

                            if (inputFile.Metadata?.VideoData == null) continue;
                            var size = inputFile.Metadata.VideoData.FrameSize.Split('x');
                            file.Width = size.First();
                            file.Height = size.Last();
                        }
                    }
                }

                GenerateJson(mapping);
            });
        }

        private async void GenerateJson(List<FolderObject> mapping)
        {
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(mapping, Formatting.Indented);
                var jsonFile = $"{DestinationPath}\\{EncryptionUtil.Encipher("mapping.json", 10)}";
                File.WriteAllText(jsonFile, json);

                var bytes = Encoding.UTF8.GetBytes(File.ReadAllText(jsonFile));
                var resultEncrypt = EncryptionUtil.EncryptBytes(bytes, PassValue, SaltValue);
                File.WriteAllBytes(jsonFile, resultEncrypt);
            });
        }

        #endregion //Async methods
    }
}
