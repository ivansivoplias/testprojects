using System;
using System.Text;
using System.Threading;

namespace InformationFinder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.GetEncoding("windows-1251");
            bool exit = false;
            string source = null;
            InfoExtractor extractor = null;
            SearchObserver observer = SearchObserver.Create();

            do
            {
                Console.WriteLine("Input path to file or url to start searching...");
                source = Console.ReadLine();

                source = source.Trim().Trim('\"', '\'');

                if (source.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                {
                    exit = true;
                }

                if (!exit)
                {
                    extractor = null;

                    try
                    {
                        extractor = InfoExtractor.Create(source);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Something gone wrong while creating searcher object. See details below: \n{0}", e.Message);
                    }

                    Console.WriteLine();
                    if (extractor != null)
                    {
                        extractor.MatchFounded += observer.OnMatchFounded;

                        var emailsThread = new Thread(extractor.SearchEmails);
                        emailsThread.Start();

                        var linksThread = new Thread(extractor.SearchLinks);
                        linksThread.Start();

                        emailsThread.Join();
                        linksThread.Join();

                        extractor.MatchFounded -= observer.OnMatchFounded;
                    }
                }
            }
            while (!exit);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}