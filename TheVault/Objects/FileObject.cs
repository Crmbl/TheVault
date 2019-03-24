namespace TheVault.Objects
{
    public class FileObject
    {
        public string OriginName { get; set; }
        public string UpdatedName { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public FileObject()
        {
        }

        public FileObject(string originName, string updatedName)
        {
            OriginName = originName;
            UpdatedName = updatedName;
        }
    }
}
