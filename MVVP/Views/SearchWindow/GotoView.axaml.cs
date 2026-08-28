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
    /// Represents the view for the "Goto" functionality in the application for navigating to a specific line or position within a text editor.
    /// </summary>
    public partial class GotoView : ViewBase
    {
        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GotoView"/> class, setting up the user interface components and preparing the view for display.
        /// </summary>
        public GotoView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the view, setting focus to the <see cref="GotoTextBox"/> when the view is loaded to improve user experience.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            GotoTextBox.Focus();
        }
    }
}