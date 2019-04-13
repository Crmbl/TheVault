// ReSharper disable InconsistentNaming
namespace TheVault.Objects
{
    public class FileObject
    {
        public string originName { get; set; }
        public string updatedName { get; set; }
        public string width { get; set; }
        public string height { get; set; }

        public FileObject()
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileObject file)) 
                return base.Equals(obj);

            return file.width == this.width
                   && file.height == this.height
                   && file.originName == this.originName
                   && file.updatedName == this.updatedName;
        }
    }
}
