namespace TheVault.Objects
{
    public class VaultFile
    {
        public bool IsSelected { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public VaultFile(bool isSelected, string fileName, string path)
        {
            IsSelected = isSelected;
            FileName = fileName;
            Path = path;
        }
    }
}
