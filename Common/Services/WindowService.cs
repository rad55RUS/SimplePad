using SimplePad.Views;

namespace SimplePad.Services
{
    /// <summary>
    /// Сontains instances of windows used in the application, providing a centralized way to access them.
    /// </summary>
    public static class WindowService
    {
        /// <summary>
        /// Gets the instance of the <see cref="SearchWindow"/> used in the application which provides search and replace functionality.
        /// </summary>
        public static SearchWindow SearchWindow { get; } = new();

        /// <summary>
        /// Gets the instance of the <see cref="ProgressWindow"/> used in the application which displays progress information for long-running operations.
        /// </summary>
        public static ProgressWindow ProgressWindow { get; } = new();
    }
}
