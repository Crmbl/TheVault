using System.Collections.Generic;
using System.IO;

namespace TheVault.Utils
{
    public static class FileUtil
    {
        public static readonly string[] FileExtensions =
        {
            /*Video*/
            ".webm", ".mkv", ".ogg", ".ogv", ".avi", ".wmv", ".mp4", ".mpg", ".mpeg", ".m2v", ".m4v", ".3gp", ".flv",
            /*Image*/
            ".gif", ".jpg", ".jpeg", ".bmp", ".png",
            /*Audio*/
            ".flac", ".m4a", ".mp3", ".wav", ".wma",
            /*Other*/
            ".json"
        };

        public static void GetFileExtensions(DirectoryInfo dir, string outputPath)
        {
            var files = dir.EnumerateFiles("*", SearchOption.AllDirectories);
            var extList = new List<string>();
            foreach (var file in files)
            {
                if (extList.Contains(file.Extension)) continue;
                extList.Add(file.Extension);
            }
            
            File.AppendAllLines(outputPath, extList);
        }
    }
}