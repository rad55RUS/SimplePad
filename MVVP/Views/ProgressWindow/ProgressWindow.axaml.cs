using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using SimplePad.ViewModels;
using System;
using System.Threading.Tasks;
using static AvaloniaEdit.Document.TextDocumentWeakEventManager;


namespace SimplePad.Views
{
    /// <summary>
    /// Represents the window for displaying progress information in the application, providing a user interface for monitoring the progress of long-running operations.
    /// </summary>
    public partial class ProgressWindow : Window
    {
        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressWindow"/> class, setting up the user interface components and preparing the window for display.
        /// </summary>
        public ProgressWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Begin Move Drag event, allowing the user to drag the window by clicking and holding the mouse button on the window's title bar or any other draggable area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBeginMoveDrag(object? sender, PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }
    }
}