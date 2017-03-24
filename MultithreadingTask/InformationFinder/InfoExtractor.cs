using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace InformationFinder
{
    public class InfoExtractor
    {
        private readonly string _sourceAddress;
        private static readonly Regex _emailRegex = new Regex("[a-zA-Z0-9\\.\\-_]+@([a-z0-9\\-]\\.?)+\\.([a-z0-9\\-])+", RegexOptions.Compiled);
        private static readonly Regex _htmlLink = new Regex(@"((http|ftp|https):\/\/|www\.|\/)([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);
        private static Regex _link = new Regex("(https?|ftp)://[^\\s/$.?#].[^\\s]*", RegexOptions.Compiled);
        private readonly object _lockObject = new object();

        private readonly bool _inFile;

        private readonly string _line;

        private EventHandler<MatchFondedEventArgs> _matchFounded;

        public static Regex Link => _link;

        public event EventHandler<MatchFondedEventArgs> MatchFounded
        {
            add
            {
                lock (_lockObject)
                {
                    _matchFounded += value;
                }
            }
            remove
            {
                lock (_lockObject)
                {
                    if (_matchFounded != null)
                    {
                        _matchFounded -= value;
                    }
                }
            }
        }

        private InfoExtractor(string source, bool inFile, List<EventHandler<MatchFondedEventArgs>> observers)
        {
            _sourceAddress = source;
            _inFile = inFile;

            if (_inFile)
            {
                _line = SearchFile();
            }
            else
            {
                _line = SearchUrl();
            }

            foreach (var observer in observers)
            {
                MatchFounded += observer;
            }
        }

        public static InfoExtractor Create(string source, bool? inFile, List<EventHandler<MatchFondedEventArgs>> observers)
        {
            if (inFile == null || !inFile.HasValue)
            {
                throw new ArgumentException("Invalid source path passed");
            }

            bool inFileSearch = inFile.Value;
            return new InfoExtractor(source, inFileSearch, observers);
        }

        private string SearchUrl()
        {
            string result = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sourceAddress);
                Stream responseStream = request.GetResponse().GetResponseStream();
                result = new StreamReader(responseStream).ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while reading from url {0}. See details below: \n{1}", _sourceAddress, e.Message);
            }

            return result;
        }

        private string SearchFile()
        {
            string line = null;
            try
            {
                line = File.ReadAllText(_sourceAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while reading from file: {0}. See details below: \n{1}", _sourceAddress, e.Message);
            }

            return line;
        }

        public void SearchEmails()
        {
            SearchInLines(_line, _emailRegex, MatchType.Email);
        }

        public void SearchLinks()
        {
            SearchInLines(_line, _htmlLink, MatchType.Link);
        }

        private void SearchInLines(string line, Regex regex, MatchType type)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var matches = regex.Matches(line);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        _matchFounded(this, new MatchFondedEventArgs(_sourceAddress, match.Value, type));
                        Thread.Sleep(1);
                    }
                }
            }
            else
            {
                Console.WriteLine("Source is empty or reading was not successful. Please input new source and try again.");
            }
        }
    }
}