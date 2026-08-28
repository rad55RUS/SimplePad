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
    /// Represents the view for displaying progress information in the application, providing a user interface for monitoring the progress of long-running operations.
    /// </summary>
    public partial class ProgressView : ViewBase
    {
        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressView"/> class, setting up the user interface components and preparing the view for display.
        /// </summary>
        public ProgressView()
        {
            InitializeComponent();
        }
    }
}