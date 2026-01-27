using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using AvaloniaEdit;

using SimplePad.Services;
using SimplePad.ViewModels;
using SimplePad.Views;

using static SimplePad.Views.ViewUtils;

namespace SimplePad
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainViewModel MainDataContext = new();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = MainDataContext,
                };

                WindowService.SearchWindow.DataContext = MainDataContext;
            }

            base.OnFrameworkInitializationCompleted();
        }

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorUndo(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Undo();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorRedo(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Redo();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorCut(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Cut();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorCopy(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Copy();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorPaste(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Paste();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorDelete(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Delete();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextEditorSelectAll(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (GetMenuItemTarget(menuItem) is TextEditor textEditor)
                {
                    textEditor.Select(0, textEditor.Text.Length);
                }
            }
        }
        #endregion
    }
}