using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace TheVault.Objects
{
    public class FolderObject
    {
        public string name { get; set; }
        public string fullPath { get; set; }
        public List<FileObject> files { get; set; }

        public FolderObject()
        {
            files = new List<FileObject>();
        }

        public FolderObject(string name, string fullPath)
        {
            this.name = name;
            this.fullPath = fullPath;
            files = new List<FileObject>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FolderObject folder))
                return base.Equals(obj);

            return folder.name == this.name
                   && folder.fullPath == this.fullPath;
        }
    }
}
