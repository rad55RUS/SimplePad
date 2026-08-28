using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using SimplePad.ViewModels;
using System;

namespace SimplePad.Views
{
    /// <summary>
    /// Represents the view for managing search functionality within files.
    /// </summary>
    public partial class FindInFilesView : ViewBase
    {
        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindInFilesView"/> class, setting up the user interface components and preparing the view for display.
        /// </summary>
        public FindInFilesView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the view, setting focus to the <see cref="FindTextBox"/> when the view is loaded to improve user experience.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            FindTextBox.Focus();
        }
    }
}