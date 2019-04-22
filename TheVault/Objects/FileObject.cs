// ReSharper disable InconsistentNaming

using System;

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

        public bool Equals(FileObject other)
        {
            return string.Equals(originName, other.originName) 
                   && string.Equals(updatedName, other.updatedName)
                   && string.Equals(width, other.width) 
                   && string.Equals(height, other.height);
        }
    }
}
