using System;

namespace InformationFinder
{
    public class MatchFondedEventArgs : EventArgs
    {
        public string Source { get; }

        public string Founded { get; }

        public string Match { get; }

        public MatchFondedEventArgs(string source, string founded, string match)
        {
            Source = source;
            Founded = founded;
            Match = match;
        }
    }
}