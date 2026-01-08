using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SimplePad.Views
{
    public partial class MainControl : ViewBase
    {
        /// <summary>
        /// 
        /// </summary>
        public MainControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUndo(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Undo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRedo(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Redo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCut(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Cut();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopy(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Copy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaste(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Paste();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDelete(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Delete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectAll(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Select(0, MainTextEditor.Text.Length);
        }

    }
}