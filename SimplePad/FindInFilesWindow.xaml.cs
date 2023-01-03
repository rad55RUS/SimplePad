using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Forms;

using TextFile_Lib;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.DirectoryServices;

// TODO: In all sub folders, in hidden folders options;
// TODO: Add fast selection of current directory;
// TODO: Make comments and readibility
// TODO: Make result window closing
// TODO: GlobalHook must initialize only after making result window visible!
namespace SimplePad
{
    /// <summary>
    /// Interaction logic for CloseSaveWindow.cs
    /// </summary>
    public partial class FindInFilesWindow : Window
    {
        // Contants
        private const uint WP_SYSTEMMENU = 0x02;
        private const uint WM_SYSTEMMENU = 0xa4;
        //

        // Private fields
        private DispatcherTimer ?dispatcherTimer;
        //

        // Initialization
        public FindInFilesWindow()
        {
            InitializeComponent();
        }

        protected virtual void OnLoad(object sender, RoutedEventArgs e)
        {
            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
            //

            // WindowChrome
            IntPtr windIntPtr = new WindowInteropHelper(this).Handle;
            HwndSource hwndSource = HwndSource.FromHwnd(windIntPtr);
            hwndSource.AddHook(new HwndSourceHook(WndProc));
            //
        }
        //

        // Updating data
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            progressBar.Value = SearchResultsData.ProgressBar_Value;
            progressBar.Maximum = SearchResultsData.ProgressBar_Maximum;
            currentFile.Content = SearchResultsData.Label_Content;
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
    }
}