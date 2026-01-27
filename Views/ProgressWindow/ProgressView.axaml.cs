using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using SimplePad.ViewModels;
using System;

namespace SimplePad.Views
{
    public partial class ProgressView : ViewBase
    {
        /// <summary>
        /// 
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// 
        /// </summary>
        public ProgressView()
        {
            InitializeComponent();
        }
    }
}