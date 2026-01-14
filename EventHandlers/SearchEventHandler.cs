namespace SimplePad.EventHandlers
{
    public delegate void SearchEventHandler(object sender, SearchEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class SearchEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string SearchText { get; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string ReplaceText { get; } = "";

        /// <summary>
        /// 
        /// </summary>
        public bool MatchCase { get; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="replaceText"></param>
        /// <param name="matchCase"></param>
        public SearchEventArgs(string searchText, bool matchCase)
        {
            SearchText = searchText;
            MatchCase = matchCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="replaceText"></param>
        /// <param name="matchCase"></param>
        public SearchEventArgs(string searchText, string replaceText, bool matchCase)
        {
            SearchText = searchText;
            ReplaceText = replaceText;
            MatchCase = matchCase;
        }
    }
}
