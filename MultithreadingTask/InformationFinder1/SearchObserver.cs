using System;

namespace InformationFinder
{
    public class SearchObserver
    {
        private static readonly string _format = "{0} {1} is founded in {2} at {3}";

        private SearchObserver()
        {
        }

        public static SearchObserver Create() => new SearchObserver();

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