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
            InfoExtractorBuilder builder;
            InfoExtractor extractor = null;
            SearchObserver observer;

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
                    builder = new InfoExtractorBuilder();
                    observer = SearchObserver.Create();
                    extractor = null;

                    try
                    {
                        extractor = builder.SetSource(source)
                            .DetectSourceType()
                            .SetObservers(observer.OnMatchFounded)
                            .Build();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Something gone wrong while creating searcher object. See details below: \n{0}", e.Message);
                    }

                    Console.WriteLine();
                    if (extractor != null)
                    {
                        var emailsThread = new Thread(extractor.SearchEmails);
                        emailsThread.Start();

                        var linksThread = new Thread(extractor.SearchLinks);
                        linksThread.Start();

                        emailsThread.Join();
                        linksThread.Join();
                    }
                }
            }
            while (!exit);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}