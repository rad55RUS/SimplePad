using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

using TextFile_Lib;

namespace SimplePad
{
    /// <summary>
    /// Interaction logic for CloseSaveWindow.cs
    /// </summary>
    public partial class CloseSaveWindow : Window
    {
        // Contants
        private const uint WP_SYSTEMMENU = 0x02;
        private const uint WM_SYSTEMMENU = 0xa4;
        internal string ?result;
        //

        // Initialization
        public CloseSaveWindow()
        {
            InitializeComponent();
        }

        protected virtual void OnLoad(object sender, RoutedEventArgs e)
        {
            // WindowChrome
            IntPtr windIntPtr = new WindowInteropHelper(this).Handle;
            HwndSource hwndSource = HwndSource.FromHwnd(windIntPtr);
            hwndSource.AddHook(new HwndSourceHook(WndProc));

            WindowChrome.SetIsHitTestVisibleInChrome(this.CloseButton, true);
            //
        }
        //

        // Window methods
        //// Fixing default context menu via creating another one
        /// <summary>
        /// Prevent bugged default title bar context menu from appearing
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (((msg == WM_SYSTEMMENU) && (wparam.ToInt32() == WP_SYSTEMMENU)) || msg == 165)
            {
                TitleBar_ContextMenu.IsOpen = true;

                handled = true;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Prevent bugged default title bar context menu from appearing on Alt+Space;
        /// <br/>//<br/>
        /// Save text file on ctrl+s;
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // ALT + SPACE
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.Space)
            {
                TitleBar_ContextMenu.IsOpen = true;

                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }
        ////

        //// Window operations
        /// <summary>
        /// Close window event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
        ////
        //

        // Buttons
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            result = "Save";
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            result = "Cancel";
            Close();
        }
        //
    }
}