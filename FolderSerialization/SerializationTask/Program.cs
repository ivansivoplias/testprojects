using System;
using System.Text;

namespace SerializationTask
{
    public class Program
    {
        public const string WRONG_ARGUMENTS_TEXT = "Wrong using of program. Please run it again with proper console arguments.\nProgram works with several type of parameters input:\n1) \"folder-path\" \"output filename\" \"format(xml or binary)\";\n2)\"filename with serialized info\"\n";
        public const string EXAMPLES = "Examples:\n1)\"E:\\\\Projects\" (\"output.bin\" or \"output.xml\") (\"bin\" or \"xml\")\n2)\"output.bin\"";

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (args != null)
            {
                if (args.Length == 3)
                {
                    var dirPath = args[0];
                    var filename = args[1];
                    var format = args[2];

                    try
                    {
                        var dirSerializer = DirectorySerializer.Create(dirPath, filename, format);
                        dirSerializer.SerializeDirectory();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Something gone wrong. Please read hint and try again. " + e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Something gone wrong. Details displayed here: " + e.Message);
                    }
                }
                else if (args.Length == 1)
                {
                    var filePath = args[0];

                    try
                    {
                        var deserializer = DirectorySerializer.Create(filePath);
                        var root = deserializer.DeserializeDirectory();
                        //FolderInfo.FixTreeLinks(root);
                        Console.WriteLine(root);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Something gone wrong while deserialization. Details displayed here: " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine(WRONG_ARGUMENTS_TEXT + EXAMPLES);
                }
            }
            else
            {
                Console.WriteLine(WRONG_ARGUMENTS_TEXT + EXAMPLES);
            }
            Console.WriteLine("Press any key to exit a program....");
            Console.ReadKey();
        }
    }
}