using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace InformationFinder
{
    public class InfoExtractorBuilder
    {
        private string _source;
        private readonly List<EventHandler<MatchFondedEventArgs>> _observers;
        private static readonly Regex _link = new Regex("(https?|ftp)://[^\\s/$.?#].[^\\s]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string _data;

        public InfoExtractorBuilder()
        {
            _observers = new List<EventHandler<MatchFondedEventArgs>>();
            _data = string.Empty;
        }

        public InfoExtractorBuilder SetSource(string source)
        {
            _source = source;
            return this;
        }

        public InfoExtractorBuilder DetectSourceType()
        {
            if (File.Exists(_source))
            {
                _data = ReadFromFile();
            }
            else if (_link.IsMatch(_source))
            {
                _data = ReadFromUrl();
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid source {0} passed. Please input write source(file or url) and try again.", _source));
            }

            return this;
        }

        private string ReadFromUrl()
        {
            string result = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_source);
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

        private string ReadFromFile()
        {
            string line = string.Empty;
            try
            {
                line = File.ReadAllText(_source);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened while reading from file: {0}. See details below: \n{1}", _source, e.Message);
            }

            return line;
        }

        public InfoExtractorBuilder SetObservers(params EventHandler<MatchFondedEventArgs>[] observers)
        {
            if (observers != null)
            {
                _observers.AddRange(observers);
            }
            return this;
        }

        public InfoExtractor Build() => InfoExtractor.Create(_source, _data, _observers);
    }
}