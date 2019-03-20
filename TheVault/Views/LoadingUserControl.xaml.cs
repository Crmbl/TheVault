﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using TheVault.Objects;
using TheVault.Utils;
using TheVault.ViewModels;

namespace TheVault.Views
{
    //TODO Clean this up!
    public partial class LoadingUserControl : UserControl
    {
        public LoadingUserControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            DataContext = new LoadingViewModel();
            var viewModel = (LoadingViewModel) DataContext;
            viewModel.ProgressValue = 0;
            viewModel.Message = "Begin decrypting files";
            DecryptFilesAsync();
        }

        private async void DecryptFilesAsync()
        {
            var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            var vaultDir = new DirectoryInfo($"{lines[0]}\\{lines[3]}");
            var originDir = new DirectoryInfo($"{lines[0]}\\{lines[1]}");
            var password = lines[4];
            var salt = lines[5];
            var viewModel = (LoadingViewModel)DataContext;
            viewModel.FilesToTreat = vaultDir.GetFiles("*", SearchOption.AllDirectories).Length;

            await Task.Run(() => {
                //Get mapping file
                foreach (var file in vaultDir.GetFiles())
                {
                    if (EncryptionUtil.Decipher(file.Name, 10) != "mapping.json") continue;
                    var bytes = File.ReadAllBytes($"{vaultDir.FullName}\\{file.Name}");
                    var decryptedFile = EncryptionUtil.DecryptBytes(bytes, password, salt);
                    File.WriteAllBytes($"{originDir.FullName}\\mapping.json", decryptedFile);

                    viewModel.ProgressValue++;
                    viewModel.Message = "mapping.json";
                    break;
                }
                var mapping = JsonConvert.DeserializeObject<FolderObject>(File.ReadAllText($"{originDir.FullName}\\mapping.json"));
            
                //Then decrypt all files
                foreach (var file in vaultDir.GetFiles())
                {
                    var clearName = EncryptionUtil.Decipher(file.Name, 10);
                    if (clearName == "mapping.json") continue;
                
                    var result = GetFileFullPath(mapping, clearName);
                    var tmp = $"{lines[0]}\\{lines[1]}".Length;
                    var pathToFile = result.First().Length == tmp-1 ? "\\" : $"\\{result.First().Remove(0, tmp)}";

                    var bytes = File.ReadAllBytes($"{vaultDir.FullName}\\{file.Name}");
                    var decryptedFile = EncryptionUtil.DecryptBytes(bytes, password, salt);

                    if (!Directory.Exists($"{originDir.FullName}{pathToFile}"))
                        Directory.CreateDirectory($"{originDir.FullName}{pathToFile}");
                    File.WriteAllBytes($"{originDir.FullName}{pathToFile}\\{result.Last()}", decryptedFile);
                
                    viewModel.Message = result.Last();
                    viewModel.ProgressValue++;
                }
            })
            .ContinueWith(r =>
            {
                viewModel.Message = $"{viewModel.ProgressValue} files processed -- 100%";
                Task.Delay(1500).ContinueWith(t =>
                {
                    Application.Current.Dispatcher.Invoke(() => { Visibility = Visibility.Collapsed; });
                });
            });
        }

        private string[] GetFileFullPath(FolderObject folder, string fileName)
        {
            var flattenedTree = new FolderObject(folder.Name, folder.FullPath, new List<FileObject>(folder.Files), new List<FolderObject>(folder.Folders));
            for (var i = 0; i < flattenedTree.Folders.Count; i++)
                flattenedTree.Folders.AddRange(flattenedTree.Folders[i].Folders);

            if (flattenedTree.Files.Any())
            {
                foreach(var file in flattenedTree.Files)
                    if (file.UpdatedName == fileName)
                        return new []{folder.FullPath, file.OriginName};
            }

            foreach (var sub in flattenedTree.Folders)
                foreach (var file in sub.Files)
                    if (file.UpdatedName == fileName)
                        return new []{sub.FullPath, file.OriginName};

            throw new Exception($"File not found : {fileName}");
        }
    }
}
