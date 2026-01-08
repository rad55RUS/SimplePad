using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;

namespace SimplePad.Views
{
    public partial class MainWindow : Window
    {
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
        private void Restore_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Resized(object? sender, WindowResizedEventArgs e)
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
    }
}