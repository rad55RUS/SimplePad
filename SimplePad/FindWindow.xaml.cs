﻿using System;
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
            WindowChrome.SetIsHitTestVisibleInChrome(this.goTo_TabItem, true);
            //
        }
        //

        // This window events
        /// <summary>
        /// Event on goToInput_Textbox text changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void goToInput_TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Event on goToInput_Textbox text changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void goToInput_TextBox_TextChanged(object sender, EventArgs e)
        {
            goToInput_TextBox.Text = goToInput_TextBox.Text.Replace(" ", "");

            if (this.Visibility == Visibility.Visible && this.IsActive)
            {
                if (goToInput_TextBox.Text != "")
                {
                    if (Int32.Parse(goToInput_TextBox.Text) > ((MainWindow)this.Owner).textBoxMain.LineCount)
                    {
                        goToInput_TextBox.Text = (((MainWindow)this.Owner).textBoxMain.LineCount - 1).ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Event on changing matchCase_CheckBox status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (find_TabItem.IsSelected == true)
            {
                matchesCounter.Visibility = Visibility.Visible;
                matchCase_CheckBox.Visibility = Visibility.Visible;
                multipleLine_CheckBox.Visibility = Visibility.Visible;

                findInput_TextBox.Visibility = Visibility.Visible;
                findInput_Label.Visibility = Visibility.Visible;
                findButton.Visibility = Visibility.Visible;

                replaceInput_TextBox.Visibility = Visibility.Hidden;
                replaceInput_Label.Visibility = Visibility.Hidden;
                replaceButton.Visibility = Visibility.Hidden;

                direction_Label.Visibility = Visibility.Visible;
                direction_Rectangle.Visibility = Visibility.Visible;
                up_RadioButton.Visibility = Visibility.Visible;
                down_RadioButton.Visibility = Visibility.Visible;

                findButton.Content = "Find next";

                this.Height = 170;
                this.Width = 463;
            }
            else if (replace_TabItem.IsSelected == true)
            {
                matchesCounter.Visibility = Visibility.Visible;
                matchCase_CheckBox.Visibility = Visibility.Visible;
                multipleLine_CheckBox.Visibility = Visibility.Visible;

                findInput_TextBox.Visibility = Visibility.Visible;
                findInput_Label.Visibility = Visibility.Visible;
                findButton.Visibility = Visibility.Visible;

                replaceInput_TextBox.Visibility = Visibility.Visible;
                replaceInput_Label.Visibility = Visibility.Visible;
                replaceButton.Visibility = Visibility.Visible;

                direction_Label.Visibility = Visibility.Visible;
                direction_Rectangle.Visibility = Visibility.Visible;
                up_RadioButton.Visibility = Visibility.Visible;
                down_RadioButton.Visibility = Visibility.Visible;

                findButton.Content = "Find next";

                this.Height = 217;
                this.Width = 463;
            }
            else if (findInFiles_TabItem.IsSelected == true)
            {
                matchesCounter.Visibility = Visibility.Visible;
                matchCase_CheckBox.Visibility = Visibility.Visible;
                multipleLine_CheckBox.Visibility = Visibility.Visible;

                findInput_TextBox.Visibility = Visibility.Visible;
                findInput_Label.Visibility = Visibility.Visible;
                findButton.Visibility = Visibility.Visible;

                replaceInput_TextBox.Visibility = Visibility.Visible;
                replaceInput_Label.Visibility = Visibility.Visible;
                replaceButton.Visibility = Visibility.Visible;

                direction_Label.Visibility = Visibility.Hidden;
                direction_Rectangle.Visibility = Visibility.Hidden;
                up_RadioButton.Visibility = Visibility.Hidden;
                down_RadioButton.Visibility = Visibility.Hidden;

                findButton.Content = "Find";

                this.Height = 217;
                this.Width = 463;
            }
            else if (goTo_TabItem.IsSelected == true)
            {
                matchesCounter.Visibility = Visibility.Hidden;
                matchCase_CheckBox.Visibility = Visibility.Hidden;
                multipleLine_CheckBox.Visibility = Visibility.Hidden;

                findInput_TextBox.Visibility = Visibility.Hidden;
                findInput_Label.Visibility = Visibility.Hidden;
                findButton.Visibility = Visibility.Hidden;

                replaceInput_TextBox.Visibility = Visibility.Hidden;
                replaceInput_Label.Visibility = Visibility.Hidden;
                replaceButton.Visibility = Visibility.Hidden;

                direction_Label.Visibility = Visibility.Hidden;
                direction_Rectangle.Visibility = Visibility.Hidden;
                up_RadioButton.Visibility = Visibility.Hidden;
                down_RadioButton.Visibility = Visibility.Hidden;

                findButton.Content = "Find";

                this.Height = 140;
                this.Width = 380;
            }
        }

        /// <summary>
        /// Event on changing matchCase_CheckBox status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (goTo_TabItem.IsSelected != true)
            {
                findInput_TextBox.Focus();
                findInput_TextBox.SelectAll();
            }
            else
            {
                goToInput_TextBox.Focus();
                goToInput_TextBox.SelectAll();
            }
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
        /// Event on goToButton clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoTo_Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).FindLine(int.Parse(goToInput_TextBox.Text));
        }

        /// <summary>
        /// Event on key down in goToInput
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void goToInput_KeyDown(object sender, KeyEventArgs e)
        {
            // ENTER
            if (e.Key == Key.Return && multipleLine_CheckBox.IsChecked == false)
            {
                ((MainWindow)this.Owner).FindLine(int.Parse(goToInput_TextBox.Text));

                e.Handled = true;
            }
        }
        //

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
                findInput_TextBox.Focus();

                e.Handled = true;
            }
        }
        //

        // Initiate search methods
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
        /// <br/>
        /// Open find_TabItem on ctrl + f
        /// <br/>
        /// Open replace_tabItem on ctrl + h
        /// <br/>
        /// Open goTo_tabItem on ctrl + g
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // CTRL + F
            if (e.Key == Key.F && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                find_TabItem.IsSelected = true;

                e.Handled = true;
            }
            // CTRL + H
            if (e.Key == Key.H && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                replace_TabItem.IsSelected = true;

                e.Handled = true;
            }
            // CTRL + G
            if (e.Key == Key.G && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                goTo_TabItem.IsSelected = true;

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