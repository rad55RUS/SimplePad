using System;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using SimplePad.ViewModels;


namespace SimplePad.Views
{
    public partial class MainWindow : Window
    {
        private bool _forceClose = false;
        private WindowState _state = WindowState.Minimized;

        /// <summary>
        /// 
        /// </summary>
        public WindowState State
        {
            get => _state;
            set
            {
                _state = value;

                if (WindowState != value)
                {
                    WindowState = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRestore(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimize(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMaximize(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResized(object? sender, WindowResizedEventArgs e)
        {
            if (WindowState == State) return;

            State = WindowState;

            RestoreMenuItem.Classes.Clear();
            MaximizeMenuItem.Classes.Clear();

            switch (State)
            {
                case WindowState.Normal:
                    RestoreMenuItem.Classes.Add("Unclickable");
                    break;
                case WindowState.Maximized:
                    MaximizeMenuItem.Classes.Add("Unclickable");
                    break;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClosing(object? sender, WindowClosingEventArgs e)
        {
            if (_forceClose) return;

            if (DataContext is MainViewModel dataContext)
            {
                if (dataContext.IsTextChanged)
                {
                    e.Cancel = true;

                    _forceClose = await dataContext.CallSaveWarning();
                    Close();
                }
            }
        }
    }
}