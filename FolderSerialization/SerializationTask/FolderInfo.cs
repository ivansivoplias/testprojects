using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SerializationTask
{
    [Serializable]
    public class FolderInfo : IEquatable<FolderInfo>
    {
        private int _indent;
        private FolderInfo _parent;
        private List<FolderInfo> _directories;
        private bool _isValid;

        public string DirectoryName { get; set; }
        public string FullPath { get; set; }
        public DateTime CreationDate { get; set; }
        public List<FileData> Files { get; set; }

        public bool IsRoot => _parent == null;

        public List<FolderInfo> Directories
        {
            get { return _directories; }
            set
            {
                _directories = value;
                if (!_isValid) FixTreeLinks(this);
            }
        }

        public FolderInfo()
        {
            Directories = new List<FolderInfo>();
            _isValid = false;
            _indent = 0;
        }

        public FolderInfo(string directoryName, string fullPath, DateTime creationDate, List<FileData> files, params FolderInfo[] children)
        {
            DirectoryName = directoryName;
            _isValid = false;
            CreationDate = creationDate;
            FullPath = fullPath;
            Files = files;

            if (children != null)
            {
                foreach (var child in children)
                {
                    child._parent = this;
                }

                var list = new List<FolderInfo>();
                list.AddRange(children);
                Directories = list;
            }
        }

        public FolderInfo AddChild(FolderInfo nodeData)
        {
            Directories.Add(nodeData);
            nodeData._parent = this;
            return nodeData;
        }

        public static void FixTreeLinks(FolderInfo folder)
        {
            if (folder != null)
            {
                if (folder.Directories != null)
                {
                    foreach (var child in folder.Directories)
                    {
                        child._parent = folder;
                        child._indent = 0;
                        FixTreeLinks(child);
                    }
                }
            }
            folder._isValid = true;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            var indentStr = GetIndentString(_indent);
            result.AppendFormat(indentStr + "\\{0} | {1}{2}", DirectoryName, CreationDate, Environment.NewLine);
            if (Directories != null)
            {
                _indent++;
                foreach (var child in Directories)
                {
                    child._indent = _indent;
                    result.Append(child.ToString());
                }
            }

            var fileIndent = GetIndentString(_indent + 1);

            if (Files != null)
            {
                foreach (var file in Files)
                {
                    result.AppendLine(fileIndent + file);
                }
            }

            result.Append(Environment.NewLine);

            return result.ToString();
        }

        private static string GetIndentString(int indent)
        {
            return new string(' ', 2 * indent);
        }

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj is FolderInfo)
            {
                var other = obj as FolderInfo;
                return Equals(other);
            }

            return false;
        }

        public bool Equals(FolderInfo other)
        {
            var otherPath = other.FullPath;

            bool namesEqual = FullPath.Equals(otherPath);
            bool filesEqual = FilesEqual(other);

            bool childrenEqual = false;

            if (Directories != null)
            {
                if (other.Directories != null)
                {
                    if (Directories.Count == other.Directories.Count)
                    {
                        childrenEqual = true;
                        for (int i = 0; i < Directories.Count; i++)
                        {
                            var child = Directories[i];
                            var otherChild = other.Directories.Find(z => z.DirectoryName == child.DirectoryName);

                            if (otherChild != null)
                            {
                                childrenEqual = child.Equals(otherChild);
                            }
                            else
                            {
                                childrenEqual = false;
                            }

                            if (!childrenEqual)
                            {
                                break;
                            }
                        }
                    }
                }
                else if (Directories.Count == 0)
                {
                    childrenEqual = true;
                }
            }
            else if (other.Directories?.Count == 0)
            {
                childrenEqual = true;
            }

            return namesEqual && filesEqual && childrenEqual;
        }

        private bool FilesEqual(FolderInfo other)
        {
            bool result = false;
            if (Files != null)
            {
                if (other.Files != null)
                {
                    if (Files.Count == other.Files.Count)
                    {
                        result = true;
                        for (int i = 0; i < Files.Count; i++)
                        {
                            var first = Files[i];
                            var second = other.Files.Find(z => z.FileName == first.FileName);

                            result = first.Equals(second);

                            if (!result) break;
                        }
                    }
                }
                else if (Files.Count == 0)
                {
                    result = true;
                }
            }
            else if (other.Files?.Count == 0)
            {
                result = true;
            }

            return result;
        }
    }
}