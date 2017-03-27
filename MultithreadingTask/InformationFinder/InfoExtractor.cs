using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace InformationFinder
{
    public class InfoExtractor
    {
        private readonly string _sourceAddress;
        private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private static readonly Regex _emailRegex = new Regex("[a-zA-Z0-9\\.\\-_]+@([a-z0-9\\-]\\.?)+\\.([a-z0-9\\-])+", Options);
        private static readonly Regex _htmlLink = new Regex(@"((http|ftp|https):\/\/|www\.|\/)([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", Options);
        private readonly object _lockObject = new object();

        private readonly string _line;

        private EventHandler<MatchFondedEventArgs> _matchFounded;

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

        private InfoExtractor(string source, string data, List<EventHandler<MatchFondedEventArgs>> observers)
        {
            _sourceAddress = source;
            _line = data;

            foreach (var observer in observers)
            {
                MatchFounded += observer;
            }
        }

        public static InfoExtractor Create(string source, string data, List<EventHandler<MatchFondedEventArgs>> observers)
        {
            return new InfoExtractor(source, data, observers);
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
                Match match = regex.Match(line);
                while (match.Success)
                {
                    _matchFounded(this, new MatchFondedEventArgs(_sourceAddress, match.Value, type));
                    match = match.NextMatch();
                    Thread.Sleep(1);
                }
            }
            else
            {
                Console.WriteLine("Source is empty or reading was not successful. Please input new source and try again.");
            }
        }
    }
}