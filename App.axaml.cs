using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using AvaloniaEdit;

using SimplePad.Services;
using SimplePad.ViewModels;
using SimplePad.Views;
using System.Linq;
using static SimplePad.Views.ViewUtils;

namespace SimplePad
{
    /// <summary>
    /// Represents the main application class for the application, responsible for initializing the application, setting up the main window, and handling application-level events and behaviors.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets or sets the command-line arguments passed to the application, allowing for processing of input parameters and configuration options at startup.
        /// </summary>
        public static string[]? Args { get; set; }

        /// <summary>
        /// Overrides the Initialize method to load the XAML markup for the application, setting up the user interface and preparing the application for execution.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Overrides the OnFrameworkInitializationCompleted method to perform additional initialization tasks after the framework has completed its setup. 
        /// <br/><br/>
        /// This includes creating the main window, setting its data context, and configuring other windows with the same data context.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Set up the main window and its data context
                MainViewModel MainDataContext = new();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = MainDataContext,
                };
                //

                // Process command-line arguments to open a file if provided
                if (Args != null)
                {
                    string? filePath = Args.FirstOrDefault(a => !a.StartsWith("-"));

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        MainDataContext.OpenFile(filePath);
                    }
                }
                //

                // Set the DataContext for other windows to the same MainDataContext
                WindowService.SearchWindow.DataContext = MainDataContext;
                WindowService.ProgressWindow.DataContext = MainDataContext;
                //
            }

            base.OnFrameworkInitializationCompleted();
        }

        #region Event handlers
        /// <summary>
        /// Handles the event when the text editor's context menu is opened, enabling or disabling menu items based on the current state of the text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorMenuOpened(object? sender, System.EventArgs e)
        {
            if (sender is MenuFlyout menu)
            {
                foreach (Control? item in menu.Items)
                {
                    if (item is MenuItem menuItem)
                    {
                        if (menuItem is null) continue;

                        if (menuItem.Header?.ToString() == "Undo")
                        {
                            if (GetMenuFlyoutTarget(menu) is TextEditor textEditor)
                            {
                                menuItem.IsEnabled = textEditor.CanUndo;
                            }
                            if (GetMenuFlyoutTarget(menu) is TextBox textBox)
                            {
                                menuItem.IsEnabled = textBox.CanUndo;
                            }
                        }
                        else if (menuItem.Header?.ToString() == "Redo")
                        {
                            if (GetMenuFlyoutTarget(menu) is TextEditor textEditor)
                            {
                                menuItem.IsEnabled = textEditor.CanRedo;
                            }
                            if (GetMenuFlyoutTarget(menu) is TextBox textBox)
                            {
                                menuItem.IsEnabled = textBox.CanRedo;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Undo" menu item is clicked in the text editor's context menu, performing the undo operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorUndo(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.Undo();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.Undo();
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Redo" menu item is clicked in the text editor's context menu, performing the redo operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorRedo(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.Redo();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.Redo();
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Cut" menu item is clicked in the text editor's context menu, performing the cut operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorCut(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.Cut();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.Cut();
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Copy" menu item is clicked in the text editor's context menu, performing the copy operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorCopy(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.Copy();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.Copy();
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Paste" menu item is clicked in the text editor's context menu, performing the paste operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorPaste(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.Paste();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.Paste();
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Delete" menu item is clicked in the text editor's context menu, performing the delete operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorDelete(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.Delete();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.SelectedText = "";
                }
            }
        }

        /// <summary>
        /// Handles the event when the "Select All" menu item is clicked in the text editor's context menu, performing the select all operation on the associated text editor or text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorSelectAll(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var menuItemTarget = GetMenuItemTarget(menuItem);

                if (menuItemTarget is TextEditor textEditor)
                {
                    textEditor.SelectAll();
                }
                else if (menuItemTarget is TextBox textBox)
                {
                    textBox.SelectAll();
                }
            }
        }
        #endregion
    }
}