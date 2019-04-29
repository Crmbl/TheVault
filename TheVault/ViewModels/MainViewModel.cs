using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MediaToolkit;
using MediaToolkit.Model;
using Newtonsoft.Json;
using SharpAdbClient;
using TheVault.Objects;
using TheVault.Utils;
using Image = System.Drawing.Image;

namespace TheVault.ViewModels
{
    public class MainViewModel : BaseViewModel
    {        
        #region Instance variables

        private string _serverText;
        
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

        private RelayCommand _closeDialogCmd;

        private RelayCommand _openDialogCmd;
        
        private RelayCommand _closeDialogDeviceCmd;

        private RelayCommand _openDialogDeviceCmd;

        private RelayCommand _decryptListItemChangedCmd;
        
        private RelayCommand _encryptListItemChangedCmd;
        
        private RelayCommand _startServerCmd;
        
        private RelayCommand _sendDataCommand;
        
        private RelayCommand _getJsonCommand;

        private List<FileViewModel> _encryptedFiles;

        private List<FileViewModel> _decryptedFiles;

        private List<DeviceData> _devices;
        
        private DeviceData _device;

        private int _progressBarValue;

        private int _progressBarMax;

        private bool _allSelected;

        private bool _showProgressBar;

        private bool _backingUpFiles;

        private string _selectedFilesToolBar;

        private string _encryptedFilesToolBar;

        #endregion //Instance variables

        #region Properties

        public string ServerText
        {
            get => _serverText;
            set
            {
                if (_serverText == value) return;
                _serverText = value;
                NotifyPropertyChanged("ServerText");
            }
        }
        
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
        
        public RelayCommand SendDataCommand
        {
            get => _sendDataCommand;
            set
            {
                if (_sendDataCommand == value) return;
                _sendDataCommand = value;
                NotifyPropertyChanged("SendDataCommand");
            }
        }
        
        public RelayCommand StartServerCmd
        {
            get => _startServerCmd;
            set
            {
                if (_startServerCmd == value) return;
                _startServerCmd = value;
                NotifyPropertyChanged("StartServerCmd");
            }
        }

        public RelayCommand CloseDialogCmd
        {
            get => _closeDialogCmd;
            set
            {
                if (_closeDialogCmd == value) return;
                _closeDialogCmd = value;
                NotifyPropertyChanged("CloseDialogCmd");
            }
        }
        
        public RelayCommand OpenDialogCmd
        {
            get => _openDialogCmd;
            set
            {
                if (_openDialogCmd == value) return;
                _openDialogCmd = value;
                NotifyPropertyChanged("OpenDialogCmd");
            }
        }
        
        public RelayCommand CloseDialogDeviceCmd
        {
            get => _closeDialogDeviceCmd;
            set
            {
                if (_closeDialogDeviceCmd == value) return;
                _closeDialogDeviceCmd = value;
                NotifyPropertyChanged("CloseDialogDeviceCmd");
            }
        }
        
