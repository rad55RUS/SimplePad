using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AvaloniaEdit.Document;

using CommunityToolkit.Mvvm.Input;

using SimplePad.Services;


namespace SimplePad.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private string _title = "SimplePad";
        private string _currentPath = "";
        private TextDocument _textDocument = new();

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
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
                Title = $"{value} - SimplePad";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextDocument TextDocument
        {
            get => _textDocument;
            set => SetProperty(ref _textDocument, value);
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
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        public void SaveCommand()
        {
            File.WriteAllText(CurrentPath, TextDocument.Text);
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
        }
    }
}