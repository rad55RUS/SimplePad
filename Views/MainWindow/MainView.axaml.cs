using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Search;
using SimplePad.EventHandlers;
using SimplePad.Services;
using SimplePad.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;

namespace SimplePad.Views
{
    public partial class MainView : ViewBase
    {
        /// <summary>
        /// 
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// 
        /// </summary>
        public MainView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.TextChanged += OnTextChanged;

            if (DefinedDataContext != null)
            {
                DefinedDataContext.FindNextCalled += OnFindNext;
                DefinedDataContext.FindPreviousCalled += OnFindPrevious;
                DefinedDataContext.FindInFilesCalled += OnFindInFiles;
                DefinedDataContext.ReplaceInFilesCalled += OnReplaceInFiles;
                DefinedDataContext.ReplaceNextCalled += OnReplaceNext;
                DefinedDataContext.ReplacePreviousCalled += OnReplacePrevious;
                DefinedDataContext.ReplaceAllCalled += OnReplaceAll;
                DefinedDataContext.GotoCalled += OnGoto;
            }
        }

        #region Edit Menu Event Handlers
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
        #endregion

        #region Search Menu Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFind(object? sender, RoutedEventArgs e)
        {
            Window? window = GetParentWindow();

            if (window != null)
            {
                SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 0;
                SearchWindowService.SearchWindow.Show(window);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindInFiles(object? sender, RoutedEventArgs e)
        {
            Window? window = GetParentWindow();

            if (window != null)
            {
                SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 2;
                SearchWindowService.SearchWindow.Show(window);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplace(object? sender, RoutedEventArgs e)
        {
            Window? window = GetParentWindow();

            if (window != null)
            {
                SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 1;
                SearchWindowService.SearchWindow.Show(window);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoto(object? sender, RoutedEventArgs e)
        {
            Window? window = GetParentWindow();

            if (window != null)
            {
                SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 3;
                SearchWindowService.SearchWindow.Show(window);
            }
        }
        #endregion

        #region Text Editor Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object? sender, EventArgs e)
        {
            if (DefinedDataContext != null)
            {
                DefinedDataContext.CanUndo = MainTextEditor.CanUndo;
                DefinedDataContext.CanRedo = MainTextEditor.CanRedo;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindNext(object? sender, SearchEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SearchText)) return;

            var strategy = SearchStrategyFactory.Create(e.SearchText, false, false, SearchMode.Normal);

            var result = strategy.FindNext(MainTextEditor.Document, MainTextEditor.CaretOffset + 1, MainTextEditor.Document.TextLength);

            if (result == null)
            {
                result = strategy.FindNext(MainTextEditor.Document, 0, MainTextEditor.CaretOffset);
            }

            if (result != null)
            {
                MainTextEditor.Select(result.Offset, result.Length);
                MainTextEditor.TextArea.Caret.Offset = result.Offset;
                MainTextEditor.ScrollToLine(MainTextEditor.Document.GetLineByOffset(result.Offset).LineNumber);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindPrevious(object? sender, SearchEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SearchText)) return;

            var strategy = SearchStrategyFactory.Create(e.SearchText, ignoreCase: true, matchWholeWords: false, SearchMode.Normal);

            var allResults = strategy.FindAll(MainTextEditor.Document, 0, MainTextEditor.Document.TextLength).ToList();
            if (allResults.Count == 0) return;

            var result = allResults.Where(r => r.Offset < MainTextEditor.CaretOffset).LastOrDefault();

            if (result == null)
            {
                result = allResults.Last();
            }

            MainTextEditor.Select(result.Offset, result.Length);
            MainTextEditor.TextArea.Caret.Offset = result.Offset;
            MainTextEditor.TextArea.Caret.BringCaretToView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindInFiles(object? sender, SearchEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceInFiles(object? sender, SearchEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceNext(object? sender, SearchEventArgs e)
        {
            if (MainTextEditor.SelectedText == e.SearchText)
            {
                MainTextEditor.SelectedText = e.ReplaceText;
            }

            OnFindNext(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplacePrevious(object? sender, SearchEventArgs e)
        {
            if (MainTextEditor.SelectedText == e.SearchText)
            {
                MainTextEditor.SelectedText = e.ReplaceText;
            }

            OnFindPrevious(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceAll(object? sender, SearchEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SearchText)) return;

            var strategy = SearchStrategyFactory.Create(e.SearchText, ignoreCase: true, matchWholeWords: false, SearchMode.Normal);

            var results = strategy.FindAll(MainTextEditor.Document, 0, MainTextEditor.Document.TextLength).ToList();
            if (results.Count == 0) return;

            foreach (var result in results)
            {
                MainTextEditor.Select(result.Offset, result.Length);
                MainTextEditor.TextArea.Caret.Offset = result.Offset;
                MainTextEditor.TextArea.Caret.BringCaretToView();
                MainTextEditor.SelectedText = e.ReplaceText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoto(object? sender, string gotoText)
        {
            int gotoLineNumber = Int32.Parse(gotoText);

            if (gotoLineNumber < 1 || gotoLineNumber > MainTextEditor.Document.LineCount)
                return;

            var line = MainTextEditor.Document.GetLineByNumber(gotoLineNumber);

            MainTextEditor.CaretOffset = line.Offset;
            MainTextEditor.TextArea.Caret.BringCaretToView();
            MainTextEditor.Focus();

            GetParentWindow()?.Activate();
        }
        #endregion
    }
}