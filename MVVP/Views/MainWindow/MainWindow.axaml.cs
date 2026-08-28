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
    /// Represents the main window of the application, providing the primary user interface for interacting with the application's features and functionalities.
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _forceClose = false;
        private WindowState _state = WindowState.Minimized;

        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Gets or sets the current state of the window (e.g., Normal, Minimized, Maximized). 
        /// <br/><br/>
        /// This property is used to synchronize the window's state with the UI and handle state changes appropriately.
        /// </summary>
        public WindowState State
        {
            get => _state;
            set
            {
                _state = value;

                if (WindowState != value)
                {
                    WindowState = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class, setting up event handlers and loading user settings for window placement and size.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);

            // Load placement & size settings
            if (SettingsService.UserSettings.UISettings.MainWindowX != -1 && SettingsService.UserSettings.UISettings.MainWindowY != -1)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;

                Position = new(SettingsService.UserSettings.UISettings.MainWindowX, SettingsService.UserSettings.UISettings.MainWindowY);
                Width = SettingsService.UserSettings.UISettings.MainWindowWidth;
                Height = SettingsService.UserSettings.UISettings.MainWindowHeight;
            }
            //
        }

        #region Window Event Handlers
        /// <summary>
        /// Handles the Key Down event for the window.
        /// <br/><br/>
        /// <b>Shortcuts:</b>
        /// <br/>
        /// <c>Ctrl+F</c> - Find - Calls the <see cref="OnFind"/> method to open the <see cref="SearchWindow"/> on the find tab.
        /// <br/>
        /// <c>Ctrl+Shift+F</c> - Find in Files - Calls the <see cref="OnFindInFiles"/> method to open the <see cref="SearchWindow"/> on the find in files tab.
        /// <br/>
        /// <c>Ctrl+H</c> - Replace - Calls the <see cref="OnReplace"/> method the open the <see cref="SearchWindow"/> on the replace tab.
        /// <br/>
        /// <c>Ctrl+G</c> - Goto - Calls the <see cref="OnGoto"/> method to open the <see cref="SearchWindow"/> on the goto tab.
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
                            OnFind(this, new RoutedEventArgs());
                        }
                        else
                        {
                            OnFindInFiles(this, new RoutedEventArgs());
                        }
                        e.Handled = true;
                        break;
                    case Key.H:
                        OnReplace(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.G:
                        OnGoto(this, new RoutedEventArgs());
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

        /// <summary>
        /// Handles the Restore event call, restoring the window to its normal state if it is currently maximized or minimized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRestore(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Handles the Minimize event call, minimizing the window to the taskbar or system tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimize(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Maximize event call, maximizing the window to fill the screen or restore it to its previous size if it is already maximized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMaximize(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Handles the Close event call, closing the window and triggering any necessary cleanup or save operations before the application exits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the window's Resized event, updating the window state and adjusting the UI elements (Restore and Maximize menu items) based on the current state of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResized(object? sender, WindowResizedEventArgs e)
        {
            if (WindowState == State) return;

            State = WindowState;

            RestoreMenuItem.Classes.Clear();
            MaximizeMenuItem.Classes.Clear();

            switch (State)
            {
                case WindowState.Normal:
                    RestoreMenuItem.Classes.Add("Unclickable");
                    break;
                case WindowState.Maximized:
                    MaximizeMenuItem.Classes.Add("Unclickable");
                    break;

            }
        }

        /// <summary>
        /// Handles the window's Closing event, checking for unsaved changes in the DataContext (MainViewModel) and prompting the user to save changes if necessary before closing the application.
        /// <br/><br/>
        /// If the user chooses to save or discard changes, the application will close; otherwise, the closing operation will be canceled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClosing(object? sender, WindowClosingEventArgs e)
        {
            if (_forceClose) return;

            if (DataContext is MainViewModel dataContext)
            {
                if (dataContext.IsTextChanged)
                {
                    e.Cancel = true;

                    _forceClose = await dataContext.CallSaveWarning();
                }
                else
                {
                    _forceClose = true;
                }
            }
            if (_forceClose)
            {
                SaveSettings();

                // Close the application
                WindowService.ProgressWindow.Close();
                WindowService.SearchWindow.Close();

                Close();
                //
            }
        }
        #endregion

        #region Search Window Event Handlers
        /// <summary>
        /// Handles the Find event call, opening the SearchWindow and selecting the Find tab for searching text within the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFind(object? sender, RoutedEventArgs e)
        {
            WindowService.SearchWindow.SearchTabControl.SelectedIndex = 0;
            WindowService.SearchWindow.Hide();
            WindowService.SearchWindow.Show(this);
        }

        /// <summary>
        /// Handles the Find in Files event call, opening the SearchWindow and selecting the Find in Files tab for searching text across multiple files within the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindInFiles(object? sender, RoutedEventArgs e)
        {
            WindowService.SearchWindow.SearchTabControl.SelectedIndex = 2;
            WindowService.SearchWindow.Hide();
            WindowService.SearchWindow.Show(this);
        }

        /// <summary>
        /// Handles the Replace event call, opening the SearchWindow and selecting the Replace tab for replacing text within the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplace(object? sender, RoutedEventArgs e)
        {
            WindowService.SearchWindow.SearchTabControl.SelectedIndex = 1;
            WindowService.SearchWindow.Hide();
            WindowService.SearchWindow.Show(this);
        }

        /// <summary>
        /// Handles the Goto event call, opening the SearchWindow and selecting the Goto tab for navigating to specific lines or sections within the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoto(object? sender, RoutedEventArgs e)
        {
            WindowService.SearchWindow.SearchTabControl.SelectedIndex = 3;
            WindowService.SearchWindow.Hide();
            WindowService.SearchWindow.Show(this);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Saves the current window placement, size, and functional settings to the user settings file.
        /// </summary>
        private void SaveSettings()
        {
            if (IsPositionValid(Position))
            {
                // Save placement settings
                SettingsService.UserSettings.UISettings.MainWindowX = Position.X;
                SettingsService.UserSettings.UISettings.MainWindowY = Position.Y;
                //
            }

            // Save size settings
            SettingsService.UserSettings.UISettings.MainWindowWidth = Width;
            SettingsService.UserSettings.UISettings.MainWindowHeight = Height;
            //

            // Save functional settings
            DefinedDataContext?.SaveSettings();
            //

            // Save user settings file
            SettingsService.SaveSettings();
            //
        }

        /// <summary>
        /// Checks if the given position is valid within the bounds of the screen, ensuring that the window does not go too far off-screen when saving its position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsPositionValid(PixelPoint position)
        {
            // Get the window screen
            var screen = Screens.ScreenFromPoint(position);
            if (screen == null) return false;
            //

            // Check if the window is too far off-screen
            bool isTooLeft = position.X < screen.Bounds.X - Width + 100;
            bool isTooTop = position.Y < screen.Bounds.Y - Height + 100;
            bool isTooRight = position.X > screen.Bounds.X + screen.Bounds.Width - 100;
            bool isTooBottom = position.Y > screen.Bounds.Y + screen.Bounds.Height - 100;
            //

            return !(isTooLeft || isTooTop || isTooRight || isTooBottom);
        }
        #endregion
    }
}