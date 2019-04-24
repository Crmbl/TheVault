using System;

// ReSharper disable InconsistentNaming
namespace TheVault.Objects
{
    public class FileObject : IEquatable<FileObject>
    {
        public string originName { get; set; }
        public string updatedName { get; set; }
        public string width { get; set; }
        public string height { get; set; }

        public FileObject()
        {
        }

        public FileObject(string originName, string updatedName, string width, string height)
        {
            this.originName = originName;
            this.updatedName = updatedName;
            this.width = width;
            this.height = height;
        }

        public bool Equals(FileObject other)
        {
            return string.Equals(originName, other.originName) 
                   && string.Equals(updatedName, other.updatedName)
                   && string.Equals(width, other.width) 
                   && string.Equals(height, other.height);
        }
    }
}
