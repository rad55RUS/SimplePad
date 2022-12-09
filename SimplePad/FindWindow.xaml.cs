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
    public partial class FindWindow : Window
    {
        // Contants
        private const uint WP_SYSTEMMENU = 0x02;
        private const uint WM_SYSTEMMENU = 0xa4;
        //

        // Initialization
        public FindWindow()
        {
            InitializeComponent();
        }

        protected virtual void OnLoad(object sender, RoutedEventArgs e)
        {
            // Load settings
            this.findInput_TextBox.Text = Properties.Settings.Default.DesiredString;
            this.replaceInput_TextBox.Text = Properties.Settings.Default.ReplaceTo_String;
            this.matchCase_CheckBox.IsChecked = Properties.Settings.Default.MatchCase;
            this.multipleLine_CheckBox.IsChecked = Properties.Settings.Default.MultipleLineInput;
            this.down_RadioButton.IsChecked = Properties.Settings.Default.SearchDirectionIsDown;
            this.down_RadioButton.IsChecked = Properties.Settings.Default.SearchDirectionIsDown;
            this.up_RadioButton.IsChecked = !Properties.Settings.Default.SearchDirectionIsDown;
            //

            if (multipleLine_CheckBox.IsChecked == true)
            {
                findInput_TextBox.AcceptsReturn = true;
                replaceInput_TextBox.AcceptsReturn = true;
                this.findButton.Focus();
            }
            else
            {
                findInput_TextBox.AcceptsReturn = false;
                replaceInput_TextBox.AcceptsReturn = false;
            }

            // WindowChrome
            IntPtr windIntPtr = new WindowInteropHelper(this).Handle;
            HwndSource hwndSource = HwndSource.FromHwnd(windIntPtr);
            hwndSource.AddHook(new HwndSourceHook(WndProc));

            WindowChrome.SetIsHitTestVisibleInChrome(this.CloseButton, true);
            WindowChrome.SetIsHitTestVisibleInChrome(this.find_TabItem, true);
            WindowChrome.SetIsHitTestVisibleInChrome(this.replace_TabItem, true);
            WindowChrome.SetIsHitTestVisibleInChrome(this.findInFiles_TabItem, true);
            //
        }
        //

        // This window events
        /// <summary>
        /// Event on changing matchCase_CheckBox status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (find_TabItem.IsSelected == true)
            {
                replaceInput_TextBox.Visibility = Visibility.Hidden;
                replaceInput_Label.Visibility = Visibility.Hidden;
                replaceButton.Visibility = Visibility.Hidden;

                direction_Label.Visibility = Visibility.Visible;
                direction_Rectangle.Visibility = Visibility.Visible;
                up_RadioButton.Visibility = Visibility.Visible;
                down_RadioButton.Visibility = Visibility.Visible;

                findButton.Content = "Find next";

                this.Height = 170;
            }
            else if (replace_TabItem.IsSelected == true)
            {
                replaceInput_TextBox.Visibility = Visibility.Visible;
                replaceInput_Label.Visibility = Visibility.Visible;
                replaceButton.Visibility = Visibility.Visible;

                direction_Label.Visibility = Visibility.Visible;
                direction_Rectangle.Visibility = Visibility.Visible;
                up_RadioButton.Visibility = Visibility.Visible;
                down_RadioButton.Visibility = Visibility.Visible;

                findButton.Content = "Find next";

                this.Height = 217;
            }
            else if (findInFiles_TabItem.IsSelected == true)
            {
                replaceInput_TextBox.Visibility = Visibility.Visible;
                replaceInput_Label.Visibility = Visibility.Visible;
                replaceButton.Visibility = Visibility.Visible;

                direction_Label.Visibility = Visibility.Hidden;
                direction_Rectangle.Visibility = Visibility.Hidden;
                up_RadioButton.Visibility = Visibility.Hidden;
                down_RadioButton.Visibility = Visibility.Hidden;

                findButton.Content = "Find";

                this.Height = 217;
            }

            findInput_TextBox.Focus();
            findInput_TextBox.SelectAll();
        }

        /// <summary>
        /// Event on changing matchCase_CheckBox status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void matchCase_Change(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).searchResults.Clear();
            this.findButton.Focus();
        }

        /// <summary>
        /// Event on changing radio button status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void direction_Changed(object sender, RoutedEventArgs e)
        {
            this.findButton.Focus();
        }

        /// <summary>
        /// Event on changing multipleLine_CheckBox unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multipleLine_Unchecked(object sender, RoutedEventArgs e)
        {

            findInput_TextBox.AcceptsReturn = false;
            replaceInput_TextBox.AcceptsReturn = false;

            this.findButton.Focus();
        }

        /// <summary>
        /// Event on changing multipleLine_CheckBox checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multipleLine_Checked(object sender, RoutedEventArgs e)
        {
            findInput_TextBox.AcceptsReturn = true;
            replaceInput_TextBox.AcceptsReturn = true;

            this.findButton.Focus();
        }

        /// <summary>
        /// Event on FindButton clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            InitiateSearch(findInput_TextBox.Text);
        }

        /// <summary>
        /// Event on ReplaceButton clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (replace_TabItem.IsSelected == true)
            {
                InitiateReplace(findInput_TextBox.Text, replaceInput_TextBox.Text);
            }
        }

        /// <summary>
        /// Event on key down in findInput
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void findInput_KeyDown(object sender, KeyEventArgs e)
        {
            // ENTER
            if (e.Key == Key.Return && multipleLine_CheckBox.IsChecked == false)
            {
                InitiateSearch(findInput_TextBox.Text);

                e.Handled = true;
            }
            findInput_TextBox.Focus();
        }
        //

        /// <summary>
        /// Call ReplaceString method based on selected settings
        /// </summary>
        /// <param name="text"></param>
        private void InitiateReplace(string replaceFrom, string replaceTo)
        {
            if (replaceFrom.Length > 0)
            {
                ((MainWindow)this.Owner).textBoxMain.Focus();
                if (matchCase_CheckBox.IsChecked == true)
                {
                    if (down_RadioButton.IsChecked == true)
                    {
                        ((MainWindow)this.Owner).ReplaceString(replaceFrom, replaceTo, false, true);
                    }
                    else
                    {
                        ((MainWindow)this.Owner).ReplaceString(replaceFrom, replaceTo, false, false);
                    }
                }
                else
                {
                    if (down_RadioButton.IsChecked == true)
                    {
                        ((MainWindow)this.Owner).ReplaceString(replaceFrom, replaceTo, true, true);
                    }
                    else
                    {
                        ((MainWindow)this.Owner).ReplaceString(replaceFrom, replaceTo, true, false);
                    }
                }
            }
            this.replaceButton.Focus();
            matchesCounter.Content = "*matches found: " + ((MainWindow)this.Owner).searchResults.Count;
        }

        /// <summary>
        /// Call FindString method based on selected settings
        /// </summary>
        /// <param name="text"></param>
        private void InitiateSearch(string text)
        {
            if (text.Length > 0)
            {
                ((MainWindow)this.Owner).textBoxMain.Focus();
                if (matchCase_CheckBox.IsChecked == true)
                {
                    if (down_RadioButton.IsChecked == true)
                    {
                        ((MainWindow)this.Owner).FindString(text, false, true);
                    }
                    else
                    {
                        ((MainWindow)this.Owner).FindString(text, false, false);
                    }
                }
                else
                {
                    if (down_RadioButton.IsChecked == true)
                    {
                        ((MainWindow)this.Owner).FindString(text, true, true);
                    }
                    else
                    {
                        ((MainWindow)this.Owner).FindString(text, true, false);
                    }
                }
            }
            this.findButton.Focus();
            matchesCounter.Content = "*matches found: " + ((MainWindow)this.Owner).searchResults.Count;
        }

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
            // ENTER
            if (e.SystemKey == Key.Enter && multipleLine_CheckBox.IsChecked == false)
            {
                InitiateSearch(findInput_TextBox.Text);

                e.Handled = true;
            }

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
            Properties.Settings.Default.DesiredString = this.findInput_TextBox.Text;
            Properties.Settings.Default.ReplaceTo_String = this.replaceInput_TextBox.Text;
            Properties.Settings.Default.MatchCase = this.matchCase_CheckBox.IsChecked ?? false;
            Properties.Settings.Default.MultipleLineInput = this.multipleLine_CheckBox.IsChecked ?? false;
            Properties.Settings.Default.SearchDirectionIsDown = this.down_RadioButton.IsChecked ?? false;

            Hide();
        }
        ////
        //
    }
}