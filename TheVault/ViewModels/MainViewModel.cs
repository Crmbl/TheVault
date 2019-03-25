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
    //TODO select by default all files : gif/jpg/mp4 ...
    //disable other critical files to selection but load them either way
    //Progress bar not working
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

        private RelayCommand _onEncryptListChanged;

        private RelayCommand _clearDestCmd;

        private RelayCommand _onDecryptListChanged;

        private List<FileViewModel> _encryptedFiles;

        private List<FileViewModel> _decryptedFiles;

        private int _progressBarValue;

        private int _progressBarMax;

        private bool _allSelected;

        private bool _showProgressBar;

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

        public bool ShowProgressBar
        {
            get => _showProgressBar;
            set
            {
                if (_showProgressBar == value) return;
                _showProgressBar = value;
                NotifyPropertyChanged("ShowProgressBar");
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

        public RelayCommand ClearDestCmd
        {
            get => _clearDestCmd;
            set
            {
                if (_clearDestCmd == value) return;
                _clearDestCmd = value;
                NotifyPropertyChanged("ClearDestCmd");
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

        public RelayCommand OnEncryptListChanged
        {
            get => _onEncryptListChanged;
            set
            {
                if (_onEncryptListChanged == value) return;
                _onEncryptListChanged = value;
                NotifyPropertyChanged("OnEncryptListChanged");
            }
        }

        public RelayCommand OnDecryptListChanged
        {
            get => _onDecryptListChanged;
            set
            {
                if (_onDecryptListChanged == value) return;
                _onDecryptListChanged = value;
                NotifyPropertyChanged("OnDecryptListChanged");
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
                OnEncryptListChanged?.Execute(null);
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
                OnDecryptListChanged?.Execute(null);
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

        public int ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                if (_progressBarValue == value) return;
                _progressBarValue = value;
                NotifyPropertyChanged("ProgressBarValue");
            }
        }

        public int ProgressBarMax
        {
            get => _progressBarMax;
            set
            {
                if (_progressBarMax == value) return;
                _progressBarMax = value;
                NotifyPropertyChanged("ProgressBarMax");
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

            ShowProgressBar = false;
            OpenOriginFolderCmd = new RelayCommand(true, _ => OpenOriginFolder());
            OpenDestinationFolderCmd = new RelayCommand(true, _ => OpenDestinationFolder());
            ClearDestCmd = new RelayCommand(true, _ => ClearDestinationFolder());
            RefreshOriginFolderCmd = new RelayCommand(true, _ =>
            {
                GetOriginFolder();
                GetDestinationFolder();
                AllSelected = false;
            });
            EncryptCmd = new RelayCommand(true, _ => EncryptSelected());

            GetOriginFolder();
            GetDestinationFolder();
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

        //TODO clean this mess
        private async void OnClose(object sender, CancelEventArgs e)
        {
            DestinationWatcher.EnableRaisingEvents = false;
            DestinationWatcher.Dispose();
            OriginWatcher.EnableRaisingEvents = false;
            OriginWatcher.Dispose();

            var vaultFolder = new DirectoryInfo(VaultPath);
            var destinationFolder = new DirectoryInfo(DestinationPath);
            var originFolder = new DirectoryInfo(OriginPath);
            
            if (!vaultFolder.EnumerateFiles().Any())
            {
                foreach (var file in destinationFolder.EnumerateFiles())
                    file.MoveTo($"{VaultPath}\\{file.Name}");
            }
            else
            {
                foreach (var file in destinationFolder.EnumerateFiles())
                    file.Delete();
                foreach (var folder in destinationFolder.EnumerateDirectories())
                    folder.Delete(true);
            }

            int jsonFile = originFolder.EnumerateFiles().Any(f => f.Name == "mapping.json") ? 0 : 1;
            var vaultCount = vaultFolder.EnumerateFiles().Count() - jsonFile;
            var originCount = originFolder.EnumerateFiles("*", SearchOption.AllDirectories).Count();
            if (vaultCount < originCount)
            {
                var result = GetMissingFileNames();
                if (result.Count() != 0)
                {
                    //TODO and if there is not json file ??? For now : ERROR !!!
                    var json = new DirectoryInfo(VaultPath).EnumerateFiles().FirstOrDefault(f => EncryptionUtil.Decipher(f.Name, 10) == "mapping.json");
                    var mBytes = File.ReadAllBytes($"{json.FullName}");
                    var mFile = EncryptionUtil.DecryptBytes(mBytes, PassValue, SaltValue);
                    File.WriteAllBytes($"{VaultPath}\\mapping.json", mFile);
                    File.Delete(json.FullName);
                    var mapping = JsonConvert.DeserializeObject<List<FolderObject>>(File.ReadAllText($"{VaultPath}\\mapping.json"));
                    var tmpFolder = mapping.FirstOrDefault(f => f.Name == "Origin");
                    var mappingEntry = tmpFolder.Files.FirstOrDefault(f => f.OriginName == "mapping.json");
                    tmpFolder.Files.Remove(mappingEntry);

                    await Task.Run(() =>
                    {
                        foreach (var fileName in result)
                        {
                            var file = new DirectoryInfo(OriginPath).EnumerateFiles("*", SearchOption.AllDirectories).FirstOrDefault(f => f.Name == fileName);
                            var fBytes = File.ReadAllBytes(file.FullName);
                            var encryptedFile = EncryptionUtil.EncryptBytes(fBytes, PassValue, SaltValue);
                            var ext = file.Extension;
                            var tmpName = file.Name;

                            var i = 0;
                            while (File.Exists($"{VaultPath}\\{EncryptionUtil.Encipher(tmpName, 10)}"))
                                tmpName = tmpName.Replace($"{(i == 0 ? string.Empty : i.ToString())}{ext}", $"{++i}{ext}");

                            var cipheredName = EncryptionUtil.Encipher(tmpName, 10);
                            File.WriteAllBytes($"{VaultPath}\\{cipheredName}", encryptedFile);

                            var folderObject = mapping.FirstOrDefault(f => f.Name == file.Directory.Name);
                            if (folderObject == null)
                            {
                                folderObject = new FolderObject(file.Directory.Name, "");
                                mapping.Add(folderObject);
                            }

                            var tmpFile = new FileObject
                            {
                                OriginName = file.Name,
                                UpdatedName = fileName
                            };
                            try
                            {
                                using (Stream stream = File.OpenRead(file.FullName))
                                {
                                    using (var srcImg = Image.FromStream(stream, false, false))
                                    {
                                        tmpFile.Width = srcImg.Width.ToString();
                                        tmpFile.Height = srcImg.Height.ToString();
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                var inputFile = new MediaFile { Filename = file.FullName };
                                using (var engine = new Engine()) { engine.GetMetadata(inputFile); }

                                if (inputFile.Metadata?.VideoData == null) continue;
                                var size = inputFile.Metadata.VideoData.FrameSize.Split('x');
                                tmpFile.Width = size.First();
                                tmpFile.Height = size.Last();
                            }
                            folderObject.Files.Add(tmpFile);
                        }

                        GenerateJson(mapping, true);
                        File.Delete($"{VaultPath}\\mapping.json");
                        foreach (var file in originFolder.EnumerateFiles("*", SearchOption.AllDirectories))
                            file.Delete();
                        foreach (var folder in originFolder.EnumerateDirectories("*", SearchOption.AllDirectories))
                            folder.Delete(true);
                    });
                }
            }
            else
            {
                foreach (var file in originFolder.EnumerateFiles("*", SearchOption.AllDirectories))
                    file.Delete();
                foreach (var folder in originFolder.EnumerateDirectories("*", SearchOption.AllDirectories))
                    folder.Delete(true);
            }
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
        }

        private IEnumerable<string> GetMissingFileNames()
        {
            var vaultFolder = new DirectoryInfo(VaultPath);
            var decipheredNames = vaultFolder.EnumerateFiles().Select(x => EncryptionUtil.Decipher(x.Name, 10)).ToList();
            var originFolder = new DirectoryInfo(OriginPath);
            
            return originFolder.EnumerateFiles("*", SearchOption.AllDirectories).Select(x => x.Name).Except(decipheredNames);
        }

        private void ClearDestinationFolder()
        {
            var destinationFolder = new DirectoryInfo(DestinationPath);
            foreach (var file in destinationFolder.EnumerateFiles())
                file.Delete();
            foreach (var folder in destinationFolder.EnumerateDirectories())
                folder.Delete(true);
        }

        #endregion //Private methods

        #region Async methods

        private async void EncryptSelected()
        {
            await Task.Run(() =>
            {
                var mapping = new List<FolderObject>();
                ProgressBarMax = DecryptedFiles.Count(f => f.IsSelected) *2 +1;
                ShowProgressBar = true;
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

                    ProgressBarValue++;
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

                        ProgressBarValue++;
                    }
                }

                GenerateJson(mapping);
            });
        }

        private async void GenerateJson(List<FolderObject> mapping, bool onClosing = false)
        {
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(mapping, Formatting.Indented);
                var path = onClosing ? VaultPath : DestinationPath;
                var jsonFile = $"{path}\\{EncryptionUtil.Encipher("mapping.json", 10)}";
                File.WriteAllText(jsonFile, json);

                var bytes = Encoding.UTF8.GetBytes(File.ReadAllText(jsonFile));
                var resultEncrypt = EncryptionUtil.EncryptBytes(bytes, PassValue, SaltValue);
                File.WriteAllBytes(jsonFile, resultEncrypt);
                ProgressBarValue++;
            });
        }

        #endregion //Async methods
    }
}
