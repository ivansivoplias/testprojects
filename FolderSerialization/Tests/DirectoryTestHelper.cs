using SerializationTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileInfo = SerializationTask.FileInfo;

namespace Tests
{
    public class DirectoryTestHelper
    {
        private static Random random = new Random();

        public static TreeNode<FolderInfo> GenerateTree()
        {
            string drive;
            var drives = DriveInfo.GetDrives().Where(x => x.IsReady && x.DriveType == DriveType.Fixed).ToArray();

            if (drives.Length > 1) drive = drives[1].Name;
            else
            {
                drive = drives[0].Name;
            }

            string rootName = "TestsRoot";
            string randomFullPath = $"{drive}{rootName}";

            if (Directory.Exists(randomFullPath))
            {
                Directory.Delete(randomFullPath, true);
            }

            FolderInfo folder = new FolderInfo();
            folder.DirectoryName = rootName;
            folder.FullPath = randomFullPath;

            TreeNode<FolderInfo> root = new TreeNode<FolderInfo>(folder);

            GenerateFolders(root);

            return root;
        }

        private static TreeNode<FolderInfo> GenerateFolders(TreeNode<FolderInfo> directory, int maxLevel = 5, int level = 0)
        {
            if (maxLevel > 0 && level < maxLevel)
            {
                int subFoldersCount = random.Next(1, 5);
                for (int i = 0; i < subFoldersCount; i++)
                {
                    var dir = new FolderInfo();
                    dir.DirectoryName = GetRandomFolderName();
                    dir.FullPath = $"{directory.Value.FullPath}\\{dir.DirectoryName}";

                    var folder = directory.AddChild(dir);

                    int choice = random.Next(25, 100);
                    if (choice < 50)
                    {
                        directory = GenerateFiles(directory);
                    }
                    else
                    {
                        folder = GenerateFolders(folder, maxLevel - (level + 1), level + 1);
                    }
                }
            }
            return directory;
        }

        private static string GetRandomFolderName()
        {
            var guid = Guid.NewGuid().ToString();
            return "TestFolder" + random.Next(0, 5000000) + guid.Substring(0, random.Next(guid.Length / 2));
        }

        private static string GetRandomFileName()
        {
            var guid = Guid.NewGuid().ToString();
            return "testfile" + random.Next(1212, 15000) + guid.Substring(0, random.Next(guid.Length / 2)) + ".txt";
        }

        private static TreeNode<FolderInfo> GenerateFiles(TreeNode<FolderInfo> folder)
        {
            int filesCount = random.Next(0, 10);
            var files = new List<FileInfo>(filesCount);

            for (int i = 0; i < filesCount; i++)
            {
                var fileName = GetRandomFileName();
                var file = GenerateFile(fileName);
                files.Add(file);
            }

            folder.Value.Files = files;
            return folder;
        }

        private static FileInfo GenerateFile(string fileName)
        {
            var file = new FileInfo();
            file.FileName = fileName;
            return file;
        }

        private static void GenerateFiles(string folderPath, List<FileInfo> files)
        {
            foreach (var file in files)
            {
                GenerateFile(folderPath + "\\", file);
            }
        }

        private static void GenerateFile(string folderPath, FileInfo file)
        {
            using (var fileStream = new StreamWriter(folderPath + file.FileName, false, Encoding.UTF8))
            {
                int randomCount = random.Next(50);
                for (int i = 0; i < randomCount; i++)
                {
                    fileStream.Write("kjsdfkjsdklfjklsdf");
                }
            }
        }

        public static void GenerateDirectories(TreeNode<FolderInfo> tree)
        {
            GenerateFolder(tree.Value);
            if (tree.Children != null)
            {
                foreach (var folder in tree.Children)
                {
                    GenerateDirectories(folder);
                }
            }
        }

        private static void GenerateFolder(FolderInfo folder)
        {
            Directory.CreateDirectory(folder.FullPath);
            if (folder.Files != null)
            {
                GenerateFiles(folder.FullPath, folder.Files);
            }
        }
    }
}