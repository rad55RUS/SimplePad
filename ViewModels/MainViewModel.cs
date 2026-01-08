using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.FileIO;
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
        private string _title = "SimplePad";
        private string _currentPath = "";
        private string _intiialText = "";
        private TextDocument _textDocument = new();

        /// <summary>
        /// 
        /// </summary>
        public MainViewModel()
        {
            _textDocument.TextChanged += TextChanged;
        }

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
        public TextDocument TextDocument
        {
            get => _textDocument;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public async Task OpenCommand()
        {
            List<string>? paths = await FileDialogService.OpenFileAsync(false);

            if (paths == null) return;
            if (paths.Count == 0) return;

            CurrentPath = paths[0];

            TextDocument.Text = File.ReadAllText(CurrentPath);

            _intiialText = TextDocument.Text;
            IsTextChanged = false;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void SaveCommand()
        {
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
                    SaveCommand();
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
        /// <exception cref="NotImplementedException"></exception>
        private void TextChanged(object? sender, EventArgs e)
        {
            IsTextChanged = TextDocument.Text != _intiialText;
        }
    }
}