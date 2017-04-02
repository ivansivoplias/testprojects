﻿using System;
using System.Text;
using System.Threading;

namespace InformationFinder
{
    public class Program
    {
        private static readonly string _format = "{0} {1} is founded in {2} at {3}";

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.GetEncoding("windows-1251");
            bool exit = false;
            string source = null;
            InfoExtractor extractor = null;

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
                        extractor.MatchFounded += OnMatchFounded;

                        var emailsThread = new Thread(extractor.SearchEmails);
                        emailsThread.Start();

                        var linksThread = new Thread(extractor.SearchLinks);
                        linksThread.Start();

                        emailsThread.Join();
                        linksThread.Join();

                        extractor.MatchFounded -= OnMatchFounded;
                    }
                }
            }
            while (!exit);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void OnMatchFounded(object sender, MatchFondedEventArgs e)
        {
            var founded = e.Founded;
            if (founded.Length > 80)
            {
                founded = string.Format("{0} ... ", founded.Substring(0, 80));
            }
            Console.WriteLine(_format, e.Match, founded, e.Source, DateTime.Now.ToShortDateString());
            Console.WriteLine();
        }
    }
}