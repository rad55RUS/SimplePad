using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Search;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.FileIO;
using SimplePad.EventHandlers;
using SimplePad.Presenters;
using SimplePad.Services;
using SimplePad.Utils;
using SimplePad.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SimplePad.ViewModels
{
    /// <summary>
    /// View Model for the whole application, managing the state and behavior of the main window and its components.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        #region Fields
        private bool _isTextChanged;
        private bool _isWordWrapEnabled;
        private bool _isUpDirection = false;
        private bool _isMatchCase = false;
        private bool _isMatchWholeWords = false;
        private bool _isMultipleLineInput = false;
        private bool _inAllSubfolders = false;
        private bool _canUndo = false;
        private bool _canRedo = false;

        private int _currentLine = 1;
        private int _fileProcessingProgress = 0;
        private int _fileProcessingMaximum = 1;

        private string _title = "SimplePad";
        private string _currentPath = "";
        private string _initialText = "";
        private string _searchText = "";
        private string _replaceText = "";
        private string _fileProcessingDirectory = "";
        private string _fileProcessingCurrent = "Counting files...";
        private string _goToLineText = "";
        private string _searchResultsTitle = "Search results";
        
        private ObservableCollection<FileExtensionItem> _fileExtensions = [];

        private TextDocument _mainTextDocument = new();
        private TextDocument _searchResultsDocument = new();

        private CancellationTokenSource? _fileProcessingCancellationTokenSource;
        #endregion

        #region Events
        /// <summary>
        /// Event triggered when the text in the main document changes
        /// </summary>
        public event EventHandler? TextChanged;

        /// <summary>
        /// Event triggered when the <see cref="FileProcessingCancelCommand"/> is called and the file processing operation is requested to be canceled.
        /// </summary>
        public event EventHandler? FileProcessingCancelCalled;

        /// <summary>
        /// Event triggered when the file processing is done
        /// </summary>
        public event EventHandler? FileProcessingDone;

        /// <summary>
        /// Event triggered when the <see cref="FindCommand"/> is called, indicating a text search operation should be performed.
        /// </summary>
        public event SearchEventHandler? FindNextCalled;

        /// <summary>
        /// Event triggered when the <see cref="FindCommand"/> is called, indicating a text search operation should be performed in the reverse direction.
        /// </summary>
        public event SearchEventHandler? FindPreviousCalled;

        /// <summary>
        /// Event triggered when the <see cref="ReplaceCommand"/> is called, indicating a textreplace operation should be performed.
        /// </summary>
        public event SearchEventHandler? ReplaceNextCalled;

        /// <summary>
        /// Event triggered when the <see cref="ReplaceCommand"/> is called, indicating a text replace operation should be performed in the reverse direction.
        /// </summary>
        public event SearchEventHandler? ReplacePreviousCalled;

        /// <summary>
        /// Event triggered when the <see cref="ReplaceAllCommand"/> is called, indicating a text replace operation should be performed across the entire document.
        /// </summary>
        public event SearchEventHandler? ReplaceAllCalled;

        /// <summary>
        /// Event triggered when the <see cref="FindInFilesCommand"/> is called, indicating a search operation should be performed across multiple files.
        /// </summary>
        public event SearchEventHandler? FindInFilesCalled;

        /// <summary>
        /// Event triggered when the <see cref="ReplaceInFilesCommand"/> is called, indicating a text replace operation should be performed across multiple files.
        /// </summary>
        public event SearchEventHandler? ReplaceInFilesCalled;

        /// <summary>
        /// Event triggered when the <see cref="GotoCommand"/> is called, indicating a request to navigate to a specific line number in the document.
        /// </summary>
        public event EventHandler<int>? GotoCalled;
        #endregion

        #region Boolean Properties
        /// <summary>
        /// Gets or sets a value indicating whether the text in the <see cref="MainTextDocument"/> has been changed since the last save operation.
        /// </summary>
        public bool IsTextChanged
        {
            get => _isTextChanged;
            set
            {
                if (_isTextChanged != value)
                {
                    SetProperty(ref _isTextChanged, value);
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(ShortTitle));
                }
                else
                {
                    SetProperty(ref _isTextChanged, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether word wrap is enabled in the <see cref="MainTextDocument"/>.
        /// </summary>
        public bool IsWordWrapEnabled
        {
            get => _isWordWrapEnabled;
            set => SetProperty(ref _isWordWrapEnabled, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search direction is upwards in the text.
        /// </summary>
        public bool IsUpDirection
        {
            get => _isUpDirection;
            set => SetProperty(ref _isUpDirection, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search should be case-sensitive.
        /// </summary>
        public bool IsMatchCase
        {
            get => _isMatchCase;
            set => SetProperty(ref _isMatchCase, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search should match whole words only.
        /// </summary>
        public bool IsMatchWholeWords
        {
            get => _isMatchWholeWords;
            set => SetProperty(ref _isMatchWholeWords, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input for search and replace operations can span multiple lines.
        /// </summary>
        public bool IsMultipleLineInput
        {
            get => _isMultipleLineInput;
            set => SetProperty(ref _isMultipleLineInput, value);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the file processing operations should include all subfolders in the specified directory
        /// </summary>
        public bool IsInAllSubfolders
        {
            get => _inAllSubfolders;
            set => SetProperty(ref _inAllSubfolders, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the undo operation can be performed in the <see cref="MainTextDocument"/>
        /// </summary>
        public bool CanUndo 
        { 
            get => _canUndo;
            set => SetProperty(ref _canUndo, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the redo operation can be performed in the <see cref="MainTextDocument"/>
        /// </summary>
        public bool CanRedo 
        {
            get => _canRedo;
            set => SetProperty(ref _canRedo, value);
        }
        #endregion

        #region Integer Properties
        /// <summary>
        /// Gets or sets the current line number in the <see cref="MainTextDocument"/> where the caret is located.
        /// </summary>
        public int CurrentLine
        {
            get => _currentLine;
            set => SetProperty(ref _currentLine, value);
        }

        /// <summary>
        /// Gets the total number of lines in the <see cref="MainTextDocument"/>.
        /// </summary>
        public int LineCount
        {
            get => _mainTextDocument.LineCount;
        }

        /// <summary>
        /// Gets or sets the current progress of the file processing operation, indicating how many files have been processed so far.
        /// </summary>
        public int FileProcessingProgress
        {
            get => _fileProcessingProgress;
            set => SetProperty(ref _fileProcessingProgress, value);
        }

        /// <summary>
        /// Gets or sets the maximum number of files to be processed in the file processing operation, indicating the total number of files to be processed
        /// </summary>
        public int FileProcessingMaximum
        {
            get => _fileProcessingMaximum;
            set => SetProperty(ref _fileProcessingMaximum, value);
        }
        #endregion

        #region String Properties
        /// <summary>
        /// Gets the title of the application window, which includes the current file path and an asterisk (*) if the text has been changed since the last save operation.
        /// </summary>
        public string Title
        {
            get
            {
                _title = "";

                if (_isTextChanged) _title += "*";
                if (String.IsNullOrEmpty(_currentPath))
                {
                    _title += "SimplePad";
                }
                else
                {
                    _title += $"{_currentPath} - SimplePad";
                }

                return _title;
            }
        }

        /// <summary>
        /// Gets the short title of the application window, which includes the current file name and an asterisk (*) if the text has been changed since the last save operation.
        /// </summary>
        public string ShortTitle
        {
            get
            {
                _title = "";

                if (_isTextChanged) _title += "*";
                if (String.IsNullOrEmpty(_currentPath))
                {
                    _title += "SimplePad";
                }
                else
                {
                    _title += $"{Path.GetFileName(_currentPath)} - SimplePad";
                }

                return _title;
            }
        }

        /// <summary>
        /// Gets or sets the current file path of the document being edited in the <see cref="MainTextDocument"/>. 
        /// <br/><br/>
        /// This property is used to display the file path in the application window title and to determine where to save the document.
        /// </summary>
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                SetProperty(ref _currentPath, value);
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(ShortTitle));
            }
        }

        /// <summary>
        /// Gets or sets the text to be searched for in the <see cref="MainTextDocument"/> or across multiple files during a search operation.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// Gets or sets the text to replace the found occurrences of <see cref="SearchText"/> in the <see cref="MainTextDocument"/> or across multiple files during a replace operation.
        /// </summary>
        public string ReplaceText
        {
            get => _replaceText;
            set => SetProperty(ref _replaceText, value);
        }

        /// <summary>
        /// Gets or sets the directory path where file processing operations (searching or replacing in files) will be performed. 
        /// <br/><br/>
        /// This property is used to specify the root directory for file processing operations.
        /// </summary>
        public string FileProcessingDirectory
        {
            get => _fileProcessingDirectory;
            set => SetProperty(ref _fileProcessingDirectory, value);
        }

        /// <summary>
        /// Gets or sets the current file being processed during a file processing operation (searching or replacing in files).
        /// </summary>
        public string FileProcessingCurrent
        {
            get => _fileProcessingCurrent;
            set => SetProperty(ref _fileProcessingCurrent, value);
        }

        /// <summary>
        /// Gets or sets the line number to which the user wants to navigate in the <see cref="MainTextDocument"/> during a "Go To Line" operation.
        /// </summary>
        public string GotoText
        {
            get => _goToLineText;
            set => SetProperty(ref _goToLineText, value);
        }

        /// <summary>
        /// Gets or sets the title of the search results section, which displays the number of matches found during a search operation across multiple files.
        /// </summary>
        public string SearchResultsTitle
        {
            get => _searchResultsTitle;
            set => SetProperty(ref _searchResultsTitle, value);
        }
        #endregion

        #region Collection Properties
        /// <summary>
        /// Gets the static list of file extensions loaded from the <i><b>extensions.json</b></i>
        /// </summary>
        public ObservableCollection<FileExtensionItem> FileExtensionItems
        {
            get
            {
                if (_fileExtensions.Count != 0) return _fileExtensions;

                List<string> extensionsStr = JsonCommon.GetDeserializedJson<List<string>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\JSONLists\\extensions.json"));

                _fileExtensions = new(extensionsStr.Select((extension, i) => new FileExtensionItem
                {
                    IsChecked = false,
                    Extension = extension
                }));

                return _fileExtensions;
            }
        }
        #endregion

        #region TextDocument Properties
        /// <summary>
        /// Gets the main text document being edited in the application. 
        /// </summary>
        public TextDocument MainTextDocument
        {
            get => _mainTextDocument;
        }

        /// <summary>
        /// Gets the text document used to display search results from file processing operations (searching or replacing in files).
        /// </summary>
        public TextDocument SearchResultsDocument
        {
            get => _searchResultsDocument;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class and loads user settings for the application.
        /// </summary>
        public MainViewModel()
        {
            _mainTextDocument.TextChanged += OnTextChanged;

            LoadSettings();
        }
        #endregion

        #region File Commands
        /// <summary>
        /// Command to open a file using the file dialogue service. 
        /// <br/><br/>
        /// If the text has been changed since the last save, it prompts the user to save changes before opening a new file.
        /// </summary>
        [RelayCommand]
        public async Task OpenCommand()
        {
            List<string>? paths = await FileDialogueService.OpenFileAsync(false);

            if (paths == null) return;
            if (paths.Count == 0) return;

            if (IsTextChanged)
            {
                if (!await CallSaveWarning()) return;
            }

            OpenFile(paths[0]);
        }

        /// <summary>
        /// Command to save the current text in the <see cref="MainTextDocument"/> to the file specified by <see cref="CurrentPath"/>.
        /// </summary>
        [RelayCommand]
        public async Task SaveCommand()
        {
            try
            {
                if (!File.Exists(CurrentPath))
                {
                    await SaveAsCommand();
                    return;
                }

                File.WriteAllText(CurrentPath, MainTextDocument.Text);

                _initialText = MainTextDocument.Text;
                IsTextChanged = false;
            }
            catch (UnauthorizedAccessException)
            {
                await SaveAsCommand();
            }
        }

        /// <summary>
        /// Command to rename or move the current file.
        /// </summary>
        [RelayCommand]
        public async Task RenameOrMoveCommand()
        {
            try
            {
                string? path = await FileDialogueService.SaveFileAsync();

                if (String.IsNullOrEmpty(path)) return;

                File.Move(CurrentPath, path);

                CurrentPath = path;
            }
            catch (UnauthorizedAccessException)
            {
                await MessageBoxService.ShowErrorDialogue("Error", "You don't have permission to rename or move this file.");
            }
        }

        /// <summary>
        /// Command to delete the current file.
        /// </summary>
        [RelayCommand]
        public async Task DeleteCommand()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    FileSystem.DeleteFile(CurrentPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("gio", $"trash \"{CurrentPath}\"");
                }

                CurrentPath = "";
                _initialText = "";
                IsTextChanged = true;
            }
            catch (UnauthorizedAccessException)
            {
                await MessageBoxService.ShowErrorDialogue("Error", "You don't have permission to delete this file.");
            }
        }

        /// <summary>
        /// Command to toggle the word wrap setting in the <see cref="MainTextDocument"/>.
        /// </summary>
        [RelayCommand]
        public void WordWrapCommand()
        {
            IsWordWrapEnabled = !IsWordWrapEnabled;
        }

        /// <summary>
        /// Command to save the current text in the <see cref="MainTextDocument"/> to a new file.
        /// </summary>
        [RelayCommand]
        public async Task SaveAsCommand()
        {
            string? path = await FileDialogueService.SaveFileAsync();

            if (String.IsNullOrEmpty(path)) return;

            CurrentPath = path;

            File.WriteAllText(CurrentPath, MainTextDocument.Text);

            _initialText = MainTextDocument.Text;
            IsTextChanged = false;
        }
        #endregion

        #region Search Commands
        /// <summary>
        /// Command to perform a search operation in the <see cref="MainTextDocument"/> based on the current search settings.
        /// </summary>
        [RelayCommand]
        public void FindCommand()
        {
            if (!IsUpDirection)
            {
                FindNextCalled?.Invoke(this, new(SearchText, IsMatchCase, IsMatchWholeWords));
            }
            else
            {
                FindPreviousCalled?.Invoke(this, new(SearchText, IsMatchCase, IsMatchWholeWords));
            }
        }

        /// <summary>
        /// Command to perform a search operation in the <see cref="MainTextDocument"/> for the next occurrence of the search text based on the current search settings.
        /// </summary>
        [RelayCommand]
        public void FindNextCommand()
        {
            FindNextCalled?.Invoke(this, new(SearchText, IsMatchCase, IsMatchWholeWords));
        }

        /// <summary>
        /// Command to perform a search operation in the <see cref="MainTextDocument"/> for the previous occurrence of the search text based on the current search settings.
        /// </summary>
        [RelayCommand]
        public void FindPreviousCommand()
        {
            FindPreviousCalled?.Invoke(this, new(SearchText, IsMatchCase, IsMatchWholeWords));
        }

        /// <summary>
        /// Command to perform a replace operation in the <see cref="MainTextDocument"/> based on the current search and replace settings.
        /// </summary>
        [RelayCommand]
        public void ReplaceCommand()
        {
            if (!IsUpDirection)
            {
                ReplaceNextCalled?.Invoke(this, new(SearchText, ReplaceText, IsMatchCase, IsMatchWholeWords));
            }
            else
            {
                ReplacePreviousCalled?.Invoke(this, new(SearchText, ReplaceText, IsMatchCase, IsMatchWholeWords));
            }
        }

        /// <summary>
        /// Command to replace all occurrences of the search text with the replace text in the <see cref="MainTextDocument"/> based on the current search and replace settings.
        /// </summary>
        [RelayCommand]
        public void ReplaceAllCommand()
        {
            ReplaceAllCalled?.Invoke(this, new(SearchText, ReplaceText, IsMatchCase, IsMatchWholeWords));
        }

        /// <summary>
        /// Command to navigate to a specific line number in the <see cref="MainTextDocument"/>.
        /// </summary>
        [RelayCommand]
        public void GotoCommand()
        {
            int gotoLineNumber = Int32.Parse(GotoText);

            if (gotoLineNumber > LineCount)
            {
                GotoText = LineCount.ToString();
            }

            GotoCalled?.Invoke(this, gotoLineNumber);
        }
        #endregion

        #region File processing commands
        /// <summary>
        /// Command to find all occurrences of the <see cref="SearchText"/> in files within the specified directory.
        /// </summary>
        [RelayCommand]
        public async Task FindInFilesCommand()
        {
            // Set up cancellation token for the file processing operation
            var cancellationToken = SetupCancellationToken();
            //

            if (!ValidateInputParameters()) return;

            // Normalize line transitions
            var searchText = NormalizeLineEndings(SearchText);
            //

            FindInFilesCalled?.Invoke(this, new(searchText, IsMatchCase, IsMatchWholeWords));

            var (files, selectedExtensions) = GetFilesForProcessing();
            if (files.Count == 0) return;

            // Set up UI for processing
            await InitializeUIForProcessing(files.Count);
            //

            await Task.Run(async () =>
            {
                int matchesCount = 0;
                var strategy = SearchStrategyFactory.Create(searchText, !IsMatchCase, IsMatchWholeWords, SearchMode.Normal);

                // Pre-allocate capacity
                var resultsBuilder = new StringBuilder(8192);
                //
                // Pre-allocate larger capacity for batch
                var batchBuilder = new StringBuilder(16384);
                //
                // Batch size for UI updates
                const int batchSize = 10;
                //
                // Track actual processed files
                int filesProcessed = 0;
                //

                try
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        string file = files[i];
                        await UpdateCurrentFileOnUI(file);

                        try
                        {
                            // Read the file content asynchronously to avoid blocking the UI thread
                            string? fileContent = await File.ReadAllTextAsync(file, cancellationToken);

                            // Skip empty files to save memory
                            if (string.IsNullOrEmpty(fileContent))
                            {
                                filesProcessed++;
                                continue;
                            }

                            TextDocument? textDocument = new(fileContent);

                            // Clear reference to file content to allow GC
                            fileContent = null;
                            //

                            // Search for matches
                            List<ISearchResult>? matches = strategy.FindAll(textDocument, 0, textDocument.TextLength).ToList();
                            int matchCount = matches.Count;

                            if (matchCount > 0)
                            {
                                resultsBuilder.Append(file).Append('\n');
                            }

                            matchesCount += matchCount;

                            // Build results for this file
                            BuildSearchResults(matches, textDocument, resultsBuilder, i, files.Count, cancellationToken);
                            //

                            // Clear references to help GC
                            matches.Clear();
                            matches = null;
                            textDocument = null;
                            //

                            // Accumulate results in batch builder
                            batchBuilder.Append(resultsBuilder.ToString());
                            resultsBuilder.Clear();

                            filesProcessed++;
                            //

                            // Update UI in batches to reduce the number of UI thread invocations
                            if (batchBuilder.Length > 0 && (filesProcessed % batchSize == 0 || i == files.Count - 1))
                            {
                                var batchText = batchBuilder.ToString();
                                batchBuilder.Clear();

                                await UpdateSearchResultsOnUI(batchText, filesProcessed);
                            }
                            //

                            if (matchCount > 0 && i != files.Count - 1)
                            {
                                resultsBuilder.Append('\n');
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Cancellation requested, exit gracefully
                            break;
                            //
                        }
                        catch (Exception ex)
                        {
                            await ShowErrorOnUI(file, ex);
                        }
                    }

                    // Final UI update for any remaining results
                    if (batchBuilder.Length > 0)
                    {
                        var finalBatch = batchBuilder.ToString();
                        batchBuilder.Clear();

                        await UpdateSearchResultsOnUI(finalBatch, filesProcessed);
                    }
                    //
                }
                finally
                {
                    // Clean up resources
                    CleanupResources(resultsBuilder, batchBuilder, files, selectedExtensions, strategy);
                    //
                }

                await FinalizeSearchOnUI(matchesCount, filesProcessed);
            }, cancellationToken);
        }

        /// <summary>
        /// Command to replace all occurrences of the <see cref="SearchText"/> with the <see cref="ReplaceText"/> in files within the specified directory.
        /// </summary>
        [RelayCommand]
        public async Task ReplaceInFilesCommand()
        {
            // Prompt the user for confirmation before proceeding with the replace operation
            var result = await MessageBoxService.ShowWarningDialogue("Warning", "Are you sure to replace in files? This action cannot be undone.", ["Yes", "No"]);
            if (result != "Yes") return;
            //

            // Set up cancellation token for the file processing operation
            var cancellationToken = SetupCancellationToken();
            //

            if (!ValidateInputParameters()) return;

            // Normalize line transitions
            var searchText = NormalizeLineEndings(SearchText);
            var replaceText = NormalizeLineEndings(ReplaceText);
            //

            ReplaceInFilesCalled?.Invoke(this, new(searchText, replaceText, IsMatchCase, IsMatchWholeWords));

            var (files, selectedExtensions) = GetFilesForProcessing();
            if (files.Count == 0) return;

            // Set up UI for processing
            await InitializeUIForProcessing(files.Count);
            //

            await Task.Run(async () =>
            {
                int matchesCount = 0;
                var strategy = SearchStrategyFactory.Create(searchText, !IsMatchCase, IsMatchWholeWords, SearchMode.Normal);

                // Pre-allocate capacity
                var resultsBuilder = new StringBuilder(8192);
                //
                // Pre-allocate larger capacity for batch
                var batchBuilder = new StringBuilder(16384);
                //
                // Batch size for UI updates
                const int batchSize = 10;
                //
                // Track actual processed files
                int filesProcessed = 0;
                //

                try
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        string file = files[i];
                        await UpdateCurrentFileOnUI(file);

                        try
                        {
                            // Read the file content asynchronously to avoid blocking the UI thread
                            string? fileContent = await File.ReadAllTextAsync(file, cancellationToken);

                            // Skip empty files to save memory
                            if (string.IsNullOrEmpty(fileContent))
                            {
                                filesProcessed++;
                                continue;
                            }

                            TextDocument? textDocument = new(fileContent);

                            // Clear reference to file content to allow GC
                            fileContent = null;
                            //

                            // Search for matches
                            List<ISearchResult>? matches = strategy.FindAll(textDocument, 0, textDocument.TextLength).ToList();
                            int matchCount = matches.Count;

                            if (matchCount > 0)
                            {
                                resultsBuilder.Append(file).Append('\n');
                            }

                            matchesCount += matchCount;

                            // Build replacement results and apply changes
                            BuildReplacementResults(matches, textDocument, replaceText, resultsBuilder, i, files.Count, cancellationToken);

                            // Save modified file if replacements were made
                            if (matchCount > 0 && !cancellationToken.IsCancellationRequested)
                            {
                                await File.WriteAllTextAsync(file, textDocument.Text, cancellationToken);
                            }
                            //

                            // Clear references to help GC
                            matches.Clear();
                            matches = null;
                            textDocument = null;
                            //

                            // Accumulate results in batch builder
                            batchBuilder.Append(resultsBuilder.ToString());
                            resultsBuilder.Clear();

                            filesProcessed++;
                            //

                            // Update UI in batches to reduce the number of UI thread invocations
                            if (batchBuilder.Length > 0 && (filesProcessed % batchSize == 0 || i == files.Count - 1))
                            {
                                var batchText = batchBuilder.ToString();
                                batchBuilder.Clear();

                                await UpdateSearchResultsOnUI(batchText, filesProcessed);
                            }
                            //

                            if (matchCount > 0 && i != files.Count - 1)
                            {
                                resultsBuilder.Append('\n');
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Cancellation requested, exit gracefully
                            break;
                            //
                        }
                        catch (Exception ex)
                        {
                            await ShowErrorOnUI(file, ex);
                        }
                    }

                    // Final UI update for any remaining results
                    if (batchBuilder.Length > 0)
                    {
                        var finalBatch = batchBuilder.ToString();
                        batchBuilder.Clear();

                        await UpdateSearchResultsOnUI(finalBatch, filesProcessed);
                    }
                    //
                }
                finally
                {
                    // Clean up resources
                    CleanupResources(resultsBuilder, batchBuilder, files, selectedExtensions, strategy);
                    //
                }

                await FinalizeReplaceOnUI(matchesCount, filesProcessed);
            }, cancellationToken);
        }

        /// <summary>
        /// Command to set the current directory for file processing operations.
        /// </summary>
        [RelayCommand]
        public void FileProcessingSetCurrentDirectoryCommand()
        {
            if (_currentPath != null)
            {
                string? directory = Path.GetDirectoryName(_currentPath);

                if (directory != null)
                {
                    FileProcessingDirectory = directory;
                }
            }
        }

        /// <summary>
        /// Command to open a folder dialogue and set the selected directory for file processing operations.
        /// </summary>
        [RelayCommand]
        public async Task FileProcessingSetDirectoryCommand()
        {
            List<string>? paths = await FileDialogueService.OpenFolderAsync(false);

            if (paths == null) return;
            if (paths.Count == 0) return;

            FileProcessingDirectory = paths[0];
        }

        /// <summary>
        /// Command to cancel the ongoing file processing operation (searching or replacing in files) by signaling the cancellation token.
        /// </summary>
        [RelayCommand]
        public void FileProcessingCancelCommand()
        {
            _fileProcessingCancellationTokenSource?.Cancel();
            FileProcessingCancelCalled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Command to finalize the file processing operation (searching or replacing in files) and reset the processing properties to their default values.
        /// </summary>
        [RelayCommand]
        public void FileProcessingFinishCommand()
        {
            FileProcessingDone?.Invoke(this, EventArgs.Empty);

            // Set processing properties to default
            FileProcessingCurrent = "Counting files...";
            FileProcessingProgress = 0;
            FileProcessingMaximum = 1;
            //
        }

        /// <summary>
        /// Command to toggle the selection state of all file extensions in the <see cref="FileExtensionItems"/> collection.
        /// </summary>
        [RelayCommand]
        public void SwitchExtensionsSelectionCommand()
        {
            if (FileExtensionItems.Any(item => !item.IsChecked))
            {
                foreach (var extensionItem in FileExtensionItems)
                {
                    extensionItem.IsChecked = true;
                }
            }
            else
            {
                foreach (var extensionItem in FileExtensionItems)
                {
                    extensionItem.IsChecked = false;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads user settings from the <see cref="SettingsService"/> and applies them to the properties of the <see cref="MainViewModel"/>.
        /// </summary>
        public void LoadSettings()
        {
            IsInAllSubfolders = SettingsService.UserSettings.FunctionalSettings.IsInAllSubfolders;

            IsMatchCase = SettingsService.UserSettings.FunctionalSettings.IsMatchCase;
            IsMatchWholeWords = SettingsService.UserSettings.FunctionalSettings.IsMatchWholeWords;
            IsMultipleLineInput = SettingsService.UserSettings.FunctionalSettings.IsMultipleLineInput;

            IsUpDirection = SettingsService.UserSettings.FunctionalSettings.IsUpDirection;

            IsWordWrapEnabled = SettingsService.UserSettings.FunctionalSettings.IsWordWrapEnabled;

            SearchText = SettingsService.UserSettings.FunctionalSettings.SearchText;
            ReplaceText = SettingsService.UserSettings.FunctionalSettings.ReplaceText;

            FileProcessingDirectory = SettingsService.UserSettings.FunctionalSettings.FileProcessingDirectory;

            foreach (string extension in SettingsService.UserSettings.FunctionalSettings.SelectedExtensions)
            {
                foreach (var item in FileExtensionItems.Where(item => item.Extension == extension))
                {
                    item.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// Saves the current settings from the properties of the <see cref="MainViewModel"/> to the <see cref="SettingsService"/> for persistence across application sessions.
        /// </summary>
        public void SaveSettings()
        {
            SettingsService.UserSettings.FunctionalSettings.IsInAllSubfolders = IsInAllSubfolders;

            SettingsService.UserSettings.FunctionalSettings.IsMatchCase = IsMatchCase;
            SettingsService.UserSettings.FunctionalSettings.IsMatchWholeWords = IsMatchWholeWords;
            SettingsService.UserSettings.FunctionalSettings.IsMultipleLineInput = IsMultipleLineInput;

            SettingsService.UserSettings.FunctionalSettings.IsUpDirection = IsUpDirection;

            SettingsService.UserSettings.FunctionalSettings.IsWordWrapEnabled = IsWordWrapEnabled;

            SettingsService.UserSettings.FunctionalSettings.SearchText = SearchText;
            SettingsService.UserSettings.FunctionalSettings.ReplaceText = ReplaceText;

            SettingsService.UserSettings.FunctionalSettings.FileProcessingDirectory = FileProcessingDirectory;

            SettingsService.UserSettings.FunctionalSettings.SelectedExtensions = [];
            foreach (var item in FileExtensionItems.Where(item => item.IsChecked))
            {
                SettingsService.UserSettings.FunctionalSettings.SelectedExtensions.Add(item.Extension);
            }
        }

        /// <summary>
        /// Displays a warning dialogue to the user before saving changes to a file.
        /// </summary>
        /// <returns>True if the user chooses to save, false otherwise.</returns>
        public async Task<bool> CallSaveWarning()
        {
            string message;

            if (string.IsNullOrEmpty(CurrentPath))
            {
                message = "Do you want to save text to file?";
            }
            else
            {
                message = $"Do you want to save changes to \"{CurrentPath}\"?";
            }

            string result = await MessageBoxService.ShowWarningDialogue("Warning!", message, ["Save", "Don't save", "Cancel"]);

            if (result == "Save")
            {
                if (string.IsNullOrEmpty(CurrentPath))
                {
                    await SaveAsCommand();
                }
                else
                {
                    await SaveCommand();
                }
                return true;
            }
            else if (result == "Don't save")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Opens a file at the specified <paramref name="path"/> and loads its content into the <see cref="MainTextDocument"/>.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public void OpenFile(string path)
        {
            CurrentPath = path;

            MainTextDocument.Text = File.ReadAllText(CurrentPath);

            _initialText = MainTextDocument.Text;
            IsTextChanged = false;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the TextChanged event of the <see cref="MainTextDocument"/>.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Updates the <see cref="IsTextChanged"/> property and raises the <see cref="TextChanged"/> event when the text in the document changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object? sender, EventArgs e)
        {
            IsTextChanged = MainTextDocument.Text != _initialText;
            TextChanged?.Invoke(MainTextDocument, e);

            OnPropertyChanged(nameof(LineCount));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets up the cancellation token for file processing operations.
        /// </summary>
        private CancellationToken SetupCancellationToken()
        {
            _fileProcessingCancellationTokenSource?.Cancel();
            _fileProcessingCancellationTokenSource?.Dispose();
            _fileProcessingCancellationTokenSource = new CancellationTokenSource();
            return _fileProcessingCancellationTokenSource.Token;
        }

        /// <summary>
        /// Validates the input parameters for file processing.
        /// </summary>
        private bool ValidateInputParameters()
        {
            if (String.IsNullOrEmpty(FileProcessingDirectory)) return false;
            if (String.IsNullOrEmpty(SearchText)) return false;
            if (!Directory.Exists(FileProcessingDirectory)) return false;
            return true;
        }

        /// <summary>
        /// Normalizes line endings in the specified text.
        /// </summary>
        private string NormalizeLineEndings(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        /// <summary>
        /// Gets the list of files to process based on selected extensions.
        /// </summary>
        private (List<string> files, List<string> selectedExtensions) GetFilesForProcessing()
        {
            List<string>? selectedExtensions = FileExtensionItems.Where(item => item.IsChecked).Select(item => item.Extension).ToList();
            List<string>? files = FileUtils.GetFiles(FileProcessingDirectory, IsInAllSubfolders, selectedExtensions);
            return (files, selectedExtensions);
        }

        /// <summary>
        /// Initializes the UI for file processing.
        /// </summary>
        private async Task InitializeUIForProcessing(int fileCount)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                FileProcessingMaximum = fileCount;
                SearchResultsDocument.Text = "";

                // Force garbage collection of previous results
                SearchResultsDocument.UndoStack.ClearAll();
                //
            });
        }

        /// <summary>
        /// Updates the current file being processed on the UI.
        /// </summary>
        private async Task UpdateCurrentFileOnUI(string file)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                FileProcessingCurrent = file;
            });
        }

        /// <summary>
        /// Builds search results for a file.
        /// </summary>
        private void BuildSearchResults(List<ISearchResult> matches, TextDocument textDocument, StringBuilder resultsBuilder,
            int fileIndex, int totalFiles, CancellationToken cancellationToken)
        {
            int previousLineNumber = -1;

            for (int j = 0; j < matches.Count; j++)
            {
                var match = matches[j];
                DocumentLine line = textDocument.GetLineByOffset(match.Offset);

                if (line.PreviousLine != null)
                {
                    if (previousLineNumber == line.PreviousLine.LineNumber) continue;
                    previousLineNumber = line.PreviousLine.LineNumber;
                }

                string lineText = textDocument.GetText(line.Offset, line.Length);

                resultsBuilder.Append($"line {line.LineNumber}: {lineText}");

                if (fileIndex != totalFiles - 1 || j != matches.Count - 1)
                {
                    resultsBuilder.Append('\n');
                }

                if (cancellationToken.IsCancellationRequested) break;
            }
        }

        /// <summary>
        /// Builds replacement results and applies changes to the document.
        /// </summary>
        private void BuildReplacementResults(List<ISearchResult> matches, TextDocument textDocument, string replaceText,
            StringBuilder resultsBuilder, int fileIndex, int totalFiles, CancellationToken cancellationToken)
        {
            int previousLineNumber = -1;

            for (int j = 0; j < matches.Count; j++)
            {
                var match = matches[j];
                DocumentLine line = textDocument.GetLineByOffset(match.Offset);

                if (line.PreviousLine != null)
                {
                    if (previousLineNumber == line.PreviousLine.LineNumber) continue;
                    previousLineNumber = line.PreviousLine.LineNumber;
                }

                string originalLineText = textDocument.GetText(line.Offset, line.Length);

                // Apply all replacements in this line
                string newLineText = originalLineText;
                var lineMatches = matches.Where(m => textDocument.GetLineByOffset(m.Offset).LineNumber == line.LineNumber)
                    .OrderByDescending(m => m.Offset).ToList();

                foreach (var lineMatch in lineMatches)
                {
                    int matchPositionInLine = lineMatch.Offset - line.Offset;
                    newLineText = newLineText.Substring(0, matchPositionInLine) + replaceText +
                                 newLineText.Substring(matchPositionInLine + lineMatch.Length);
                }
                //

                resultsBuilder.Append($"line {line.LineNumber}: {originalLineText.Trim()} => {newLineText.Trim()}");

                if (fileIndex != totalFiles - 1 || j != matches.Count - 1)
                {
                    resultsBuilder.Append('\n');
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            // Apply all replacements to the document
            if (!cancellationToken.IsCancellationRequested)
            {
                for (int j = matches.Count - 1; j >= 0; j--)
                {
                    var match = matches[j];
                    textDocument.Replace(match.Offset, match.Length, replaceText);
                }
            }
            //
        }

        /// <summary>
        /// Updates the search results on the UI.
        /// </summary>
        private async Task UpdateSearchResultsOnUI(string results, int filesProcessed)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                FileProcessingProgress = filesProcessed;
                SearchResultsDocument.Text += results;
            });
        }

        /// <summary>
        /// Shows an error message on the UI.
        /// </summary>
        private async Task ShowErrorOnUI(string file, Exception ex)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                SearchResultsDocument.Text = $"Error reading file: {file}\n{ex.Message}\n\n{SearchResultsDocument.Text}";
            });
        }

        /// <summary>
        /// Finalizes the search operation on the UI.
        /// </summary>
        private async Task FinalizeSearchOnUI(int matchesCount, int filesProcessed)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                SearchResultsTitle = $"Search results ({matchesCount} matches in {filesProcessed} files found)";
                FileProcessingFinishCommand();
            });
        }

        /// <summary>
        /// Finalizes the replace operation on the UI.
        /// </summary>
        private async Task FinalizeReplaceOnUI(int matchesCount, int filesProcessed)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                SearchResultsTitle = $"Replace results ({matchesCount} matches replaced in {filesProcessed} files)";
                FileProcessingFinishCommand();
            });
        }

        /// <summary>
        /// Cleans up resources and forces garbage collection.
        /// </summary>
        private void CleanupResources(StringBuilder resultsBuilder, StringBuilder batchBuilder,
            List<string> files, List<string> selectedExtensions, object? strategy)
        {
            // Clean up resources
            resultsBuilder.Clear();
            batchBuilder.Clear();
            files.Clear();
            selectedExtensions.Clear();
            strategy = null;
            //

            // Force garbage collection to release memory
            GC.Collect(0, GCCollectionMode.Optimized);
            //
        }
        #endregion
    }
}