        public RelayCommand OpenDialogDeviceCmd
        {
            get => _openDialogDeviceCmd;
            set
            {
                if (_openDialogDeviceCmd == value) return;
                _openDialogDeviceCmd = value;
                NotifyPropertyChanged("OpenDialogDeviceCmd");
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
        
        public RelayCommand DecryptListItemChangedCmd
        {
            get => _decryptListItemChangedCmd;
            set
            {
                if (_decryptListItemChangedCmd == value) return;
                _decryptListItemChangedCmd = value;
                NotifyPropertyChanged("DecryptListItemChangedCmd");
            }
        }
        
        public RelayCommand EncryptListItemChangedCmd
        {
            get => _encryptListItemChangedCmd;
            set
            {
                if (_encryptListItemChangedCmd == value) return;
                _encryptListItemChangedCmd = value;
                NotifyPropertyChanged("EncryptListItemChangedCmd");
            }
        }
            
        public RelayCommand GetJsonCommand
        {
            get => _getJsonCommand;
            set
            {
                if (_getJsonCommand == value) return;
                _getJsonCommand = value;
                NotifyPropertyChanged("GetJsonCommand");
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

        public string SelectedFilesToolBar
        {
            get => _selectedFilesToolBar;
            set
            {
                _selectedFilesToolBar = $"Files selected : {value}";
                NotifyPropertyChanged("SelectedFilesToolBar");
            }
        }
        
        public string EncryptedFilesToolBar
        {
            get => _encryptedFilesToolBar;
            set
            {
                _encryptedFilesToolBar = $"Files in folder : {value}";
                NotifyPropertyChanged("EncryptedFilesToolBar");
            }
        }
        
        public bool AllSelected
        {
            get => _allSelected;
            set
            {
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

        public bool BackingUpFiles
        {
            get => _backingUpFiles;
            set
            {
                if (_backingUpFiles == value) return;
                _backingUpFiles = value;
                NotifyPropertyChanged("BackingUpFiles");
            }
        }
        
        public List<DeviceData> Devices
        {
            get => _devices;
            set
            {
                if (_devices == value) return;
                _devices = value;
                NotifyPropertyChanged("Devices");
            }
        }
        
        public DeviceData Device
        {
            get => _device;
            set
            {
                if (_device == value) return;
                _device = value;
                NotifyPropertyChanged("Device");
            }
        }

        private List<FolderObject> Mapping { get; set; }
        
        private List<FolderObject> DownloadedMapping { get; set; }

        public bool OriginFolderEmpty => !DecryptedFiles.Any();
        
        public bool DestinationFolderEmpty => !EncryptedFiles.Any();

        private bool IsItemSelectionChanged { get; set; }

        private string BasePath { get; set; }

        private FileSystemWatcher DestinationWatcher { get; set; }

        private FileSystemWatcher OriginWatcher { get; set; }
        
        private List<string> OriginFileNames { get; set; }
        
        private List<string> DestinationFileNames { get; set; }
        
        private AdbServer AdbServer { get; set; }

        private DeviceMonitor DeviceMonitor { get; set; }

        #endregion //Properties

        #region Constructors

        public MainViewModel(List<FolderObject> mapping)
        {
            #region Init
            
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Height = 900;
                Application.Current.MainWindow.Width = 1200;
                Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
                Application.Current.MainWindow.Closing += OnClosing;
                Application.Current.MainWindow.Closed += OnClosed;
                
                var left = (int)Math.Round(Application.Current.MainWindow.Left + Application.Current.MainWindow.Width);
                var top = (int)Math.Round(Application.Current.MainWindow.Top);
                ConsoleManager.Show(left, top);
            }
            
            Mapping = mapping ?? new List<FolderObject>();
            DecryptedFiles = new List<FileViewModel>();
            EncryptedFiles = new List<FileViewModel>();

            var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            BasePath = lines[0].EndsWith("\\") ? lines[0] : $"{lines[0]}\\";
            OriginPath = $"{BasePath}{lines[1]}";
            DestinationPath = $"{BasePath}{lines[2]}";
            VaultPath = $"{BasePath}{lines[3]}";
            PassValue = lines[4];
            SaltValue = lines[5];
            
            OriginFileNames = new DirectoryInfo(OriginPath).EnumerateFiles("*", SearchOption.AllDirectories).Select(f => f.FullName).ToList();
            DestinationFileNames = new DirectoryInfo(DestinationPath).EnumerateFiles("*", SearchOption.AllDirectories).Select(f => f.FullName).ToList();
            
            ServerText = "START SERVER";
            BackingUpFiles = false;
            AllSelected = false;
            ShowProgressBar = false;
            
            OpenOriginFolderCmd = new RelayCommand(true, _ => OpenOriginFolder());
            OpenDestinationFolderCmd = new RelayCommand(true, _ => OpenDestinationFolder());
            ClearDestCmd = new RelayCommand(true, _ => ClearDestinationFolder(false));
            StartServerCmd = new RelayCommand(true, _ => StartServer());
            SendDataCommand = new RelayCommand(true, _ => SendData());
            GetJsonCommand = new RelayCommand(true, _ => GetJson());
            RefreshOriginFolderCmd = new RelayCommand(true, _ =>
            {
                GetOriginFolder();
                GetDestinationFolder();
                AllSelected = false;
            });
            EncryptCmd = new RelayCommand(true, _ =>
            {
                ClearDestinationFolder(true);
                EncryptSelected();
            });

            OriginWatcher = new FileSystemWatcher
            {
                Path = OriginPath,
                Filter = "*",
                IncludeSubdirectories = true,
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
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            DestinationWatcher.Changed += OnChanged;
            DestinationWatcher.Created += OnChanged;
            DestinationWatcher.Deleted += OnChanged;
            DestinationWatcher.Renamed += OnRenamed;
            DestinationWatcher.EnableRaisingEvents = true;
            
            #endregion //Init
            
            GetOriginFolder();
            GetDestinationFolder();
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

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (BackingUpFiles) return;

            e.Cancel = true;
            OpenDialogCmd.Execute(null); 
        }
        
        private void OnClosed(object sender, EventArgs e)
        {
            DestinationWatcher.EnableRaisingEvents = false;
            OriginWatcher.EnableRaisingEvents = false;
        }

        public void OnCloseDialog(object param)
        {
            if (!(param is bool result)) return;
            if (!result) return;
            
            BackupFiles();
        }

        public async void OnCloseDialogDevice(object param)
        {
            if (Device == null)
                Device = Devices.FirstOrDefault();
            
            WriteLine($@"Selected device: {Device?.Model}");
            await Task.Delay(2000).ContinueWith(_ => { ServerMessage = ""; });
        }
        
        private async void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            await Task.Delay(100);
            Devices = AdbClient.Instance.GetDevices();
            WriteLine($@"{Devices.FirstOrDefault(d => d.Serial == e.Device.Serial)?.Model} connected to server");
            
            if (Devices.Count > 1)
                OpenDialogDeviceCmd.Execute(null);
            else
                Device = Devices.LastOrDefault();
        }

        private async void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            await Task.Delay(100);
            WriteLine($@"{Devices.FirstOrDefault(d => d.Serial == e.Device.Serial)?.Model} disconnected from server");
            Devices = AdbClient.Instance.GetDevices();
            
            if (Devices.Count > 1)
                OpenDialogDeviceCmd.Execute(null);
            else
                Device = Devices.LastOrDefault();
        }

        private void AllSelectedChanged()
        {
            if (IsItemSelectionChanged)
            {
                IsItemSelectionChanged = false;
                return;
            }

            if (AllSelected)
                DecryptedFiles.ForEach(f => { if (f.IsEnabled) f.IsSelected = true; });
            else
                DecryptedFiles.ForEach(f => { if (f.IsEnabled) f.IsSelected = false; });
            
            SelectedFilesToolBar = DecryptedFiles.Count(f => f.IsSelected).ToString();
        }

        private void ItemSelectionChanged()
        {
            SelectedFilesToolBar = DecryptedFiles.Count(f => f.IsSelected).ToString();
            if (AllSelected)
            {
                IsItemSelectionChanged = true;
                AllSelected = false;
            }
            else
            {
                if (!DecryptedFiles.Where(f => f.IsEnabled).All(f => f.IsSelected)) return;
                IsItemSelectionChanged = true;
                AllSelected = true;
            }
        }

        public void SortEncryptButtonChanged(object item)
        {
            var name = (item as ListBoxItem)?.Name;
            switch (name)
            {
                case "SortAlpha":
                    EncryptedFiles = EncryptedFiles.OrderByDescending(f => f.FileName).ToList();
                    break;
                
                case "SortWeight":
                    EncryptedFiles = EncryptedFiles.OrderByDescending(f =>
                    {
                        if (f.SizeMb.Contains("Mo"))
                            return int.Parse(f.SizeMb.Remove(f.SizeMb.Length - 3)) * 1024;
                        
                        return int.Parse(f.SizeMb.Remove(f.SizeMb.Length - 3));
                    }).ToList();
                    break;
                
                case null:
                    EncryptedFiles = EncryptedFiles.OrderBy(f => f.FileName).ToList();
                    break;
            }
        }

        public void SortDecryptButtonChanged(object item)
        {
            var name = (item as ListBoxItem)?.Name;
            switch (name)
            {
                case "SortAlphaOne":
                    DecryptedFiles = DecryptedFiles.OrderBy(f => f.FileName).ToList();
                    break;
                
                case "SortSelected":
                    DecryptedFiles = DecryptedFiles.OrderByDescending(f => f.IsSelected && f.IsNew && f.IsEnabled)
                                                    .ThenByDescending(f => f.IsSelected && !f.IsNew && f.IsEnabled)
                                                    .ThenByDescending(f => !f.IsSelected && f.IsNew && f.IsEnabled)
                                                    .ThenByDescending(f => !f.IsSelected && !f.IsNew && f.IsEnabled)
                                                    .ThenByDescending(f => !f.IsEnabled).ToList();
                    break;
                
                case "SortPath":
                    DecryptedFiles = DecryptedFiles.OrderByDescending(f => f.Path).ToList();
                    break;
                
                case null:
                    DecryptedFiles = DecryptedFiles.OrderBy(f => f.Path).ToList();
                    break;
            }
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
            var files = new DirectoryInfo(OriginPath).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            var fileNames = files.Select(f => f.FullName).ToList();

            if (files.Count == OriginFileNames.Count && DecryptedFiles.Count == OriginFileNames.Count)
            {
                var diff = fileNames.Except(OriginFileNames).ToList();
                if (diff.Count == 0) return;
            }
            
            OriginFileNames = fileNames;
            DecryptedFiles = new List<FileViewModel>();
            if (Mapping != null)
            {
                var mappingFiles = new List<FileObject>();
                Mapping.ForEach(fo => mappingFiles.AddRange(fo.files));

                foreach (var file in files)
                {
                    var isEnabled = FileUtil.FileExtensions.Contains(file.Extension);
                    var folderName = file.DirectoryName?.Remove(0, BasePath.Length);
                    var existingInMapping = Mapping.FirstOrDefault(fo => fo.fullPath == folderName)?.files.Any(fi => fi.originName == file.Name);
                    var fileViewModel = mappingFiles.Any(f => f.originName == file.Name && existingInMapping != null && existingInMapping == true) ? 
                        new FileViewModel(isEnabled, file.Name, folderName) : new FileViewModel(isEnabled, file.Name, folderName, isEnabled);
                    
                    fileViewModel.SelectionChanged = new RelayCommand(true, _ => ItemSelectionChanged());
                    DecryptedFiles.Add(fileViewModel);
                }
            }
            else if (Mapping == null)
            {
                foreach (var file in files)
                {
                    var isEnabled = FileUtil.FileExtensions.Contains(file.Extension);
                    var fileViewModel = new FileViewModel(isEnabled, file.Name, file.DirectoryName?.Remove(0, BasePath.Length), isEnabled)
                    {
                        SelectionChanged = new RelayCommand(true, _ => ItemSelectionChanged())
                    };
                    DecryptedFiles.Add(fileViewModel);
                }
            }
            
            AllSelected = true;
            SelectedFilesToolBar = DecryptedFiles.Count(f => f.IsSelected).ToString();
            NotifyPropertyChanged("OriginFolderEmpty");
        }

        private void GetDestinationFolder()
        {
            var files = new DirectoryInfo(DestinationPath).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            var fileNames = files.Select(f => f.FullName).ToList();
            
            if (files.Count == DestinationFileNames.Count && EncryptedFiles.Count == DestinationFileNames.Count)
            {
                var diff = fileNames.Except(DestinationFileNames).ToList();
                if (diff.Count == 0) return;
            }

            DestinationFileNames = fileNames;
            EncryptedFiles = new List<FileViewModel>();
            foreach (var file in files)
            {
                var fileSize = file.Length / 1024 > 1024 ? $"{file.Length / 1024 / 1024} Mo" : file.Length / 1024 == 0 ? "1 Ko" : $"{file.Length / 1024} Ko";
                var fileViewModel = new FileViewModel(file.Name, fileSize);
                EncryptedFiles.Add(fileViewModel);
            }

            EncryptedFilesToolBar = EncryptedFiles.Count.ToString();
            NotifyPropertyChanged("DestinationFolderEmpty");
        }

        private void ClearDestinationFolder(bool isEncrypt)
        {
            var destinationFolder = new DirectoryInfo(DestinationPath);
            var list = isEncrypt ? destinationFolder.EnumerateFiles().Where(f => f.Name != "mobileMapping.json") : destinationFolder.EnumerateFiles();
            foreach (var file in list)
                file.Delete();
            foreach (var folder in destinationFolder.EnumerateDirectories())
                folder.Delete(true);
        }

        private List<FolderObject> RemoveMissingFiles(List<FolderObject> mapping)
        {
            var tmpMapping = new List<FolderObject>();
            tmpMapping.AddRange(mapping.Select(fo => new FolderObject(fo.name, fo.fullPath, 
                fo.files.Select(fi => new FileObject(fi.originName, fi.updatedName, fi.width, fi.height)).ToList())));
            foreach (var folder in tmpMapping)
            {
                if (!Directory.Exists($"{BasePath}{folder.fullPath}"))
                {
                    foreach (var file in folder.files)
                        File.Delete($"{VaultPath}\\{EncryptionUtil.Encipher(file.updatedName, 10)}");
                    
                    mapping.Remove(folder);
                }
                else
                {
                    foreach (var file in folder.files)
                    {
                        if (File.Exists($"{BasePath}{folder.fullPath}\\{file.originName}")) continue;
                        
                        File.Delete($"{VaultPath}\\{EncryptionUtil.Encipher(file.updatedName, 10)}");
                        mapping.FirstOrDefault(fo => fo.Equals(folder))?.files.Remove(file);
                    }
                }
            }

            return mapping;
        }
        
        private void GetMediaSize(List<FolderObject> mapping, bool onClosing = false)
        {
            foreach (var folder in mapping)
            {
                foreach (var file in folder.files)
                {
                    var fullPathToFile = $"{BasePath}{folder.fullPath}\\{file.originName}";
                    try
                    {
                        using (Stream stream = File.OpenRead(fullPathToFile))
                        {
                            using (var srcImg = Image.FromStream(stream, false, false))
                            {
                                file.width = srcImg.Width.ToString();
                                file.height = srcImg.Height.ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        var inputFile = new MediaFile { Filename = fullPathToFile };
                        using (var engine = new Engine()) { engine.GetMetadata(inputFile); }

                        if (inputFile.Metadata?.VideoData == null) continue;
                        var size = inputFile.Metadata.VideoData.FrameSize.Split('x');
                        file.width = size.First();
                        file.height = size.Last();
                    }

                    ProgressBarValue++;
                }
            }

            GenerateJson(mapping, onClosing);
        }

        private void GenerateJson(List<FolderObject> mapping, bool onClosing = false)
        {
            var json = JsonConvert.SerializeObject(mapping, Formatting.Indented);
            var path = onClosing ? VaultPath : DestinationPath;
            var jsonFile = $"{path}\\{EncryptionUtil.Encipher("mapping.json", 10)}";
            File.WriteAllText(jsonFile, json);

            var bytes = Encoding.UTF8.GetBytes(File.ReadAllText(jsonFile));
            var resultEncrypt = EncryptionUtil.EncryptBytes(bytes, PassValue, SaltValue);
            File.WriteAllBytes(jsonFile, resultEncrypt);
            ProgressBarValue++;
            
            File.Delete($"{path}\\mapping.json");
            var destDir = new DirectoryInfo(DestinationPath);
            
            if (destDir.EnumerateFiles().Any(f => f.Name == "mobileMapping.json"))
                EditJson();
        }

        private async void StartServer()
        {
            //TODO MUST WORK ON THIS
            if (AdbServer == null)
            {
                ServerText = "KILL SERVER";
                ProgressBarMax = 0;
                ProgressBarValue = 0;

                var adbPath = $@"C:\Users\{Environment.UserName}\AppData\Local\Android\Sdk\platform-tools\adb.exe";
                if (!File.Exists(adbPath)) throw new FileNotFoundException("Error can't find adb.exe");
                await Task.Run(() =>
                {
                    AdbServer = new AdbServer();
                    AdbServer.StartServer(adbPath, false);
                });
                WriteLine(AdbServer.GetStatus().ToString());
                
                DeviceMonitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
                DeviceMonitor.DeviceConnected += OnDeviceConnected;
                DeviceMonitor.DeviceDisconnected += OnDeviceDisconnected;
                DeviceMonitor.Start();
            }
            else
            {
                ServerText = "START SERVER";
                AdbClient.Instance.KillAdb();
                AdbServer = null;
                DeviceMonitor.Dispose();
                DeviceMonitor = null;
                WriteLine("AdbServer has been killed");
                
                await Task.Delay(2000).ContinueWith(_ =>
                {
                    ServerMessage = "";
                    ProgressBarMax = 0;
                    ProgressBarValue = 0;
                    ShowProgressBar = false;
                });
            }
        }

        private void WriteLine(string message)
        {
            Console.WriteLine(message);
            ServerMessage = message;
        }

        private void ServerCallback(object info)
        {
            switch (info)
            {
                case string message:
                    ServerMessage = message;
                    break;
                case long value:
                    ProgressBarMax = Convert.ToInt32(value);
                    ShowProgressBar = true;
                    break;
                case int value:
                    ProgressBarValue = value;
                    break;
                case byte[] jsonBytes:
                    DecryptDownloadedJson(jsonBytes);
                    break;
            }
            
            if (ServerMessage.Contains("Socket transfer is done"))
                Task.Delay(2500).ContinueWith(_ =>
                {
                    ServerMessage = "";
                    ProgressBarMax = 0;
                    ProgressBarValue = 0;
                    ShowProgressBar = false;
                });
        }

        private void SendData()
        {
            /*//TODO implement sending files
            var list = EncryptedFiles.Take(1);
            foreach (var file in list)
            {
                
            }*/
        }

        private void GetJson()
        {
            var receiver = new ConsoleOutputReceiver();
            AdbClient.Instance.ExecuteRemoteCommand("ls -n /Android", Device, receiver);

            Console.WriteLine("The device responded:");
            Console.WriteLine(receiver.ToString());
            
            
            //TODO !
            using (var service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), Device))
            using (var stream = File.OpenWrite(@"C:\Users\axels\Downloads\test.json"))
            {
                service.Pull("/data/local/tmp/wkzzsxq.tcyx", stream, null, CancellationToken.None);
            }
        }
        
        #endregion //Private methods

        #region Async methods
    
        private async void BackupFiles()
        {
            BackingUpFiles = true;
            ProgressBarValue = 0;
            var vaultFolder = new DirectoryInfo(VaultPath);
            var destinationFolder = new DirectoryInfo(DestinationPath);
            var originFolder = new DirectoryInfo(OriginPath);

            File.Delete($"{DestinationPath}\\mobileMapping.json");
            if (!vaultFolder.EnumerateFiles().Any() && !destinationFolder.EnumerateFiles().Any())
            { //There is no file in Vault nor in dest, so we take every file from the decryptedFiles list (i.e.: origin folder)
                var mapping = new List<FolderObject>();
                await Task.Run(() =>
                {
                    ProgressBarMax = DecryptedFiles.Count;
                    foreach (var file in DecryptedFiles)
                    {
                        var filePath = $"{file.Path.Remove(0, OriginPath.Length - BasePath.Length)}";
                        var fBytes = File.ReadAllBytes($"{OriginPath}{filePath}\\{file.FileName}");
                        var encryptedFile = EncryptionUtil.EncryptBytes(fBytes, PassValue, SaltValue);
                        var ext = file.FileName.Split('.').Last().Insert(0, ".");
                        var fileName = file.FileName;

                        var i = 0;
                        while (File.Exists($"{VaultPath}\\{EncryptionUtil.Encipher(fileName, 10)}"))
                            fileName = fileName.Replace($"{(i == 0 ? string.Empty : i.ToString())}{ext}", $"{++i}{ext}");

                        var cipheredName = EncryptionUtil.Encipher(fileName, 10);
                        File.WriteAllBytes($"{VaultPath}\\{cipheredName}", encryptedFile);

                        var fName = file.Path.Contains("\\") ? file.Path.Split('\\').Last() : file.Path;
                        var folderObject = mapping.FirstOrDefault(f => f.name == fName && f.fullPath == file.Path);
                        if (folderObject == null)
                        {
                            folderObject = new FolderObject(fName, file.Path);
                            mapping.Add(folderObject);
                        }

                        folderObject.files.Add(new FileObject
                        {
                            originName = file.FileName,
                            updatedName = fileName
                        });

                        ProgressBarValue++;
                    }
                }).ContinueWith(_ =>
                {
                    GetMediaSize(mapping, true);
                });
            }
            else
            {
                var mapping = new List<FolderObject>();
                await Task.Run(() =>
                {
                    foreach (var file in destinationFolder.EnumerateFiles())
                    {
                        if (File.Exists($"{VaultPath}\\{file.Name}") && file.Name == EncryptionUtil.Encipher("mapping.json", 10))
                            File.Delete($"{VaultPath}\\{file.Name}");
                        
                        if (!File.Exists($"{VaultPath}\\{file.Name}"))
                            file.MoveTo($"{VaultPath}\\{file.Name}");
                        else
                            File.Delete($"{DestinationPath}\\{file.Name}");
                    }
                    
                    var json = new DirectoryInfo(VaultPath).EnumerateFiles().First(f => EncryptionUtil.Decipher(f.Name, 10) == "mapping.json");
                    var mBytes = File.ReadAllBytes($"{json.FullName}");
                    var mFile = EncryptionUtil.DecryptBytes(mBytes, PassValue, SaltValue);
                    File.WriteAllBytes($"{VaultPath}\\mapping.json", mFile);
                    File.Delete(json.FullName);

                    mapping = JsonConvert.DeserializeObject<List<FolderObject>>(File.ReadAllText($"{VaultPath}\\mapping.json"));
                    mapping = RemoveMissingFiles(mapping);

                    ProgressBarMax = DecryptedFiles.Count;
                    foreach (var file in DecryptedFiles)
                    {
                        ProgressBarValue++;
                        if (file.FileName == "mapping.json") continue;

                        var fName = file.Path.Contains("\\") ? file.Path.Split('\\').Last() : file.Path;
                        var tmpFolder = mapping.FirstOrDefault(fo => fo.name == fName && fo.fullPath == file.Path);
                        if (tmpFolder == null)
                        {
                            tmpFolder = new FolderObject(fName, file.Path);
                            mapping.Add(tmpFolder);
                        }
                        
                        var tmpFile = tmpFolder.files.FirstOrDefault(fi => fi.originName == file.FileName);
                        if (tmpFile != null) continue;

                        var ext = file.FileName.Split('.').Last().Insert(0, ".");
                        var fileName = file.FileName;

                        var fileSameNameCount = DecryptedFiles.Count(fi => fi.FileName == file.FileName);
                        var mappingSameNameCount = mapping.SelectMany(fo => fo.files.Where(fi => fi.originName == file.FileName)).Count();
                        if (fileSameNameCount > 1 && mappingSameNameCount > 0)
                            fileName = fileName.Replace($"{ext}", $"{mappingSameNameCount}{ext}");
                        
                        var cipheredName = EncryptionUtil.Encipher(fileName, 10);
                        if (!File.Exists($"{VaultPath}\\{cipheredName}"))
                        {
                            var filePath = $"{file.Path.Remove(0, OriginPath.Length - BasePath.Length)}";
                            var fBytes = File.ReadAllBytes($"{OriginPath}{filePath}\\{file.FileName}");
                            var encryptedFile = EncryptionUtil.EncryptBytes(fBytes, PassValue, SaltValue);
                            File.WriteAllBytes($"{VaultPath}\\{cipheredName}", encryptedFile);
                        }
                            
                        tmpFolder.files.Add(new FileObject
                        {
                            originName = file.FileName,
                            updatedName = fileName
                        });
                    }
                }).ContinueWith(_ =>
                {
                    GetMediaSize(mapping, true);
                });
            }
            
            #region Purge of Destination and Origin folders
            
            foreach (var file in destinationFolder.EnumerateFiles())
                file.Delete();
            foreach (var folder in destinationFolder.EnumerateDirectories())
                folder.Delete(true);
            
            foreach (var file in originFolder.EnumerateFiles("*", SearchOption.AllDirectories))
                file.Delete();
            foreach (var folder in originFolder.EnumerateDirectories("*", SearchOption.AllDirectories))
                folder.Delete(true);

            #endregion //Purge of Destination and Origin folders
            
            Application.Current.MainWindow?.Close();
        }
        
        private async void EncryptSelected()
        {
            var mapping = new List<FolderObject>();
            await Task.Run(() =>
            {
                ProgressBarValue = 0;
                ProgressBarMax = DecryptedFiles.Count(f => f.IsSelected) *2;
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
                    var folderObject = mapping.FirstOrDefault(f => f.name == fName && f.fullPath == file.Path);
                    if (folderObject == null)
                    {
                        folderObject = new FolderObject(fName, file.Path);
                        mapping.Add(folderObject);
                    }
                    
                    folderObject.files.Add(new FileObject
                    {
                        originName = file.FileName,
                        updatedName = fileName
                    });

                    ProgressBarValue++;
                }
            });
            await Task.Run(() =>
            {
                GetMediaSize(mapping);
            }).ContinueWith(_ =>
            {
                OnEncryptFinished.Execute(DecryptedFiles.Count(f => f.IsSelected));
                GetDestinationFolder();
            });
        }

        private async void DecryptDownloadedJson(byte[] jsonBytes)
        {
            await Task.Run(() =>
            {
                var destDir = new DirectoryInfo(DestinationPath);
                var mFile = EncryptionUtil.DecryptBytes(jsonBytes, PassValue, SaltValue);
                File.WriteAllBytes($"{destDir.FullName}\\mobileMapping.json", mFile);
                DownloadedMapping = JsonConvert.DeserializeObject<List<FolderObject>>(File.ReadAllText($"{destDir.FullName}\\mobileMapping.json"));
            }).ContinueWith(t =>
            {
                var destDir = new DirectoryInfo(DestinationPath);
                if (!destDir.EnumerateFiles().Any() || destDir.EnumerateFiles().All(f => f.Name == "mobileMapping.json")) 
                    return;
                
                EditJson();
            });
        }
        
        private async void EditJson()
        {
            if (DownloadedMapping == null)
                DownloadedMapping = JsonConvert.DeserializeObject<List<FolderObject>>(File.ReadAllText($"{DestinationPath}\\mobileMapping.json"));
            
            var json = new DirectoryInfo(DestinationPath).EnumerateFiles().FirstOrDefault(f => EncryptionUtil.Decipher(f.Name, 10) == "mapping.json");
            if (json != null)
            {
                await Task.Run(() =>
                {
                    var mBytes = File.ReadAllBytes($"{json.FullName}");
                    var mFile = EncryptionUtil.DecryptBytes(mBytes, PassValue, SaltValue);
                    File.WriteAllBytes($"{DestinationPath}\\mapping.json", mFile);
                    File.Delete(json.FullName);

                    var mapping = JsonConvert.DeserializeObject<List<FolderObject>>(File.ReadAllText($"{DestinationPath}\\mapping.json"));
                    mapping.FirstOrDefault(f => f.name == "Origin")?.files.RemoveAll(f => f.originName == "mapping.json");
                    
                    foreach (var folder in DownloadedMapping)
                        foreach (var lFolder in mapping)
                            if (folder.Equals(lFolder) && folder.files.Count != lFolder.files.Count)
                                foreach (var lFile in lFolder.files)
                                    if (folder.files.FirstOrDefault(f => f.Equals(lFile)) == null) //New file
                                        folder.files.Add(lFile);

                    foreach (var folder in mapping)
                    {
                        var dFolder = DownloadedMapping.FirstOrDefault(fo => fo.Equals(folder));
                        if (dFolder == null) //New folder
                            DownloadedMapping.Add(folder);
                    }
                }).ContinueWith(t =>
                {
                    File.Delete($"{DestinationPath}\\mobileMapping.json");
                    GenerateJson(DownloadedMapping);
                });
            }
        }
        
        #endregion //Async methods
    }
}
