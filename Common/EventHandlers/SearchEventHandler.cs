namespace SimplePad.EventHandlers
{
    public delegate void SearchEventHandler(object sender, SearchEventArgs e);

    /// <summary>
    /// Event arguments for search-related events, containing information about the search and replace operations.
    /// </summary>
    public class SearchEventArgs
    {
        /// <summary>
        /// Gets the text to search for.
        /// </summary>
        public string SearchText { get; } = "";

        /// <summary>
        /// Gets the text to replace with.
        /// </summary>
        public string ReplaceText { get; } = "";

        /// <summary>
        /// Gets a value indicating whether the search should be case-sensitive.
        /// </summary>
        public bool MatchCase { get; } = false;

        /// <summary>
        /// Gets a value indicating whether the search should match whole words only.
        /// </summary>
        public bool MatchWholeWords { get; } = false;

        /// <summary>
        /// Initializes a new instance of the SearchEventArgs class for search operations.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="replaceText"></param>
        /// <param name="matchCase"></param>
        public SearchEventArgs(string searchText, bool matchCase, bool matchWholeWords)
        {
            SearchText = searchText;
            MatchCase = matchCase;
            MatchWholeWords = matchWholeWords;
        }

        /// <summary>
        /// Initializes a new instance of the SearchEventArgs class for search and replace operations.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="replaceText"></param>
        /// <param name="matchCase"></param>
        public SearchEventArgs(string searchText, string replaceText, bool matchCase, bool matchWholeWords)
        {
            SearchText = searchText;
            ReplaceText = replaceText;
            MatchCase = matchCase;
            MatchWholeWords = matchWholeWords;
        }
    }
}
