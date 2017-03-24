using System;
using System.Collections.Generic;
using System.IO;

namespace SerializationTask
{
    public static class DirectoryHelper
    {
        public static void GetDirectoryTree(FolderInfo tree, DirectoryInfo dirInfo)
        {
            DirectoryInfo[] directories = null;
            try
            {
                directories = dirInfo.GetDirectories();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurs while serialization. Details: {0}", e.Message);
                return;
            }

            foreach (var directory in directories)
            {
                var directoryInfo = new FolderInfo(directory.Name, directory.FullName, directory.CreationTime, null);
                try
                {
                    directoryInfo.Files = GetFiles(directory);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Files in folder {0} cannot be serialized. \nMore information here: {1}", dirInfo.FullName, e.Message);
                }

                var treeNode = tree.AddChild(directoryInfo);
                GetDirectoryTree(treeNode, directory);
            }
        }

        public static List<FileData> GetFiles(DirectoryInfo directory)
        {
            var list = new List<FileData>();
            foreach (var fileInfo in directory.GetFiles())
            {
                var file = new FileData(fileInfo.Name, fileInfo.CreationTime, fileInfo.Length, fileInfo.Attributes.ToString());
                list.Add(file);
            }
            return list;
        }

        public static SerializationFormat GetFormatFromString(string format)
        {
            SerializationFormat result;
            var isXml = Enum.TryParse(format, true, out result);
            var isBin = Enum.TryParse(format, true, out result);

            if (!isXml && !isBin)
            {
                throw new ArgumentException("Invalid output format. Please close program and pass right format(bin or xml) as console argument.");
            }
            return result;
        }
    }
}