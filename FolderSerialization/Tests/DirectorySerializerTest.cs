using NUnit.Framework;
using SerializationTask;
using System;
using System.IO;
using System.Reflection;

namespace Tests
{
    [TestFixture]
    public class DirectorySerializerTest
    {
        private FolderInfo _testDirectory;
        public string testFolderName;
        public string testOutputFile;

        [OneTimeSetUp]
        public void Setup()
        {
            var directoryName = GetCurrentDirectory() + "\\";

            testFolderName = directoryName + "TestRoot";
            testOutputFile = directoryName + "testOutputFile.xml";

            _testDirectory = new FolderInfo() { DirectoryName = "TestRoot", FullPath = testFolderName };

            Directory.CreateDirectory(testFolderName);

            for (int i = 0; i < 5; i++)
            {
                string dirName = FormatValues("folder #{0}", (i + 1).ToString());
                string dirPath = FormatValues("{0}\\{1}", testFolderName, dirName);

                var folder = new FolderInfo() { DirectoryName = dirName, FullPath = dirPath };
                Directory.CreateDirectory(dirPath);
                _testDirectory.AddChild(folder);
            }
        }

        [Test]
        public void TestDeserializer()
        {
            //Arrange
            var serializer = DirectorySerializer.Create(testFolderName, testOutputFile, "xml");
            serializer.SerializeDirectory();

            //Act
            serializer = DirectorySerializer.Create(testOutputFile);
            FolderInfo tree = serializer.DeserializeDirectory();

            //Assert
            Assert.IsTrue(_testDirectory.Equals(tree));
        }

        public string FormatValues(string format, params string[] args)
        {
            return string.Format(format, args);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            try
            {
                Directory.Delete(testFolderName, true);
                File.Delete(testOutputFile);
            }
            catch
            {
                return;
            }
        }

        public string GetCurrentDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}