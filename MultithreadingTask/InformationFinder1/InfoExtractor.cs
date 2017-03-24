using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace InformationFinder
{
    public class InfoExtractor
    {
        private readonly string _sourceAddress;
        private bool _isSearching;
        private static readonly Regex _emailRegex = new Regex("[a-zA-Z0-9\\.\\-_]+@([a-z0-9\\-]\\.?)+\\.([a-z0-9\\-])+", RegexOptions.Compiled);
        private static readonly Regex _htmlLink = new Regex("(?<=([\"']))((?<=href=['\"])|(?<=src=['\"])).*?(?=\\1)", RegexOptions.Compiled);

        private readonly bool _inFile;

        private EventHandler<MatchFondedEventArgs> _matchFounded;
        private readonly object _lockObject = new object();

        public static Regex Link { get; } = new Regex("(https?|ftp)://[^\\s/$.?#].[^\\s]*", RegexOptions.Compiled);

        public bool IsSearching => _isSearching;

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

        public InfoExtractor(string source, bool? inFile, List<EventHandler<MatchFondedEventArgs>> observers)
        {
            if (inFile == null || !inFile.HasValue)
            {
                throw new ArgumentException("Invalid source path passed");
            }

            _sourceAddress = source;
            _inFile = inFile.Value;
            _isSearching = true;

            foreach (var observer in observers)
            {
                MatchFounded += observer;
            }
        }

        public void Search()
        {
            if (_inFile)
            {
                SearchFile();
            }
            else
            {
                SearchUrl();
            }
        }

        private void SearchUrl()
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(_sourceAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad gone with request to {0}. See details below: \n{1}", _sourceAddress, e.Message);
                _isSearching = false;
                return;
            }
            Stream responseStream = null;
            try
            {
                responseStream = request.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while getting a response from: {0}. See details below: \n{1}", _sourceAddress, e.Message);
                _isSearching = false;
                return;
            }

            string strResponse = null;
            try
            {
                strResponse = new StreamReader(responseStream).ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while reading response from: {0}. See details below: \n{1}", _sourceAddress, e.Message);
                _isSearching = false;
                return;
            }

            var lines = strResponse.Split('\n');
            SearchInLines(lines);
        }

        private void SearchFile()
        {
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(_sourceAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while reading from file: {0}. See details below: \n{1}", _sourceAddress, e.Message);
                _isSearching = false;
                return;
            }
            SearchInLines(lines);
        }

        private void SearchInLines(string[] lines)
        {
            foreach (var line in lines)
            {
                var emailMatches = _emailRegex.Matches(line);
                var linkMatches = _htmlLink.Matches(line);
                if (emailMatches.Count > 0)
                {
                    foreach (Match match in emailMatches)
                    {
                        _matchFounded(this, new MatchFondedEventArgs(_sourceAddress, match.Value, MatchType.Email));
                    }
                }

                if (linkMatches.Count > 0)
                {
                    foreach (Match match in linkMatches)
                    {
                        _matchFounded(this, new MatchFondedEventArgs(_sourceAddress, match.Value, MatchType.Link));
                    }
                }
            }

            _isSearching = false;
        }
    }
}