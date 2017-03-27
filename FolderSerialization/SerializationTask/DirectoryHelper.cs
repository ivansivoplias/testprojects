using System;
using System.Collections.Generic;
using System.IO;

namespace SerializationTask
{
    public static class DirectoryHelper
    {
        public static FolderInfo GetDirectoryTree(string directoryPath)
        {
            FolderInfo root = null;
            DirectoryInfo rootDir = null;
            try
            {
                rootDir = new DirectoryInfo(directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurs while serializing \"{0}\". Details: {1}", directoryPath, e.Message);
            }

            if (rootDir != null)
            {
                root = new FolderInfo(rootDir.Name, rootDir.FullName, rootDir.CreationTime, null);
                root.Files = GetFiles(rootDir);

                GetDirectoryTree(root, rootDir);
            }

            return root;
        }

        private static void GetDirectoryTree(FolderInfo tree, DirectoryInfo dirInfo)
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