using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using SimplePad.Services;
using SimplePad.ViewModels;
using System;
using System.Threading.Tasks;


namespace SimplePad.Views
{
    public partial class MainWindow : Window
    {
        private bool _forceClose = false;
        private WindowState _state = WindowState.Minimized;

        /// <summary>
        /// 
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

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

            AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.F:
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                        {
                            OnFind(this, new RoutedEventArgs());
                        }
                        else
                        {
                            OnFindInFiles(this, new RoutedEventArgs());
                        }
                        e.Handled = true;
                        break;
                    case Key.H:
                        OnReplace(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.G:
                        OnGoto(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                }
            }
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
                    if (_forceClose) Close();
                }
            }
        }

        #region Search Window Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFind(object? sender, RoutedEventArgs e)
        {
            SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 0;
            SearchWindowService.SearchWindow.Show(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindInFiles(object? sender, RoutedEventArgs e)
        {
            SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 2;
            SearchWindowService.SearchWindow.Show(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplace(object? sender, RoutedEventArgs e)
        {
            SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 1;
            SearchWindowService.SearchWindow.Show(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoto(object? sender, RoutedEventArgs e)
        {
            SearchWindowService.SearchWindow.SearchTabControl.SelectedIndex = 3;
            SearchWindowService.SearchWindow.Show(this);
        }
        #endregion
    }
}