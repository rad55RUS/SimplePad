using SimplePad.Views;

namespace SimplePad.Services
{
    public static class WindowService
    {
        /// <summary>
        /// 
        /// </summary>
        public static SearchWindow SearchWindow { get; } = new();

        /// <summary>
        /// 
        /// </summary>
        public static ProgressWindow ProgressWindow { get; } = new();
    }
}
