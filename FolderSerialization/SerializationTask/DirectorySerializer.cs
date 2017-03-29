using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace SerializationTask
{
    public class DirectorySerializer
    {
        private readonly string _directoryPath;
        private readonly string _outputFileName;
        private readonly SerializationFormat _format;

        private DirectorySerializer()
        {
        }

        private DirectorySerializer(string directoryPath, string outputFileName, SerializationFormat format)
        {
            _format = format;
            _directoryPath = directoryPath;
            _outputFileName = outputFileName;
        }

        private DirectorySerializer(string filePath, SerializationFormat format)
        {
            _outputFileName = filePath;
            _format = format;
        }

        public static DirectorySerializer Create(string directoryPath, string outputFileName, string format)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Invalid directory path. Please close program assign right directory path and try run it again.");
            }

            if (!CheckFileNameWithFormat(outputFileName, format))
            {
                throw new ArgumentException("Invalid output filename. Please close program assign right output filename and try run it again.");
            }

            SerializationFormat serializationFormat = DirectoryHelper.GetFormatFromString(format);

            return new DirectorySerializer(directoryPath, outputFileName, serializationFormat);
        }

        public static DirectorySerializer Create(string filePath)
        {
            SerializationFormat format;
            if (CheckFileName(filePath) && File.Exists(filePath))
            {
                var extention = Path.GetExtension(filePath).Replace(".", "");
                format = DirectoryHelper.GetFormatFromString(extention);
            }
            else
            {
                throw new ArgumentException("File path is invalid.");
            }

            return new DirectorySerializer(filePath, format);
        }

        public FolderInfo DeserializeDirectory()
        {
            FolderInfo root = null;
            if (_format == SerializationFormat.XML)
            {
                var xmlSerializer = new XmlSerializer(typeof(FolderInfo));
                using (var stream = new FileStream(_outputFileName, FileMode.Open))
                {
                    root = (FolderInfo)xmlSerializer.Deserialize(stream);
                }
            }
            else
            {
                var binFormatter = new BinaryFormatter();
                using (var stream = new FileStream(_outputFileName, FileMode.Open))
                {
                    root = (FolderInfo)binFormatter.Deserialize(stream);
                }
            }
            return root;
        }

        public void SerializeDirectory()
        {
            FolderInfo root = DirectoryHelper.GetDirectoryTree(_directoryPath);

            if (_format == SerializationFormat.XML)
            {
                var xmlSerializer = new XmlSerializer(typeof(FolderInfo));
                using (var stream = new StreamWriter(_outputFileName, false))
                {
                    xmlSerializer.Serialize(stream, root);
                }
            }
            else
            {
                var binSerializer = new BinaryFormatter();
                using (var stream = new FileStream(_outputFileName, FileMode.OpenOrCreate))
                {
                    binSerializer.Serialize(stream, root);
                }
            }
            Console.WriteLine("Serialization successful!");
        }

        private static bool CheckFileNameWithFormat(string fileName, string format)
        {
            bool result = false;
            bool fileChars = CheckFileName(fileName);

            string extention = Path.GetExtension(fileName);

            if (extention.Length > 0)
            {
                string formatFromExt = extention.Substring(1);
                if (formatFromExt.Equals(format, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = fileChars;
                }
            }
            return result;
        }

        private static bool CheckFileName(string fileName)
        {
            var nameOfFile = Path.GetFileName(fileName);
            var forbidden = Path.GetInvalidFileNameChars();
            bool result = true;

            for (int i = 0; i < nameOfFile.Length; i++)
            {
                if (forbidden.Contains(nameOfFile[i]))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}