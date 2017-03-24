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
            bool isSearching = false;
            string source = null;
            InfoExtractorBuilder builder;
            InfoExtractor extractor = null;
            SearchObserver observer;

            do
            {
                if (!isSearching)
                {
                    Console.WriteLine("Input path to file or url to start searching...");
                    source = Console.ReadLine();

                    source = source.Trim().Trim('\"', '\'');

                    if (source.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        exit = true;
                    }
                }

                if (!exit && !isSearching)
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
                        var thread = new Thread(extractor.Search);
                        thread.Start();
                    }
                }

                if (extractor != null)
                {
                    isSearching = extractor.IsSearching;
                }
            }
            while (!exit);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}