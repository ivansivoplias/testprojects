using System;

namespace SerializationTask
{
    [Serializable]
    public class FileData : IEquatable<FileData>
    {
        public const double STEP_SIZE = 1024;

        public FileSize FileSizeFormat { get; set; }
        public string FileName { get; set; }
        public DateTime CreationDate { get; set; }
        public double Size { get; set; }
        public string Attributes { get; set; }

        public FileData()
        {
        }

        public FileData(string filename, DateTime creationDate, double size, string attributes)
        {
            FileName = filename;
            CreationDate = creationDate;

            Size = DetectSize(size);

            Attributes = attributes;
        }

        private double DetectSize(double size)
        {
            var result = size;
            var power = 0;

            while (result >= STEP_SIZE)
            {
                result /= STEP_SIZE;
                power++;
            }

            FileSizeFormat = (FileSize)power;

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0} | {1} | {2:f2} {3} | {4}", FileName, CreationDate, Size, FileSizeFormat, Attributes);
        }

        public bool Equals(FileData other)
        {
            return FileName.Equals(other.FileName);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj is FileData)
            {
                var other = obj as FileData;
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}