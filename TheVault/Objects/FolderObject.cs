using System.Collections.Generic;

namespace TheVault.Objects
{
    public class FolderObject
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public List<FileObject> Files { get; set; }

        public FolderObject()
        {
            Files = new List<FileObject>();
        }

        public FolderObject(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
            Files = new List<FileObject>();
        }
    }
}
