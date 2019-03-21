using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheVault.Objects;
using TheVault.Utils;

namespace TheVault.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        #region Instance variables

        private int _filesToTreat;

        private string _message;

        private int _progressValue;

        private RelayCommand _decryptFinished;

        #endregion //Instance variables

        #region Properties

        public int FilesToTreat
        {
            get => _filesToTreat;
            set
            {
                if (_filesToTreat == value) return;
                _filesToTreat = value;
                NotifyPropertyChanged("FilesToTreat");
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value) return;
                _progressValue = value;
                NotifyPropertyChanged("ProgressValue");
            }
        }

        public RelayCommand DecryptFinished
        {
            get => _decryptFinished;
            set
            {
                if (_decryptFinished == value) return;
                _decryptFinished = value;
                NotifyPropertyChanged("DecryptFinished");
            }
        }

        #endregion //Properties

        #region Constructors

        public LoadingViewModel()
        {
            ProgressValue = 0;
            Message = "Begin decrypting files";
        }

        #endregion //Constructors

        #region Public methods

        /// <summary>
        /// Get all the files from the Vault folder and decrypt them in Origin folder.
        /// </summary>
        public async void DecryptFilesAsync()
        {
            var lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\settings");
            if (lines.Length != 6) throw new Exception("The settings file is not correct.");
            
            var basePath = lines[0];
            var originDir = new DirectoryInfo($"{basePath}\\{lines[1]}");
            var vaultDir = new DirectoryInfo($"{basePath}\\{lines[3]}");
            var password = lines[4];
            var salt = lines[5];
            FilesToTreat = vaultDir.GetFiles("*", SearchOption.AllDirectories).Length;

            /*if (FilesToTreat == 0)
            {*/
                Message = $"{ProgressValue} files processed - 100%";
                DecryptFinished.Execute(null);
            /*}
            else
            {
                await Task.Run(() =>
                {
                    var mappingFile = vaultDir.GetFiles().FirstOrDefault(f => EncryptionUtil.Decipher(f.Name, 10) == "mapping.json");
                    if (mappingFile == null) throw new Exception("Mapping file not found.");

                    //Decrypt mapping file
                    var mBytes = File.ReadAllBytes($"{mappingFile.FullName}");
                    var mFile = EncryptionUtil.DecryptBytes(mBytes, password, salt);
                    File.WriteAllBytes($"{originDir.FullName}\\mapping.json", mFile);
                    var mapping = JsonConvert.DeserializeObject<FolderObject>(File.ReadAllText($"{originDir.FullName}\\mapping.json"));
                    ProgressValue++;
                    Message = "mapping.json";

                    //Decrypt all files
                    foreach (var file in vaultDir.GetFiles().Where(f => EncryptionUtil.Decipher(f.Name, 10) != "mapping.json"))
                    {
                        var result = GetFileInformation(mapping, EncryptionUtil.Decipher(file.Name, 10));
                        var pathToFile = $"\\{result.First().Remove(0, originDir.FullName.Length)}";
                        var fBytes = File.ReadAllBytes($"{vaultDir.FullName}\\{file.Name}");
                        var decryptedFile = EncryptionUtil.DecryptBytes(fBytes, password, salt);

                        if (!Directory.Exists($"{originDir.FullName}{pathToFile}"))
                            Directory.CreateDirectory($"{originDir.FullName}{pathToFile}");

                        File.WriteAllBytes($"{originDir.FullName}{pathToFile}\\{result.Last()}", decryptedFile);
                        Message = result.Last();
                        ProgressValue++;
                    }

                    ProgressValue = FilesToTreat;
                    Message = $"{ProgressValue} files processed - 100%";
                    DecryptFinished.Execute(null);
                });
            }*/
        }

        #endregion //Public methods

        #region Private methods

        /// <summary>
        /// Returns the folder and file originName.
        /// </summary>
        private string[] GetFileInformation(FolderObject mapping, string fileName)
        {
            var flattenedTree = new FolderObject(mapping.Name, mapping.FullPath, 
                new List<FileObject>(mapping.Files), new List<FolderObject>(mapping.Folders));

            for (var i = 0; i < flattenedTree.Folders.Count; i++)
                flattenedTree.Folders.AddRange(flattenedTree.Folders[i].Folders);

            if (flattenedTree.Files.Any())
            {
                foreach(var file in flattenedTree.Files)
                    if (file.UpdatedName == fileName)
                        return new []{mapping.FullPath, file.OriginName};
            }

            foreach (var sub in flattenedTree.Folders)
            foreach (var file in sub.Files)
                if (file.UpdatedName == fileName)
                    return new []{sub.FullPath, file.OriginName};

            throw new Exception($"File not found : {fileName}");
        }

        #endregion //Private methods
    }
}
