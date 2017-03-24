using System;

namespace InformationFinder
{
    public class MatchFondedEventArgs : EventArgs
    {
        public string Source { get; }

        public string Founded { get; }

        public MatchType Match { get; }

        public MatchFondedEventArgs(string source, string founded, MatchType match)
        {
            Source = source;
            Founded = founded;
            Match = match;
        }
    }
}