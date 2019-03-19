using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using TheVault.Objects;

namespace TheVault.Utils
{
    public class FileUtil
    {
        //public static FolderObject WalkDirectoryTree(FolderObject folderObject, DirectoryInfo root)
        //{
        //    folderObject.Name = root.Name;
        //    FileInfo[] files = null;
        //    try
        //    {
        //        files = root.GetFiles("*.*");
        //    }
        //    catch (UnauthorizedAccessException e) { Console.WriteLine(e.Message); }
        //    catch (DirectoryNotFoundException e) { Console.WriteLine(e.Message); }

        //    if (files != null && files.Any())
        //    {
        //        foreach (var fi in files)
        //        {
        //            FileObject file;
        //            try
        //            {
        //                using (Stream stream = File.OpenRead(fi.FullName))
        //                {
        //                    using (var srcImg = Image.FromStream(stream, false, false))
        //                    {
        //                        file = new FileObject(fi.Name, srcImg.Width.ToString(), srcImg.Height.ToString());
        //                    }
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                var inputFile = new MediaFile { Filename = fi.FullName };
        //                using (var engine = new Engine()) { engine.GetMetadata(inputFile); }

        //                if (inputFile.Metadata == null) continue;
        //                if (inputFile.Metadata.VideoData == null) continue;
        //                var size = inputFile.Metadata.VideoData.FrameSize.Split('x');
        //                file = new FileObject(fi.Name, size.First(), size.Last());
        //            }

        //            folderObject.Files.Add(file);
        //        }
        //    }

        //    var subDirs = root.GetDirectories();
        //    if (subDirs.Any())
        //        foreach (var dirInfo in subDirs)
        //            folderObject.Folders.Add(WalkDirectoryTree(new FolderObject(), dirInfo));

        //    return folderObject;
        //}

        //private static FileObject GetFile(FolderObject flatTree, string fName, string dName)
        //{
        //    if (flatTree.Files.Any())
        //    {
        //        foreach (var file in flatTree.Files)
        //            if (file.OriginName == fName && file.UpdatedName == null && flatTree.Name == dName)
        //                return file;
        //    }

        //    foreach (var sub in flatTree.Folders)
        //        foreach (var file in sub.Files)
        //            if (file.OriginName == fName && file.UpdatedName == null && sub.Name == dName)
        //                return file;

        //    return null;
        //}
    }
}
