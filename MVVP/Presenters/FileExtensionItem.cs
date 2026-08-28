using CommunityToolkit.Mvvm.Input;
using SimplePad.ViewModels;
using System;

namespace SimplePad.Presenters
{
    /// <summary>
    /// Presents a file extension item with properties for its checked state and the extension string. 
    /// <b></b>
    /// This class is used in the context of a UI to represent file extensions that can be selected or deselected by the user
    /// </summary>
    public partial class FileExtensionItem : ViewModelBase
    {
        private bool _isChecked = false;
        private string _extension = ".example";

        /// <summary>
        /// Gets or sets a value indicating whether the file extension is checked.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        /// <summary>
        /// Gets or sets the selectable file extension string.
        /// </summary>
        public string Extension 
        { 
            get => _extension;
            set => SetProperty(ref _extension, value);
        }

        /// <summary>
        /// Empty constructor of the <see cref="FileExtensionItem"/> class for deserialization purposes.
        /// </summary>
        public FileExtensionItem() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionItem"/> class.
        /// </summary>
        /// <param name="isChecked">A value indicating whether the file extension is checked.</param>
        /// <param name="extension">The file extension string.</param>
        public FileExtensionItem(bool isChecked, string extension)
        {
            _isChecked = isChecked;
            _extension = extension;
        }
    }
}
