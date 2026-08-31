using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using SimplePad.Services;
using SimplePad.ViewModels;
using System;
using System.Threading.Tasks;

using static SimplePad.Views.ViewUtils;


namespace SimplePad.Views
{
    /// <summary>
    /// Represents the search window in the application, providing a user interface for navigation, searching and replacing text within the application or documents.
    /// </summary>
    public partial class SearchWindow : Window
    {
        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchWindow"/> class, setting up the user interface components and preparing the window for display.
        /// </summary>
        public SearchWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the window, setting up event handlers and initializing the window for display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// Handles the Opened event of the window, positioning the window relative to its owner and ensuring it is displayed correctly on the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpened(object? sender, EventArgs e)
        {
            if (Owner is Window ownerWindow)
            {
                PixelPoint point = GetRightUpCornerRelativeTo(this, ownerWindow);
                Position = new PixelPoint(point.X, point.Y + 60);
            }
        }

        /// <summary>
        /// Handles the Close button click event, hiding the window instead of closing it and marking the event as handled to prevent further propagation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object? sender, RoutedEventArgs e)
        {
            Hide();

            e.Handled = true;
        }

        /// <summary>
        /// Handles the Key Down event for the window.
        /// <br/><br/>
        /// <b>Shortcuts:</b>
        /// <br/>
        /// <c>Ctrl+F</c> - Find - Switches to the find tab in the <see cref="SearchTabControl"/>.
        /// <br/>
        /// <c>Ctrl+Shift+F</c> - Find in Files - Switches to the find in files tab in the <see cref="SearchTabControl"/>.
        /// <br/>
        /// <c>Ctrl+H</c> - Replace - Switches to the replace tab in the <see cref="SearchTabControl"/>.
        /// <br/>
        /// <c>Ctrl+G</c> - Goto - Switches to the goto tab in the <see cref="SearchTabControl"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.F:
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                        {
                            SearchTabControl.SelectedIndex = 0;
                        }
                        else
                        {
                            SearchTabControl.SelectedIndex = 2;
                        }
                        e.Handled = true;
                        break;
                    case Key.H:
                        SearchTabControl.SelectedIndex = 1;
                        e.Handled = true;
                        break;
                    case Key.G:
                        SearchTabControl.SelectedIndex = 3;
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Hide();
                        e.Handled = true;
                        break;
                }
            }
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