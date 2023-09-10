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
using System.Threading;

using TextFile_Lib;
using Point = System.Windows.Point;
using Gma.System.MouseKeyHook;
using System.Collections;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Threading;
using System.Windows.Media.TextFormatting;
using System.Text.RegularExpressions;
using System.IO;

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

        internal class SearchResult
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
        private bool searchResults_isResizing = false;
		private readonly FindWindow findWindow = new();
        private IKeyboardMouseEvents m_GlobalHook;
        private Paragraph paragraph;
        private Point mouseOffset;
        private FindInFilesWindow findInFilesWindow;
        //

        // Common internal fields
        internal List<SearchResult> searchResults = new();
        internal SearchInFilesArgs searchInFilesArgs;
        internal TextFile textFile;
        internal Thread thread1;
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

            searchResults_RichTextBox.FontSize = Properties.Settings.Default.FontSize_SearchResults;
            searchResults_Grid.Height = Properties.Settings.Default.SearchResultsHeight;
            searchResults_Grid.Visibility = Visibility.Collapsed;
            searchResults_Grid.Height = 0;


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

            // GlobalHook initializing
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseUpExt += GlobalHookMouseUpExt;
            //

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
            textFile.OnFileOperation += OnFileOperation;
            if (textFile.Path != "")
			{
				textBoxMain.Text = TextFile.ReadFromFile(textFile.Path);
                findWindow.lineCounter.Content = "line amount: " + textBoxMain.LineCount.ToString();

                OnFileOperation(textFile, new EventArgs());
            }
            textBoxMain.TextChanged += textBoxMain_TextChanged;
            //

            this.textBoxMain.Focus();
        }
		//

		// Closing
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
            m_GlobalHook.MouseUpExt -= GlobalHookMouseUpExt;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();

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
            Properties.Settings.Default.FontSize_SearchResults = (int)searchResults_RichTextBox.FontSize;
            if (searchResults_Grid.Height != 0)
            {
                Properties.Settings.Default.SearchResultsHeight = searchResults_Grid.Height;
            }

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

            textBoxMain.Text = textFile.OpenFile(textBoxMain.Text);

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
            this.Title = "SimplePad";
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

        private void FindInFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenSearchWindow(2);
        }

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
			FindNext();
        }

        private void FindPrevious_Click(object sender, RoutedEventArgs e)
        {
            FindPrevious();
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            OpenSearchWindow(1);
        }

        private void GoTo_Click(object sender, RoutedEventArgs e)
        {
            OpenSearchWindow(3);
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
			{
				this.textBoxMain.TextWrapping = TextWrapping.NoWrap;
			}

            int currentLine = textBoxMain.GetLineIndexFromCharacterIndex(textBoxMain.SelectionStart);

            findWindow.currentLine_Label.Content = "current line is " + currentLine.ToString();
            findWindow.goToInput_TextBox.Text = currentLine.ToString();
        }
        ///
        //

        // Find window methods
        /// <summary>
        /// Find next
        /// </summary>
        private void FindNext()
		{
            if (!findWindow.IsVisible)
            {
                OpenSearchWindow(0);
            }

            if (findWindow.findInput_TextBox.Text != "")
            {
                FindString(findWindow.findInput_TextBox.Text, (bool)!findWindow.matchCase_CheckBox.IsChecked, true);
                findWindow.findButton.Focus();
                findWindow.matchesCounter.Content = "*matches found: " + searchResults.Count;
            }

			textBoxMain.Focus();
        }

        /// <summary>
        /// Find previous
        /// </summary>
        private void FindPrevious()
        {
            if (!findWindow.IsVisible)
            {
                OpenSearchWindow(0);
            }

            if (findWindow.findInput_TextBox.Text != "")
            {
                FindString(findWindow.findInput_TextBox.Text, (bool)!findWindow.matchCase_CheckBox.IsChecked, false);
                findWindow.findButton.Focus();
                findWindow.matchesCounter.Content = "*matches found: " + searchResults.Count;
            }

            textBoxMain.Focus();
        }

        /// <summary>
        /// Open search window
        /// </summary>
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
                    findWindow.findInput_TextBox.Focus();
                    findWindow.findInput_TextBox.SelectAll();
                    break;
				case 1:
					findWindow.replace_TabItem.IsSelected = true;
                    findWindow.findInput_TextBox.Focus();
                    findWindow.findInput_TextBox.SelectAll();
                    break;
                case 2:
                    findWindow.findInFiles_TabItem.IsSelected = true;
                    findWindow.findInput_TextBox.Focus();
                    findWindow.findInput_TextBox.SelectAll();
                    break;
                case 3:
                    findWindow.goTo_TabItem.IsSelected = true;
                    findWindow.goToInput_TextBox.Focus();
                    findWindow.goToInput_TextBox.SelectAll();
                    break;
                default:
					break;
            }
        }

		/// <summary>
		/// Create result list from matches to desiredString in a text
		/// </summary>
		/// <param name="desiredString"></param>
		/// <param name="text"></param>
		private void CreateFindResultList(string desiredString, string text, bool anyCase)
		{
            if (anyCase == true)
            {
                text = text.ToLower();
                desiredString = desiredString.ToLower();
            }
            for (int i = 0; i < text.Length - desiredString.Length + 1; i++)
            {
                string checkingString = "";

                for (int j = 0; j < desiredString.Length; j++)
                {
                    checkingString += text[i + j];
                    if (checkingString[j] != desiredString[j])
                        break;
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
            if (searchResults.Count == 0)
            {
                CreateFindResultList(desiredString, textBoxMain.Text, anyCase);
            }
            else if (searchResults[0].desiredString != desiredString)
            {
                searchResults.Clear();
                CreateFindResultList(desiredString, textBoxMain.Text, anyCase);
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
        /// Replace string in the textBoxMain
        /// </summary>
        /// <param name="desiredString"></param>
        /// <param name="anyCase"></param>
        /// <param name="DirectionIsDown"></param>
        internal void ReplaceString(string replaceFrom, string replaceTo, bool anyCase, bool DirectionIsDown)
        {
            searchResults.Clear();

            bool textReplaced = false;

            if (textBoxMain.SelectedText == replaceFrom)
            {
                textBoxMain.SelectedText = textBoxMain.SelectedText.Replace(textBoxMain.SelectedText, replaceTo);
                textReplaced = true;
            }

			FindString(replaceFrom, anyCase, DirectionIsDown);

            textBoxMain.TextChanged -= textBoxMain_TextChanged;
            if (searchResults.Count > 0)
            {
                if (textReplaced == false)
                {
                    textBoxMain.SelectedText = textBoxMain.SelectedText.Replace(textBoxMain.SelectedText, replaceTo);
                }
            }
            textBoxMain.TextChanged += textBoxMain_TextChanged;
        }

        /// <summary>
        /// Replace all strings in the textBoxMain
        /// </summary>
        /// <param name="desiredString"></param>
        /// <param name="anyCase"></param>
        /// <param name="DirectionIsDown"></param>
        internal void ReplaceAllStrings(string replaceFrom, string replaceTo, bool anyCase)
        {
            int deltaLength = replaceTo.Length - replaceFrom.Length;
            int deltaLengthStor = deltaLength;

            searchResults.Clear();

            if (searchResults.Count == 0)
            {
                CreateFindResultList(replaceFrom, textBoxMain.Text, anyCase);
            }
            else if (searchResults[0].desiredString != replaceFrom)
            {
                searchResults.Clear();
                CreateFindResultList(replaceFrom, textBoxMain.Text, anyCase);
            }

            textBoxMain.TextChanged -= textBoxMain_TextChanged;
            if (searchResults.Count > 0)
            {
                for (int i = 0; i < searchResults.Count; i++)
                {
                    textBoxMain.SelectionStart = searchResults[i].startPosition;
                    textBoxMain.SelectionLength = replaceFrom.Length;
                    textBoxMain.SelectedText = textBoxMain.SelectedText.Replace(textBoxMain.SelectedText, replaceTo);
                    if (i != searchResults.Count - 1)
                    {
                        if (deltaLength != 0)
                        {
                            searchResults[i + 1].startPosition += deltaLengthStor;
                            deltaLengthStor += deltaLength;
                        }
                    }
                }
            }
            textBoxMain.TextChanged += textBoxMain_TextChanged;
        }

        /// <summary>
        /// Find line in the textBoxMain
        /// </summary>
        internal void FindLine(int line)
        {
            textBoxMain.Focus();

            textBoxMain.SelectionStart = textBoxMain.GetCharacterIndexFromLineIndex(line - 1);
            textBoxMain.SelectionLength = 0;
        }

        /// <summary>
		/// Find string in files in specified directory
		/// </summary>
		/// <param name="desiredString"></param>
		/// <param name="anyCase"></param>
		/// <param name="DirectionIsDown"></param>
        internal void FindStringInFiles(object? obj)
        {
            bool filesFound = false;
            int fileCount = 0;
            int matches = 0;
            string text = "";
            string[] extensionsArray = { "txt", "json", "lua", "cfg", "xml", "xaml", "yml", "ini", "html", "css", "cs", "config", "readme", "toml", "properties", "log" };
            List<List<string>> fileList = new List<List<string>>();
            foreach (string extension in extensionsArray)
            {
                fileList.Add(new List<string>());
            }

            string desiredStringTemp = searchInFilesArgs.desiredString;

            if (searchInFilesArgs.anyCase == true)
            {
                desiredStringTemp = searchInFilesArgs.desiredString.ToLower();
            }

            List<string> directoryList = System.IO.Directory.GetDirectories(searchInFilesArgs.directory + "\\").ToList();

            // File array building
            for (int i = 0; i < extensionsArray.Length; i++)
            {
                System.IO.Directory.GetFiles(searchInFilesArgs.directory + "\\", "*." + extensionsArray[i], System.IO.SearchOption.TopDirectoryOnly).ToList().ForEach(file => fileList[i].Add(file));
                if (searchInFilesArgs.subfolders)
                {
                    if (directoryList != null)
                    {
                        foreach (string directory in directoryList)
                        {
                            try
                            {
                                System.IO.Directory.GetFiles(directory, "*." + extensionsArray[i]).ToList().ForEach(file => fileList[i].Add(file));
                            }
                            catch
                            {
                                Debug.Print("FindStringInFiles - Error while trying to get access in " + directory + "\n");
                            }
                        }
                    }
                }
            }

            // Main cycle
            for (int l = 0; l < fileList.Count; l++)
            {
                if (fileList[l] != null)
                {
                    // Preparations
                    if (filesFound == false)
                    {
                        filesFound = true;

                        for (int i = 0; i < fileList.Count; i++)
                        {
                            for (int j = 0; j < fileList[i].Count; j++)
                            {
                                fileCount++;
                            }
                        }

                        // Building findInFilesWindow
                        App.Current.Dispatcher.Invoke(delegate
                        {
                            findInFilesWindow = new FindInFilesWindow();
                            findInFilesWindow.Owner = this;
                            findInFilesWindow.progressBar.Maximum = fileCount;

                            if (findWindow.Left >= System.Windows.SystemParameters.WorkArea.Width / 1.5)
                            {
                                findInFilesWindow.Left = System.Windows.SystemParameters.WorkArea.Width - findInFilesWindow.Width;
                            }
                            else if (findWindow.Left <= 0)
                            {
                                findInFilesWindow.Left = 0;
                            }
                            else
                            {
                                findInFilesWindow.Left = findWindow.Left;
                            }
                            if (findWindow.Top >= System.Windows.SystemParameters.WorkArea.Height / 1.5)
                            {
                                findInFilesWindow.Top = System.Windows.SystemParameters.WorkArea.Height - findInFilesWindow.Height;
                            }
                            else if (findWindow.Top <= 0)
                            {
                                findInFilesWindow.Top = 0;
                            }
                            else
                            {
                                findInFilesWindow.Top = findWindow.Top + 100;
                            }
                            findInFilesWindow.Show();

                            searchResults_RichTextBox.Document.Blocks.Clear();
                            searchResults_Grid.Visibility = Visibility.Visible;

                            searchResults_Grid.Height = Properties.Settings.Default.SearchResultsHeight;
                        });
                    }

                    bool foundInFile = false;

                    // Search cycle
                    for (int i = 0; i < fileList[l].Count; i++)
                    {
                        // Progress bar updating
                        App.Current.Dispatcher.Invoke(delegate
                        {
                            if (fileList[l][i].Count() < 71)
                            {
                                findInFilesWindow.currentFile_Label.Content = fileList[l][i];
                            }
                            else
                            {
                                string currentFile_LabelContent = "";

                                for (int j = 0; j < 35; j++)
                                {
                                    currentFile_LabelContent += fileList[l][i][j];
                                }

                                if (!currentFile_LabelContent.EndsWith("\\"))
                                {
                                    currentFile_LabelContent += "..";
                                    currentFile_LabelContent += "\\";
                                    currentFile_LabelContent += "..";
                                }
                                else
                                {
                                    currentFile_LabelContent += "...";
                                }

                                for (int j = fileList[l][i].Count() - 35; j < fileList[l][i].Count(); j++)
                                {
                                    currentFile_LabelContent += fileList[l][i][j];
                                }

                                findInFilesWindow.currentFile_Label.Content = currentFile_LabelContent;
                            }
                            findInFilesWindow.progressBar.Value++;
                        });
                        //

                        // Line preparations
                        if (l != 3)
                        {
                            text = TextFile.ReadFromFile(fileList[l][i]);
                        }
                        else
                        {
                            text = TextFile.ReadFromFile(fileList[l][i], Encoding.UTF8);
                        }
                        string[] lines = text.Replace("\r", "").Split('\n');
                        if (searchInFilesArgs.replaceString != null)
                        {
                            if (searchInFilesArgs.anyCase == true)
                            {
                                text = Regex.Replace(text, desiredStringTemp, searchInFilesArgs.replaceString, RegexOptions.IgnoreCase);
                            }
                            else
                            {
                                text = Regex.Replace(text, desiredStringTemp, searchInFilesArgs.replaceString);
                            }
                            TextFile.WriteToFile(text, fileList[l][i]);
                        }
                        //
                        
                        // Search-in-file process
                        for (int j = 0; j < lines.Length; j++)
                        {
                            bool foundInLine = false;
                            string line = lines[j];
                            string noDesiredPart = "";

                            for (int t = 0; t < line.Length - desiredStringTemp.Length + 1; t++)
                            {
                                string checkingString = "";
                                string checkingStringTemp = "";

                                for (int m = 0; m < desiredStringTemp.Length; m++)
                                {
                                    checkingString += line[t + m];
                                    checkingStringTemp = checkingString;
                                    if (searchInFilesArgs.anyCase)
                                    {
                                        checkingStringTemp = checkingStringTemp.ToLower();
                                    }
                                    if (checkingStringTemp[m] != desiredStringTemp[m])
                                        break;
                                }
                                if (checkingStringTemp == desiredStringTemp)
                                {
                                    matches++;
                                    t += desiredStringTemp.Length - 1;

                                    if (!foundInFile)
                                    {
                                        App.Current.Dispatcher.Invoke(delegate
                                        {
                                            paragraph = new Paragraph();
                                            paragraph.Inlines.Add(new Run(fileList[l][i]) { Foreground = System.Windows.Media.Brushes.PaleGoldenrod });
                                        });
                                    }
                                    if (!foundInLine)
                                    {
                                        App.Current.Dispatcher.Invoke(delegate
                                        {
                                            paragraph.Inlines.Add(new Bold(new Run("\n" + "line ")));
                                            paragraph.Inlines.Add(new Bold(new Run("" + (j + 1))) { Foreground = System.Windows.Media.Brushes.LightSkyBlue });
                                            paragraph.Inlines.Add(new Bold(new Run(": ")));
                                        });
                                    }
                                    App.Current.Dispatcher.Invoke(delegate
                                    {
                                        paragraph.Inlines.Add(new Run(noDesiredPart));
                                        paragraph.Inlines.Add(new Run(checkingString) { Foreground = System.Windows.Media.Brushes.Gold });
                                    });

                                    foundInFile = true;
                                    foundInLine = true;

                                    noDesiredPart = "";
                                }
                                else
                                {
                                    noDesiredPart += line[t];
                                    if (t == line.Length - desiredStringTemp.Length && foundInLine == true)
                                    {
                                        for (int m = t + 1; m < line.Length; m++)
                                        {
                                            noDesiredPart += line[m];
                                        }
                                    }
                                }
                            }
                            if (foundInLine)
                            {
                                App.Current.Dispatcher.Invoke(delegate
                                {
                                    paragraph.Inlines.Add(new Run(noDesiredPart));
                                });

                                if (searchInFilesArgs.replaceString != null)
                                {
                                    App.Current.Dispatcher.Invoke(delegate
                                    {
                                        paragraph.Inlines.Add(new Run(" => "));
                                    });
                                    for (int t = 0; t < line.Length - desiredStringTemp.Length + 1; t++)
                                    {
                                        string checkingString = "";
                                        string checkingStringTemp = "";

                                        for (int m = 0; m < desiredStringTemp.Length; m++)
                                        {
                                            checkingString += line[t + m];
                                            checkingStringTemp = checkingString;
                                            if (searchInFilesArgs.anyCase)
                                            {
                                                checkingStringTemp = checkingStringTemp.ToLower();
                                            }
                                            if (checkingStringTemp[m] != desiredStringTemp[m])
                                                break;
                                        }
                                        if (checkingStringTemp == desiredStringTemp)
                                        {
                                            t += desiredStringTemp.Length - 1;

                                            App.Current.Dispatcher.Invoke(delegate
                                            {
                                                paragraph.Inlines.Add(new Run(noDesiredPart));
                                                paragraph.Inlines.Add(new Run(searchInFilesArgs.replaceString) { Foreground = System.Windows.Media.Brushes.LightGreen });
                                            });

                                            noDesiredPart = "";
                                        }
                                        else
                                        {
                                            noDesiredPart += line[t];
                                            if (t == line.Length - desiredStringTemp.Length)
                                            {
                                                for (int m = t + 1; m < line.Length; m++)
                                                {
                                                    noDesiredPart += line[m];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //

                        if (foundInFile)
                        {
                            App.Current.Dispatcher.Invoke(delegate
                            {
                                searchResults_RichTextBox.Document.Blocks.Add(paragraph);
                            });
                            foundInFile = false;
                        }
                    }
                    //
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        searchResults_Label.Content = "Search results (" + matches + " matches)";
                    });
                }
            }
            App.Current.Dispatcher.Invoke(delegate
            {
                if (findInFilesWindow != null)
                {
                    if (findInFilesWindow.IsEnabled)
                    {
                        findInFilesWindow.Close();
                    }
                }
            });
        }
        // 

        // Search results events
        /// <summary>
        /// Resize searchResults_Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (searchResults_isResizing)
            {
                Point mouseDelta = Mouse.GetPosition(this);
                double deltaY = mouseDelta.Y - mouseOffset.Y;
                mouseOffset.Y = mouseDelta.Y;

                if ((searchResults_Grid.Height - deltaY > 17) && (this.Height - 63 - searchResults_Grid.Height + deltaY >= 0))
                {
                    searchResults_Grid.Height -= deltaY;
                }
            }
        }

        /// <summary>
        /// Start resize searchResults_Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchResults_Resize_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseOffset = Mouse.GetPosition(this);
            searchResults_isResizing = true;
        }

        /// <summary>
        /// Event on mouse wheel using
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void searchResults_RichTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // CTRL + MouseWheel
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                /// Scale up
                if (e.Delta > 0)
                {
                    searchResults_RichTextBox.FontSize += 1;
                }
                /// Scale down
                if (e.Delta < 0 && textBoxMain.FontSize > 1)
                {
                    searchResults_RichTextBox.FontSize -= 1;
                }
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                searchResults_RichTextBox.ScrollToHorizontalOffset(searchResults_RichTextBox.HorizontalOffset - e.Delta);
                e.Handled = true;
            }
            else
            {
                searchResults_RichTextBox.ScrollToVerticalOffset(searchResults_RichTextBox.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void CloseSearchResults(object sender, RoutedEventArgs e)
        {
            searchResults_Grid.Visibility = Visibility.Collapsed;
            searchResults_Grid.Height = 0;
        }
        //

        // TextBox events
        /// <summary>
        /// Event on lost focus
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private void textBoxMain_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
        }

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
			int currentLine = textBoxMain.GetLineIndexFromCharacterIndex(textBoxMain.SelectionStart) + 1;

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

            findWindow.currentLine_Label.Content = "current line is " + currentLine.ToString();
            findWindow.goToInput_TextBox.Text = currentLine.ToString();
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
			{
				EditButton_Redo.Style = (Style)Resources["ClickableMenuItemBlack"];
			}
			else
			{
				EditButton_Redo.Style = (Style)Resources["UnclickableMenuItem"];
			}

			if (textBoxMain.CanUndo == true)
			{
				EditButton_Undo.Style = (Style)Resources["ClickableMenuItemBlack"];
			}
			else
			{
				EditButton_Undo.Style = (Style)Resources["UnclickableMenuItem"];
			}

			if (!String.IsNullOrEmpty(textFile.Path))
			{
				WindowName.Content = "*" + textFile.Path + " - SimplePad";
                this.Title = "*" + textFile.Path + " - SimplePad";
            }
			else
			{
				WindowName.Content = "*" + "SimplePad";
				this.Title = "*" + "SimplePad";
            }

			findWindow.lineCounter.Content = "line amount: " + textBoxMain.LineCount.ToString();
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
        /// Event on mouse left button up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalHookMouseUpExt(object sender, MouseEventExtArgs e)
        {
            searchResults_isResizing = false;
        }

        /// <summary>
        /// Change title after save, opening and other operations with text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            searchResults_RectangleTitle.Width = this.Width - 12;
            searchResults_RectangleUpResizer.Width = this.Width - 12;
        }

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
            this.Title = textFile.Path + " - SimplePad";
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
        /// Find next on f3;
        /// <br/>
        /// Find previous on shift + f3;
        /// <br/>
        /// Prevent bugged default title bar context menu from appearing on alt+Space;
        /// <br/>
        /// Save text file on ctrl+s;
        /// <br/>
        /// Open text file on ctrl+o
        /// <br/>
        /// Open find tab item in search window on ctrl+f
        /// <br/>
		/// Open 'find in files' tab item in search window on ctrl+shift+f
        /// <br/>
        /// Open replace tab item in search window on ctrl+h
		/// <br/>
        /// Open GoTo tab item in search window on ctrl+g
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
		{
            // F3
            if (e.Key == Key.F3)
            {
				if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				{
					FindPrevious();

                    e.Handled = true;
                }
				else
				{
					FindNext();

					e.Handled = true;
				}
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
            // CTRL + F
            if (e.Key == Key.F && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
				// NO SHIFT
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    OpenSearchWindow(0);
                }
				// SHIFT
				else
                {
                    OpenSearchWindow(2);
                }

                e.Handled = true;
            }
            // CTRL + H
            if (e.Key == Key.H && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                OpenSearchWindow(1);

                e.Handled = true;
            }
            // CTRL + G
            if (e.Key == Key.G && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                OpenSearchWindow(3);

                e.Handled = true;
            }
            // CTRL + O
            if (e.Key == Key.O && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
                textBoxMain.TextChanged -= textBoxMain_TextChanged;

                textBoxMain.Text = textFile.OpenFile(textBoxMain.Text);

                textBoxMain.TextChanged += textBoxMain_TextChanged;

                e.Handled = true;
			}
			// CTRL + S
			if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
                // SHIFT
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
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
                string textFilePath = "";
                if (textFile.Path.Length > 45)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        textFilePath += textFile.Path[j];
                    }

                    if (!textFilePath.EndsWith("\\"))
                    {
                        textFilePath += "...";
                        textFilePath += "\\";
                        textFilePath += "...";
                    }
                    else
                    {
                        textFilePath += "...";
                    }

                    for (int j = textFile.Path.Length - 20; j < textFile.Path.Length; j++)
                    {
                        textFilePath += textFile.Path[j];
                    }
                }
                else
                {
                    textFilePath = textFile.Path;
                }

                CloseSaveWindow closeSaveWindow = new()
                {
                    Left = this.Left + this.Width / 2,
                    Top = this.Top + this.Height / 2
                };

                if (textFilePath == "")
                    closeSaveWindow.MainText.Content = "Do you want to save the file?";
                else
                    closeSaveWindow.MainText.Content = "Do you want to save changes to a file \n\"" + textFilePath + "\"?";
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
            else
            {
                Close();
            }
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