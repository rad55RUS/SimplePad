using static System.Net.Mime.MediaTypeNames;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Microsoft.Win32;
using System.IO.Pipes;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using System.Runtime.InteropServices;

using TextFile_Lib;
using Point = System.Windows.Point;

namespace SimplePad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		
        // Constants
        private const uint WP_SYSTEMMENU = 0x02;
		private const uint WM_SYSTEMMENU = 0xa4;
		//

		// Readonly static fields
		/// Images
		private readonly static BitmapImage MaximizeButton_BitmapImage_White = new(new Uri(@"\Assets\WindowChrome\MaximizeWhite.png", UriKind.Relative));
		private readonly static BitmapImage RestoreButton_BitmapImage_White = new(new Uri(@"\Assets\WindowChrome\RestoreWhite.png", UriKind.Relative));
        /// 
        //

        // Structs
        internal struct SearchResult
		{
			public string desiredString;
            public int startPosition;

			public SearchResult(string _desiredString, int _startPosition)
			{
				desiredString = _desiredString;
                startPosition = _startPosition;
            }
        }
		//

		// Common private fields
		private readonly FindWindow findWindow = new();
        private Encoding encoding = Encoding.UTF8;
		//

		// Common protected fields
		private TextFile textFile;
        internal List<SearchResult> searchResults = new();
        //

        // Initialization
        public MainWindow()
		{
            InitializeComponent();
			this.StateChanged += ResizeWindow;
		}

		protected void OnLoad(object sender, RoutedEventArgs e)
		{
			// Load properties
            this.Top = Properties.Settings.Default.WindowTop;
			this.Left = Properties.Settings.Default.WindowLeft;
			this.Height = Properties.Settings.Default.WindowHeight;
			this.Width = Properties.Settings.Default.WindowWidth;

            textBoxMain.FontSize = Properties.Settings.Default.FontSize;

            if (Properties.Settings.Default.Maximized)
			{
				WindowState = WindowState.Maximized;
			}

			if (Properties.Settings.Default.WordWrap)
			{
				textBoxMain.TextWrapping = TextWrapping.Wrap;
				FormatButton_WordWrap.IsChecked = true;
			}
			else
			{
                textBoxMain.TextWrapping = TextWrapping.NoWrap;
                FormatButton_WordWrap.IsChecked = false;
            }
			//

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			// WindowChrome
			IntPtr windIntPtr = new WindowInteropHelper(this).Handle;
			HwndSource hwndSource = HwndSource.FromHwnd(windIntPtr);
			hwndSource.AddHook(new HwndSourceHook(WndProc));

			WindowChrome.SetIsHitTestVisibleInChrome(this.CloseButton, true);
			WindowChrome.SetIsHitTestVisibleInChrome(this.MaximizeButton, true);
			WindowChrome.SetIsHitTestVisibleInChrome(this.MinimizeButton, true);
			WindowChrome.SetIsHitTestVisibleInChrome(this.IconButton, true);
			WindowChrome.SetIsHitTestVisibleInChrome(this.FileButton, true);
			//

			// Initial title bar extra buttons settings
			/// FileButton context menu
			FileButton.ContextMenu.PlacementTarget = FileButton;
			FileButton_Save.Style = (Style)Resources["UnclickableMenuItem"];
            FileButton_RenameOrMove.Style = (Style)Resources["UnclickableMenuItem"];
            FileButton_MoveToRecycleBin.Style = (Style)Resources["UnclickableMenuItem"];
            ///
            /// EditButton context menu
            EditButton.ContextMenu.PlacementTarget = EditButton;
			EditButton_Undo.Style = (Style)Resources["UnclickableMenuItem"];
			EditButton_Redo.Style = (Style)Resources["UnclickableMenuItem"];
			EditButton_Cut.Style = (Style)Resources["UnclickableMenuItem"];
			EditButton_Copy.Style = (Style)Resources["UnclickableMenuItem"];
			EditButton_Delete.Style = (Style)Resources["UnclickableMenuItem"];
            /// SearchButton context menu
            SearchButton.ContextMenu.PlacementTarget = SearchButton;
            ///
            /// FormatButton context menu
            FormatButton.ContextMenu.PlacementTarget = FormatButton;
            ///
			//

            // Initial title bar context menu settings
            TitleBar_Restore.Style = (Style)Resources["UnclickableMenuItem"];
			//

			this.textBoxMain.IsReadOnly = false;

            // Insert text from startup file
            textBoxMain.TextChanged -= textBoxMain_TextChanged;
            textFile = App.textFile;
            if (textFile.Path != "")
                textBoxMain.Text = textFile.ReadFromFile(textBoxMain.Text, encoding);
            textFile.OnFileOperation += OnFileOperation;
            textBoxMain.TextChanged += textBoxMain_TextChanged;
            //
        }
		//

		// Closing
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			if (WindowState == WindowState.Maximized)
			{
				// Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
				Properties.Settings.Default.WindowTop = RestoreBounds.Top;
				Properties.Settings.Default.WindowLeft = RestoreBounds.Left;
				Properties.Settings.Default.WindowHeight = RestoreBounds.Height;
				Properties.Settings.Default.WindowWidth = RestoreBounds.Width;
				Properties.Settings.Default.Maximized = true;
			}
			else
			{
				Properties.Settings.Default.WindowTop = this.Top;
				Properties.Settings.Default.WindowLeft = this.Left;
				Properties.Settings.Default.WindowHeight = this.Height;
				Properties.Settings.Default.WindowWidth = this.Width;
				Properties.Settings.Default.Maximized = false;
			}

			if (textBoxMain.TextWrapping == TextWrapping.Wrap)
			{
				Properties.Settings.Default.WordWrap = true;
			}
			else
			{
                Properties.Settings.Default.WordWrap = false;
            }

            Properties.Settings.Default.FontSize = (int)textBoxMain.FontSize;

            Properties.Settings.Default.Save();
			Process.GetCurrentProcess().Kill();
        }
		//

		// Title bar extra buttons methods
		/// EditButton methods
		private void EditButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				EditButton.ContextMenu.IsOpen = true;
			}
		}

		private void Undo_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.Undo();
		}

		private void Redo_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.Redo();
		}

		private void Cut_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.Cut();
		}

		private void Copy_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.Copy();
		}

		private void Paste_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.Paste();
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.SelectedText = "";
		}

		private void SelectAll_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.SelectAll();
		}
		///
		/// FileButton methods
		private void FileButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				FileButton.ContextMenu.IsOpen = true;
			}
		}

		private void Open_Click(object sender, RoutedEventArgs e)
		{
			textBoxMain.TextChanged -= textBoxMain_TextChanged;

            textBoxMain.Text = textFile.OpenFile(textBoxMain.Text, ref encoding);

            textBoxMain.TextChanged += textBoxMain_TextChanged;
        }

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			textFile.SaveFile(textBoxMain.Text);
		}

		private void SaveAs_Click(object sender, RoutedEventArgs e)
		{
			textFile.SaveFile(textBoxMain.Text, true);
		}

		private void RenameOrMove_Click(object sender, RoutedEventArgs e)
		{
			textFile.RenameOrMoveFile();
		}

        private void MoveToRecycleBin_Click(object sender, RoutedEventArgs e)
        {
            textFile.DeleteFile();

            // Reset title bar menu item style
            /// FileButton context menu
            FileButton_Save.Style = (Style)Resources["UnclickableMenuItem"];
            FileButton_RenameOrMove.Style = (Style)Resources["UnclickableMenuItem"];
            FileButton_MoveToRecycleBin.Style = (Style)Resources["UnclickableMenuItem"];
            ///
            /// EditButton context menu
            EditButton_Undo.Style = (Style)Resources["UnclickableMenuItem"];
            EditButton_Redo.Style = (Style)Resources["UnclickableMenuItem"];
            EditButton_Cut.Style = (Style)Resources["UnclickableMenuItem"];
            EditButton_Copy.Style = (Style)Resources["UnclickableMenuItem"];
            EditButton_Delete.Style = (Style)Resources["UnclickableMenuItem"];
            /// SearchButton context menu
            ///
            /// FormatButton context menu
            ///
            //

            WindowName.Content = "SimplePad";
            textBoxMain.Text = "";
        }
        ///
        /// SearchButton methods
		private void SearchButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SearchButton.ContextMenu.IsOpen = true;
            }
        }
        private void Find_Click(object sender, RoutedEventArgs e)
        {
			OpenSearchWindow(0);
        }
        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            OpenSearchWindow(1);
        }
        ///
        /// FormatButton methods
        private void FormatButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FormatButton.ContextMenu.IsOpen = true;
            }
        }
        private void WordWrap_Click(object sender, RoutedEventArgs e)
        {
            if (FormatButton_WordWrap.IsChecked)
			{
				this.textBoxMain.TextWrapping = TextWrapping.Wrap;
			}
			else
                this.textBoxMain.TextWrapping = TextWrapping.NoWrap;
        }
		///
		//

		// Find window methods
		/// Open find window
		private void OpenSearchWindow(int tabItemNum)
		{
			var placement = SearchButton.PointToScreen(new Point(0, 0));

			if (this.Left >= System.Windows.SystemParameters.WorkArea.Width / 1.5)
			{
				findWindow.Left = System.Windows.SystemParameters.WorkArea.Width - findWindow.Width;
			}
			else if (this.Left <= 0)
			{
				findWindow.Left = 0;
			}
			else
			{
				findWindow.Left = this.Left + this.Width - findWindow.Width;
			}
			if (this.Top >= System.Windows.SystemParameters.WorkArea.Height / 1.5)
			{
				findWindow.Top = System.Windows.SystemParameters.WorkArea.Height - findWindow.Height;
			}
			else if (this.Top <= 0)
			{
				findWindow.Top = 0;
			}
			else
			{
				findWindow.Top = placement.Y;
			}

			findWindow.Owner = this;
            findWindow.Show();

            switch (tabItemNum)
			{
				case 0:
					findWindow.find_TabItem.IsSelected = true;
                    break;
				case 1:
					findWindow.replace_TabItem.IsSelected = true;
                    break;
                case 2:
                    findWindow.findInFiles_TabItem.IsSelected = true;
					break;
				default:
					break;
            }
            findWindow.findInput_TextBox.Focus();
            findWindow.findInput_TextBox.SelectAll();
        }

		/// <summary>
		/// Create result list from matches to desiredString in a text
		/// </summary>
		/// <param name="desiredString"></param>
		/// <param name="text"></param>
		private void CreateFindResultList(string desiredString, string text)
		{
            for (int i = 0; i < text.Length - desiredString.Length + 1; i++)
            {
                string checkingString = "";

                for (int j = 0; j < desiredString.Length; j++)
                {
                    checkingString += text[i + j];
                }
                if (checkingString == desiredString)
                {
                    searchResults.Add(new SearchResult(desiredString, i));
                    i += desiredString.Length - 1;
                }
            }
        }

        /// <summary>
		/// Find string in the textBoxMain
		/// </summary>
		/// <param name="desiredString"></param>
		/// <param name="anyCase"></param>
		/// <param name="DirectionIsDown"></param>
        internal void FindString(string desiredString, bool anyCase, bool DirectionIsDown)
        {
			if (anyCase == true)
			{
				string lowerCaseText = textBoxMain.Text.ToLower();
				desiredString = desiredString.ToLower();
				if (searchResults.Count == 0)
				{
					CreateFindResultList(desiredString, lowerCaseText);
				}
				else if (searchResults[0].desiredString != desiredString)
				{
					searchResults.Clear();
					CreateFindResultList(desiredString, lowerCaseText);
				}
			}
			else
			{
				if (searchResults.Count == 0)
				{
					CreateFindResultList(desiredString, textBoxMain.Text);
				}
				else if (searchResults[0].desiredString != desiredString)
				{
					searchResults.Clear();
					CreateFindResultList(desiredString, textBoxMain.Text);
				}
			}
			if (searchResults.Count > 0)
			{
				if (DirectionIsDown == true)
				{
					for (int i = 0; i < searchResults.Count; i++)
					{
                        if (searchResults[i].startPosition > textBoxMain.SelectionStart)
                        {
                            textBoxMain.SelectionStart = searchResults[i].startPosition;
                            textBoxMain.SelectionLength = desiredString.Length;
                            break;
                        }
                        else if (i == searchResults.Count - 1)
                        {
                            textBoxMain.SelectionStart = searchResults[0].startPosition;
                            textBoxMain.SelectionLength = desiredString.Length;
                            break;
                        }
                    }
				}
				else
                {
                    for (int i = searchResults.Count - 1; i >= 0; i--)
                    {
                        if (searchResults[i].startPosition < textBoxMain.SelectionStart)
                        {
                            textBoxMain.SelectionStart = searchResults[i].startPosition;
                            textBoxMain.SelectionLength = desiredString.Length;
                            break;
                        }
                        else if (i == 0)
                        {
                            textBoxMain.SelectionStart = searchResults[^1].startPosition;
                            textBoxMain.SelectionLength = desiredString.Length;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find string in the textBoxMain
        /// </summary>
        /// <param name="desiredString"></param>
        /// <param name="anyCase"></param>
        /// <param name="DirectionIsDown"></param>
        internal void ReplaceString(string replaceFrom, string replaceTo, bool anyCase, bool DirectionIsDown)
        {
			bool textReplaced = false;

            if (textBoxMain.SelectedText == replaceFrom)
            {
                textBoxMain.SelectedText = textBoxMain.SelectedText.Replace(textBoxMain.SelectedText, replaceTo);
                textReplaced = true;
            }

			FindString(replaceFrom, anyCase, DirectionIsDown);

            if (searchResults.Count > 0)
            {
                if (textReplaced == false)
                {
                    textBoxMain.SelectedText = textBoxMain.SelectedText.Replace(textBoxMain.SelectedText, replaceTo);
                }
            }
        }
        // 

        // TextBox events
        /// <summary>
        /// Event on text selection
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private void textBoxMain_SelectionChanged(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(textBoxMain.SelectedText))
			{
				EditButton_Cut.Style = (Style)Resources["ClickableMenuItemBlack"];
				EditButton_Copy.Style = (Style)Resources["ClickableMenuItemBlack"];
				EditButton_Delete.Style = (Style)Resources["ClickableMenuItemBlack"];
			}
			else
			{
				EditButton_Cut.Style = (Style)Resources["UnclickableMenuItem"];
				EditButton_Delete.Style = (Style)Resources["UnclickableMenuItem"];
			}
		}

        /// <summary>
        /// Event on text changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void textBoxMain_TextChanged(object sender, EventArgs e)
		{
			if (textBoxMain.CanRedo == true)
				EditButton_Redo.Style = (Style)Resources["ClickableMenuItemBlack"];
			else
				EditButton_Redo.Style = (Style)Resources["UnclickableMenuItem"];

			if (textBoxMain.CanUndo == true)
				EditButton_Undo.Style = (Style)Resources["ClickableMenuItemBlack"];
			else
				EditButton_Undo.Style = (Style)Resources["UnclickableMenuItem"];

			if (!String.IsNullOrEmpty(textFile.Path))
			{
				WindowName.Content = "*" + textFile.Path + " - SimplePad";
			}
			else
			{
				WindowName.Content = "*" + "SimplePad";
			}

            searchResults.Clear();
            textFile.isSaved = false;
		}

        /// <summary>
        /// Event on mouse wheel using
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void textBoxMain_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			// CTRL + MouseWheel
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
				/// Scale up
				if (e.Delta > 0)
				{
					textBoxMain.FontSize += 1;
				}
				/// Scale down
				if (e.Delta < 0 && textBoxMain.FontSize > 1)
				{
					textBoxMain.FontSize -= 1;
				}
			}
			else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
			{
				textBoxMain.ScrollToHorizontalOffset(textBoxMain.HorizontalOffset - e.Delta);
				e.Handled = true;
			}
			else
			{
				textBoxMain.ScrollToVerticalOffset(textBoxMain.VerticalOffset - e.Delta);
				e.Handled = true;
			}
		}
        //

        // Window methods
		/// <summary>
		/// Change title after save, opening and other operations with text file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void OnFileOperation(object sender, EventArgs e)
        {
            FileButton_Save.Style = (Style)Resources["ClickableMenuItemBlack"];
            FileButton_RenameOrMove.Style = (Style)Resources["ClickableMenuItemBlack"];
            FileButton_MoveToRecycleBin.Style = (Style)Resources["ClickableMenuItemBlack"];

            WindowName.Content = textFile.Path + " - SimplePad";
        }

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
        /// Prevent bugged default title bar context menu from appearing on alt+Space;
        /// <br/>//<br/>
        /// Save text file on ctrl+s;
        /// <br/>//<br/>
        /// Open text file on ctrl+o
        /// <br/>//<br/>
        /// Open find tab item in search window on ctrl+f
        /// </summary>
		/// Open replace tab item in search window on ctrl+f
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
            // CTRL + F
            if (e.Key == Key.F && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                OpenSearchWindow(0);

                e.Handled = true;
            }
            // CTRL + H
            if (e.Key == Key.H && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                OpenSearchWindow(1);

                e.Handled = true;
            }
            // CTRL + O
            if (e.Key == Key.O && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
                textBoxMain.TextChanged -= textBoxMain_TextChanged;

                textBoxMain.Text = textFile.OpenFile(textBoxMain.Text, ref encoding);

                textBoxMain.TextChanged += textBoxMain_TextChanged;

                e.Handled = true;
			}
			// CTRL + S
			if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
                // Interface conversion testing
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    ITextFilePublicMethods conversionTest = textFile;
                    Debug.Print("Попытка сохранения файла с помощью преобразования к интерфейсу");
                    conversionTest.SaveFile(textBoxMain.Text, false);
                    Debug.Print("(выполняется явное преобразование интерфейса к классу) Путь файла равен: " + ((TextFile)conversionTest).Path);
                    Debug.Print("");
                }
				//
                // SHIFT
                else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				{
					textFile.SaveFile(textBoxMain.Text, true);
				}
				// NO SHIFT
				else
				{
					textFile.SaveFile(textBoxMain.Text);
				}

				e.Handled = true;
			}
		}

		/// <summary>
		/// Open title bar context menu via icon in top left
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IconButton_Click(object sender, RoutedEventArgs e)
		{
			TitleBar_ContextMenu.IsOpen = true;
		}
		////

		//// Window Operations
		/// <summary>
		/// Close window event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloseWindow(object sender, RoutedEventArgs e)
		{
			if (textFile.isSaved == false)
			{
				CloseSaveWindow closeSaveWindow = new()
				{
					Left = this.Left + this.Width / 2,
					Top = this.Top + this.Height / 2
				};

				if (textFile.Path == "")
					closeSaveWindow.MainText.Content = "Do you want to save the file?";
				else
					closeSaveWindow.MainText.Content = "Do you want to save changes to a file \n\"" + textFile.Path + "\"?";
				closeSaveWindow.ShowDialog();

                switch (closeSaveWindow.result)
                {
                    case "Save":
                        textFile.SaveFile(textBoxMain.Text);
                        Close();
                        break;
                    case "Cancel":
                        break;
                    default:
                        Close();
                        break;
                }
            }
			else Close();
		}

		/// <summary>
		/// Maximize window event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MaximizeWindow(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Maximized;
		}

		/// <summary>
		/// Restore window event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RestoreWindow(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Normal;
		}

		/// <summary>
		/// Minimize window event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MinimizeWindow(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Window state changing event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResizeWindow(object? sender, EventArgs e)
		{
			var TitleBarContextMenu = Resources["titleBar_ContextMenu"] as ContextMenu;

			switch (this.WindowState)
			{
				case WindowState.Normal:

					// Title bar context menu
					TitleBar_Restore.Style = (Style)Resources["UnclickableMenuItem"];
					TitleBar_Maximize.Style = (Style)Resources["ClickableMenuItemBlack"];
					//

					// Title bar buttons
					this.MaximizeButton_Icon.Source = MaximizeButton_BitmapImage_White;
					this.MaximizeButton.Click -= RestoreWindow;
					this.MaximizeButton.Click += MaximizeWindow;
					//
					break;

				case WindowState.Maximized:

					// Title bar context menu
					TitleBar_Restore.Style = (Style)Resources["ClickableMenuItemBlack"];
					TitleBar_Maximize.Style = (Style)Resources["UnclickableMenuItem"];
					//

					// Title bar buttons
					this.MaximizeButton_Icon.Source = RestoreButton_BitmapImage_White;
					this.MaximizeButton.Click -= MaximizeWindow;
					this.MaximizeButton.Click += RestoreWindow;
					//
					break;

			}
		}
		////
		//
	}
}