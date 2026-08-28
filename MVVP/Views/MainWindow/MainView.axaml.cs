using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Svg.Skia;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Search;
using SimplePad.EventHandlers;
using SimplePad.Transformers;
using SimplePad.Services;
using SimplePad.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;

using static SimplePad.Views.ViewUtils;

namespace SimplePad.Views
{
    /// <summary>
    /// Represents the main view for the application, providing the user interface for text editing and related operations.
    /// </summary>
    public partial class MainView : ViewBase
    {
        private bool _areSearchResultsVisible = false;

        private readonly SearchResultsTransformer _searchResultsTransformer = new();

        /// <summary>
        /// Defines a strongly-typed property to access the DataContext as a MainViewModel, allowing for easier interaction with the view model's properties and methods.
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class, setting up the user interface and adding necessary transformers for text processing.
        /// </summary>
        public MainView()
        {
            InitializeComponent();

            // Add transformers
            SearchResultsViewer.TextArea.TextView.LineTransformers.Add(
                _searchResultsTransformer
            );
            //
        }

        /// <summary>
        /// Handles the Loaded event of the MainView, setting up event handlers for text changes, caret movements, and various commands from the view model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.TextChanged += OnTextChanged;
            MainTextEditor.TextArea.Caret.PositionChanged += OnCaretMoved;

            MainTextEditor.TextArea.Options.AllowScrollBelowDocument = false;
            SearchResultsViewer.TextArea.Options.AllowScrollBelowDocument = false;

            if (DefinedDataContext != null)
            {
                DefinedDataContext.FindNextCalled += OnFindNext;
                DefinedDataContext.FindPreviousCalled += OnFindPrevious;
                DefinedDataContext.FindInFilesCalled += OnFindInFiles;
                DefinedDataContext.ReplaceInFilesCalled += OnReplaceInFiles;
                DefinedDataContext.ReplaceNextCalled += OnReplaceNext;
                DefinedDataContext.ReplacePreviousCalled += OnReplacePrevious;
                DefinedDataContext.ReplaceAllCalled += OnReplaceAll;
                DefinedDataContext.FileProcessingCancelCalled += OnFileProcessingCancelling;
                DefinedDataContext.FileProcessingDone += OnFileProcessingDone;
                DefinedDataContext.GotoCalled += OnGoto;
            }
        }

