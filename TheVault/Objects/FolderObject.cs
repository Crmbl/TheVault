using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace TheVault.Objects
{
    public class FolderObject : IEquatable<FolderObject>
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

        public FolderObject(string name, string fullPath, List<FileObject> files)
        {
            this.name = name;
            this.fullPath = fullPath;
            this.files = files;
        }
        
        public bool Equals(FolderObject other)
        {
            return string.Equals(name, other.name) 
                   && string.Equals(fullPath, other.fullPath);
        }
    }
}
