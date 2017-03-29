using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace InformationFinder
{
    public class InfoExtractor
    {
        public const string LinkMatchType = "Link";
        public const string EmailMatchType = "E-mail";
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        private static readonly Regex _emailRegex = new Regex("[a-zA-Z0-9\\.\\-_]+@([a-z0-9\\-]\\.?)+\\.([a-z0-9\\-])+", DefaultOptions);
        private static readonly Regex _htmlLink = new Regex(@"((http|ftp|https):\/\/|www\.|\/)([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", DefaultOptions);
        private static readonly Regex _link = new Regex("(https?|ftp)://[^\\s/$.?#].[^\\s]*", DefaultOptions);

        private static readonly string _format = "{0} {1} is founded in {2} at {3}";

        private readonly object _lockObject = new object();
        private readonly string _sourceAddress;
        private readonly string _sourceDataString;

        public EventHandler<MatchFondedEventArgs> MatchFounded;

        private InfoExtractor(string source, string sourceData)
        {
            _sourceAddress = source;
            _sourceDataString = sourceData;
        }

        public static InfoExtractor Create(string source)
        {
            string resultStr;
            if (File.Exists(source))
            {
                resultStr = ReadFromFile(source);
            }
            else if (_link.IsMatch(source))
            {
                resultStr = ReadFromUrl(source);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid source {0} passed. Please input write source(file or url) and try again.", source));
            }

            return new InfoExtractor(source, resultStr);
        }

        public void SearchEmails()
        {
            SearchInLine(_sourceDataString, _emailRegex, EmailMatchType);
        }

        public void SearchLinks()
        {
            SearchInLine(_sourceDataString, _htmlLink, LinkMatchType);
        }

        private static string ReadFromFile(string source)
        {
            string line = string.Empty;
            try
            {
                line = File.ReadAllText(source);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while reading from file: {0}. See details below: \n{1}", source, e.Message);
            }

            return line;
        }

        private static string ReadFromUrl(string source)
        {
            string result = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(source);
            try
            {
                using (Stream responseStream = request.GetResponse().GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something gone wrong while reading from url.\nSee details below: \n{0}", e.Message);
            }

            return result;
        }

        private void SearchInLine(string line, Regex regex, string type)
        {
            if (!string.IsNullOrEmpty(line))
            {
                Match match = regex.Match(line);
                while (match.Success)
                {
                    MatchFounded(this, new MatchFondedEventArgs(_sourceAddress, match.Value, type));
                    match = match.NextMatch();
                    Thread.Sleep(1);
                }
            }
            else
            {
                Console.WriteLine("Source is empty or reading was not successful. Please input new source and try again.");
            }
        }

        public void OnMatchFounded(object sender, MatchFondedEventArgs e)
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