        #region Edit Menu Event Handlers
        /// <summary>
        /// Handles the Undo event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Undo method of the <see cref="MainTextEditor"/> to revert the last change made to the text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUndo(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Undo();
        }

        /// <summary>
        /// Handles the Redo event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Redo method of the <see cref="MainTextEditor"/> to reapply the last undone change to the text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRedo(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Redo();
        }

        /// <summary>
        /// Handles the Cut event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Cut method of the <see cref="MainTextEditor"/> to remove the selected text and place it on the clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCut(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Cut();
        }

        /// <summary>
        /// Handles the Copy event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Copy method of the <see cref="MainTextEditor"/> to copy the selected text to the clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopy(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Copy();
        }

        /// <summary>
        /// Handles the Paste event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Paste method of the <see cref="MainTextEditor"/> to insert the text from the clipboard at the current cursor position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaste(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Paste();
        }

        /// <summary>
        /// Handles the Delete event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Delete method of the <see cref="MainTextEditor"/> to remove the selected text without placing it on the clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDelete(object? sender, RoutedEventArgs e)
        {
            MainTextEditor.Delete();
        }

        /// <summary>
        /// Handles the Select All event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Calls the Select method of the <see cref="MainTextEditor"/> to select all text within the editor.
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
        /// Handles the Find event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the search window and selects the "Find" tab, allowing the user to search for text within the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFind(object? sender, RoutedEventArgs e)
        {
            Window? mainWindow = GetParentWindow();

            if (mainWindow != null)
            {
                WindowService.SearchWindow.SearchTabControl.SelectedIndex = 0;
                WindowService.SearchWindow.Hide();
                WindowService.SearchWindow.Show(mainWindow);
            }
        }

        /// <summary>
        /// Handles the Find in Files event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the search window and selects the "Find in Files" tab, allowing the user to search for text across multiple files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindInFiles(object? sender, RoutedEventArgs e)
        {
            Window? mainWindow = GetParentWindow();

            if (mainWindow != null)
            {
                WindowService.SearchWindow.SearchTabControl.SelectedIndex = 2;
                WindowService.SearchWindow.Hide();
                WindowService.SearchWindow.Show(mainWindow);
            }
        }

        /// <summary>
        /// Handles the Replace event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the search window and selects the "Replace" tab, allowing the user to find and replace text within the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplace(object? sender, RoutedEventArgs e)
        {
            Window? mainWindow = GetParentWindow();

            if (mainWindow != null)
            {
                WindowService.SearchWindow.SearchTabControl.SelectedIndex = 1;
                WindowService.SearchWindow.Hide();
                WindowService.SearchWindow.Show(mainWindow);
            }
        }

        /// <summary>
        /// Handles the Go To event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the search window and selects the "Go To" tab, allowing the user to navigate to a specific line number within the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoto(object? sender, RoutedEventArgs e)
        {
            Window? mainWindow = GetParentWindow();

            if (mainWindow != null)
            {
                WindowService.SearchWindow.SearchTabControl.SelectedIndex = 3;
                WindowService.SearchWindow.Hide();
                WindowService.SearchWindow.Show(mainWindow);
            }
        }
        #endregion

        #region Text Editor Event Handlers
        /// <summary>
        /// Handles the Text Changed event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Updates the undo and redo capabilities of the defined data context based on the text editor's state.
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
        /// Handles the Caret Moved event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Updates the current line number in the defined data context based on the text editor's caret position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCaretMoved(object? sender, EventArgs e)
        {
            if (DefinedDataContext != null)
            {
                DefinedDataContext.CurrentLine = MainTextEditor.TextArea.Caret.Line;
            }
        }

        /// <summary>
        /// Handles the Find Next event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Searches for the next occurrence of the specified text in the document and selects it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindNext(object? sender, SearchEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SearchText)) return;

            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            // Perform the search
            var strategy = SearchStrategyFactory.Create(searchText, !e.MatchCase, e.MatchWholeWords, SearchMode.Normal);

            var result = strategy.FindNext(MainTextEditor.Document, MainTextEditor.CaretOffset + 1, MainTextEditor.Document.TextLength);

            if (result == null)
            {
                result = strategy.FindNext(MainTextEditor.Document, 0, MainTextEditor.CaretOffset + searchText.Length);
            }
            //

            // Select the found result in the text editor
            if (result != null)
            {
                MainTextEditor.Select(result.Offset, result.Length);
                MainTextEditor.TextArea.Caret.Offset = result.Offset;
                MainTextEditor.ScrollToLine(MainTextEditor.Document.GetLineByOffset(result.Offset).LineNumber);
            }
            //
        }

        /// <summary>
        /// Handles the Find Previous event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Searches for the previous occurrence of the specified text in the document and selects it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindPrevious(object? sender, SearchEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SearchText)) return;

            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            // Perform the search
            var strategy = SearchStrategyFactory.Create(searchText, !e.MatchCase, e.MatchWholeWords, SearchMode.Normal);

            var allResults = strategy.FindAll(MainTextEditor.Document, 0, MainTextEditor.Document.TextLength).ToList();
            if (allResults.Count == 0) return;

            var result = allResults.Where(r => r.Offset < MainTextEditor.CaretOffset).LastOrDefault();

            if (result == null)
            {
                result = allResults.Last();
            }
            //

            // Select the found result in the text editor
            MainTextEditor.Select(result.Offset, result.Length);
            MainTextEditor.TextArea.Caret.Offset = result.Offset;
            MainTextEditor.TextArea.Caret.BringCaretToView();
            //
        }

        /// <summary>
        /// Handles the Replace Next event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Replaces the currently selected text with the specified replacement text if it matches the search text, and then searches for the next occurrence of the search text in the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceNext(object? sender, SearchEventArgs e)
        {
            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            if (MainTextEditor.SelectedText == searchText)
            {
                MainTextEditor.SelectedText = e.ReplaceText;
            }

            OnFindNext(sender, e);
        }

        /// <summary>
        /// Handles the Replace Previous event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Replaces the currently selected text with the specified replacement text if it matches the search text, and then searches for the previous occurrence of the search text in the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplacePrevious(object? sender, SearchEventArgs e)
        {
            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            if (MainTextEditor.SelectedText == searchText)
            {
                MainTextEditor.SelectedText = e.ReplaceText;
            }

            OnFindPrevious(sender, e);
        }

        /// <summary>
        /// Handles the Replace All event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Replaces all occurrences of the specified search text with the replacement text in the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceAll(object? sender, SearchEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SearchText)) return;

            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            // Perform the search for all occurrences of the search text in the document
            var strategy = SearchStrategyFactory.Create(searchText, !e.MatchCase, e.MatchWholeWords, SearchMode.Normal);

            var results = strategy.FindAll(MainTextEditor.Document, 0, MainTextEditor.Document.TextLength).ToList();
            if (results.Count == 0) return;
            //

            // Replace all occurrences of the search text with the replacement text
            foreach (var result in results)
            {
                MainTextEditor.Select(result.Offset, result.Length);
                MainTextEditor.TextArea.Caret.Offset = result.Offset;
                MainTextEditor.TextArea.Caret.BringCaretToView();
                MainTextEditor.SelectedText = e.ReplaceText;
            }
            //
        }

        /// <summary>
        /// Handles the Go To event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Navigates to the specified line number in the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoto(object? sender, int lineNumber)
        {
            // Validate the line number and navigate to the specified line in the text editor
            if (lineNumber >= 1 && lineNumber <= MainTextEditor.Document.LineCount)
            {
                var line = MainTextEditor.Document.GetLineByNumber(lineNumber);

                MainTextEditor.CaretOffset = line.Offset;
            }
            //

            // Ensure the caret is visible and focus the text editor
            MainTextEditor.TextArea.Caret.BringCaretToView();
            MainTextEditor.Focus();

            GetParentWindow()?.Activate();
            //
        }
        #endregion

        #region File Processing Event Handlers
        /// <summary>
        /// Handles the Find in Files event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the progress window and updates the search results transformer with the specified search text for searching across multiple files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindInFiles(object? sender, SearchEventArgs e)
        {
            Window? mainWindow = GetParentWindow();

            // Show the progress window centered relative to the main window
            if (mainWindow != null)
            {
                WindowService.ProgressWindow.Position = GetCenterRelativeTo(WindowService.ProgressWindow, mainWindow);
                WindowService.ProgressWindow.Show(mainWindow);
            }
            //

            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            _searchResultsTransformer.UpdateSearchText(searchText);
        }

        /// <summary>
        /// Handles the Replace in Files event call from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the progress window and updates the search results transformer with the specified search and replace texts for replacing across multiple files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceInFiles(object? sender, SearchEventArgs e)
        {
            Window? mainWindow = GetParentWindow();

            // Show the progress window centered relative to the main window
            if (mainWindow != null)
            {
                WindowService.ProgressWindow.Position = GetCenterRelativeTo(WindowService.ProgressWindow, mainWindow);
                WindowService.ProgressWindow.Show(mainWindow);
            }
            //

            // Normalize line transitions
            var searchText = e.SearchText.Replace("\r\n", "\n").Replace("\r", "\n");
            var replaceText = e.ReplaceText.Replace("\r\n", "\n").Replace("\r", "\n");
            //

            _searchResultsTransformer.UpdateSearchText(searchText);
            _searchResultsTransformer.UpdateReplaceText(replaceText);
        }

        /// <summary>
        /// Handles the cancellation of file processing from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Hides the progress window when the file processing operation is cancelled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileProcessingCancelling(object? sender, EventArgs e)
        {
            WindowService.ProgressWindow.Hide();
        }

        /// <summary>
        /// Handles the completion of file processing from the data context.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Hides the progress window and shows the search results if they are not already visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileProcessingDone(object? sender, EventArgs e)
        {
            WindowService.ProgressWindow.Hide();

            if (!_areSearchResultsVisible)
            {
                OnShowHideSearchResults(null, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Handles the Show/Hide Search Results event call.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Minimizes or maximizes the search results section in the user interface, adjusting the layout and icon accordingly based on the current visibility state of the search results.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowHideSearchResults(object? sender, RoutedEventArgs e)
        {
            if (_areSearchResultsVisible)
            {
                ShowHideSearchResultsIcon.Source = GetSvgFromUriString("avares://SimplePad/Assets/UpDirectionIcon.svg");
                TextGrid.RowDefinitions[2].Height = new GridLength(25);
            }
            else
            {
                Window? mainWindow = GetParentWindow();

                ShowHideSearchResultsIcon.Source = GetSvgFromUriString("avares://SimplePad/Assets/DownDirectionIcon.svg");

                if (mainWindow != null)
                {
                    TextGrid.RowDefinitions[2].Height = new GridLength(mainWindow.Height / 3);
                }
                else
                {
                    TextGrid.RowDefinitions[2].Height = new GridLength(300);
                }
            }

            _areSearchResultsVisible = !_areSearchResultsVisible;
        }

        /// <summary>
        /// Handles the Size Changed event call for the search results section.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Updates the visibility state of the search results and the corresponding icon based on the new size of the search results section, ensuring that the user interface reflects the current state of the search results visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchResultsSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height > 25)
            {
                if (!_areSearchResultsVisible)
                {
                    ShowHideSearchResultsIcon.Source = GetSvgFromUriString("avares://SimplePad/Assets/DownDirectionIcon.svg");

                    _areSearchResultsVisible = true;
                }
            }
            else
            {
                if (_areSearchResultsVisible)
                {
                    ShowHideSearchResultsIcon.Source = GetSvgFromUriString("avares://SimplePad/Assets/UpDirectionIcon.svg");

                    _areSearchResultsVisible = false;
                }
            }
        }
        #endregion
    }
}