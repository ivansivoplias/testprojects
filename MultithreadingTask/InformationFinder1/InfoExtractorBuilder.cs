using System;
using System.Collections.Generic;
using System.IO;

namespace InformationFinder
{
    public class InfoExtractorBuilder
    {
        private string _source;
        private bool? _inFile;
        private readonly List<EventHandler<MatchFondedEventArgs>> _observers;

        public InfoExtractorBuilder()
        {
            _inFile = null;
            _observers = new List<EventHandler<MatchFondedEventArgs>>();
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
                _inFile = true;
            }
            else
            {
                _inFile &= !InfoExtractor.Link.IsMatch(_source);
            }

            return this;
        }

        public InfoExtractorBuilder SetObservers(params EventHandler<MatchFondedEventArgs>[] observers)
        {
            if (observers != null)
            {
                _observers.AddRange(observers);
            }
            return this;
        }

        public InfoExtractor Build() => new InfoExtractor(_source, _inFile, _observers);
    }
}