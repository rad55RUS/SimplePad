using System.Collections.Generic;
using System.Reflection;

namespace SimplePad.Settings
{
    /// <summary>
    /// Сontains settings for saving and restoring the state of the application, including search and replace functionality, file processing, and user interface preferences.
    /// </summary>
    public class FunctionalSettings
    {
        /// <summary>
        /// Indicates whether the search should include all subfolders in the specified directory.
        /// </summary>
        public bool IsInAllSubfolders { get; set; } = true;

        /// <summary>
        /// Indicates whether the search should be case-sensitive.
        /// </summary>
        public bool IsMatchCase { get; set; } = false;

        /// <summary>
        /// Indicates whether the search should match whole words only.
        /// </summary>
        public bool IsMatchWholeWords { get; set; } = false;

        /// <summary>
        /// Indicates whether the input for search and replace operations can span multiple lines.
        /// </summary>
        public bool IsMultipleLineInput { get; set; } = false;

        /// <summary>
        /// Indicates whether the search direction is upwards in the text.
        /// </summary>
        public bool IsUpDirection { get; set; } = false;

        /// <summary>
        /// Indicates whether the text in the main text area should be wrapped.
        /// </summary>
        public bool IsWordWrapEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the saved text to search for.
        /// </summary>
        public string SearchText { get; set; } = "";

        /// <summary>
        /// Gets or sets the saved text to replace with.
        /// </summary>
        public string ReplaceText { get; set; } = "";

        /// <summary>
        /// Gets or sets the saved directory for search and replace operations in files.
        /// </summary>
        public string FileProcessingDirectory { get; set; } = "";

        /// <summary>
        /// Gets or sets the list of saved selected file extensions for search and replace operations.
        /// </summary>
        public List<string> SelectedExtensions { get; set; } = [".txt"];
    }
}
