using AvaloniaEdit;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.FileIO;
using SimplePad.EventHandlers;
using SimplePad.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace SimplePad.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private bool _isTextChanged;
        private bool _isWordWrapEnabled;
        private bool _isUpDirection = false;
        private bool _isMatchCase = false;
        private bool _isMultipleLineInput = false;
        private bool _canUndo = false;
        private bool _canRedo = false;
        private string _title = "SimplePad";
        private string _currentPath = "";
        private string _intiialText = "";
        private string _findText = "";
        private string _replaceText = "";
        private string _goToLineText = "";
        private TextDocument _textDocument = new();

        public event EventHandler? TextChanged;
        public event SearchEventHandler? FindNextCalled;
        public event SearchEventHandler? FindPreviousCalled;
        public event SearchEventHandler? FindInFilesCalled;
        public event SearchEventHandler? ReplaceInFilesCalled;
        public event SearchEventHandler? ReplaceNextCalled;
        public event SearchEventHandler? ReplacePreviousCalled;
        public event SearchEventHandler? ReplaceAllCalled;
        public event EventHandler<string>? GotoCalled;

        #region Boolean Properties
        /// <summary>
        /// 
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
                }
                else
                {
                    SetProperty(ref _isTextChanged, value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWordWrapEnabled
        {
            get => _isWordWrapEnabled;
            set => SetProperty(ref _isWordWrapEnabled, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUpDirection
        {
            get => _isUpDirection;
            set => SetProperty(ref _isUpDirection, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMatchCase
        {
            get => _isMatchCase;
            set => SetProperty(ref _isMatchCase, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMultipleLineInput
        {
            get => _isMultipleLineInput;
            set => SetProperty(ref _isMultipleLineInput, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanUndo 
        { 
            get => _canUndo;
            set => SetProperty(ref _canUndo, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanRedo 
        {
            get => _canRedo;
            set => SetProperty(ref _canRedo, value);
        }
        #endregion

        #region String Properties
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                SetProperty(ref _currentPath, value);
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FindText
        {
            get => _findText;
            set => SetProperty(ref _findText, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReplaceText
        {
            get => _replaceText;
            set => SetProperty(ref _replaceText, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string GotoText
        {
            get => _goToLineText;
            set => SetProperty(ref _goToLineText, value);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public TextDocument TextDocument
        {
            get => _textDocument;
        }

        /// <summary>
        /// 
        /// </summary>
        public MainViewModel()
        {
            _textDocument.TextChanged += OnTextChanged;
        }

        #region File Commands
        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public async Task OpenCommand()
        {
            List<string>? paths = await FileDialogService.OpenFileAsync(false);

            if (paths == null) return;
            if (paths.Count == 0) return;

            if (IsTextChanged)
            {
                if (!await CallSaveWarning()) return;
            }

            CurrentPath = paths[0];

            TextDocument.Text = File.ReadAllText(CurrentPath);

            _intiialText = TextDocument.Text;
            IsTextChanged = false;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public async Task SaveCommand()
        {
            if (!File.Exists(CurrentPath))
            {
                await SaveAsCommand();
                return;
            }

            File.Create(CurrentPath);
            File.WriteAllText(CurrentPath, TextDocument.Text);

            _intiialText = TextDocument.Text;
            IsTextChanged = false;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public async Task RenameOrMoveCommand()
        {
            string? path = await FileDialogService.SaveFileAsync();

            if (String.IsNullOrEmpty(path)) return;

            File.Move(CurrentPath, path);

            CurrentPath = path;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void DeleteCommand()
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
            _intiialText = "";
            IsTextChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void WordWrapCommand()
        {
            IsWordWrapEnabled = !IsWordWrapEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public async Task SaveAsCommand()
        {
            string? path = await FileDialogService.SaveFileAsync();

            if (String.IsNullOrEmpty(path)) return;

            CurrentPath = path;

            File.WriteAllText(CurrentPath, TextDocument.Text);

            _intiialText = TextDocument.Text;
            IsTextChanged = false;
        }
        #endregion

        #region Search Commands
        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void FindCommand()
        {
            if (!IsUpDirection)
            {
                FindNextCalled?.Invoke(this, new(FindText, IsMatchCase));
            }
            else
            {
                FindPreviousCalled?.Invoke(this, new(FindText, IsMatchCase));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void FindNextCommand()
        {
            FindNextCalled?.Invoke(this, new(FindText, IsMatchCase));
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void FindPreviousCommand()
        {
            FindPreviousCalled?.Invoke(this, new(FindText, IsMatchCase));
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void FindInFilesCommand()
        {
            FindInFilesCalled?.Invoke(this, new(FindText, IsMatchCase));
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void ReplaceInFilesCommand()
        {
            ReplaceInFilesCalled?.Invoke(this, new(FindText, ReplaceText, IsMatchCase));
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void ReplaceCommand()
        {
            if (!IsUpDirection)
            {
                ReplaceNextCalled?.Invoke(this, new(FindText, ReplaceText, IsMatchCase));
            }
            else
            {
                ReplacePreviousCalled?.Invoke(this, new(FindText, ReplaceText, IsMatchCase));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void ReplaceAllCommand()
        {
            ReplaceAllCalled?.Invoke(this, new(FindText, ReplaceText, IsMatchCase));
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void GotoCommand()
        {
            GotoCalled?.Invoke(this, GotoText);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

            var messageBox = MessageBoxService.Dialogue("Warning!", message, ["Save", "Don't save", "Cancel"]);

            string result = await messageBox.ShowAsync();

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object? sender, EventArgs e)
        {
            IsTextChanged = TextDocument.Text != _intiialText;
            TextChanged?.Invoke(TextDocument, e);
        }
    }
}