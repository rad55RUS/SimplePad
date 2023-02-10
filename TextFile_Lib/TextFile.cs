using static System.Net.Mime.MediaTypeNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.IO.Pipes;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using System.Runtime;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using static System.Net.WebRequestMethods;
using System.Windows.Shapes;

namespace TextFile_Lib
{
	/// <summary>
	/// Represents text file with necessary methods for integration into an application.
	/// </summary>
	public sealed class TextFile : File
	{
        // Static fields
        public static Encoding encoding;

		// Events
		#region
		public delegate void OnFileOperationHandler(object sender, EventArgs e);
		public event OnFileOperationHandler ?OnFileOperation;
        #endregion

        // Static methods
        /// <summary>
        /// Static method for reading text from file with specified directory.
        /// </summary>
        /// <param name="text"></param>
        public static string ReadFromFile(string directory)
        {
            // Encoding determining
            FileStream fileStream = System.IO.File.OpenRead(directory);

            Ude.CharsetDetector cDet = new();
            cDet.Feed(fileStream);
            cDet.DataEnd();
            if (cDet.Charset != null)
            {
                encoding = Encoding.GetEncoding(cDet.Charset);
            }
            fileStream.Dispose();
            fileStream.Close();

            // Read from file
            StreamReader reader = new(directory, encoding);
            string text = reader.ReadToEnd();
            reader.Dispose();
            reader.Close();
            //

            return text;
        }

        /// <summary>
        /// Static method for reading text from file with specified directory and encoding.
        /// </summary>
        /// <param name="text"></param>
        public static string ReadFromFile(string directory, Encoding encoding)
        {
            // Read from file
            StreamReader reader = new(directory, encoding);
            string text = reader.ReadToEnd();
            reader.Dispose();
            reader.Close();
            //

            return text;
        }
        //

        // Object methods
        /// <summary>
        /// Constructor
        /// </summary>
        public TextFile() : base()
		{
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 2;
			saveFileDialog.FileName = "TextFile.txt";

			openFileDialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 2;
		}

        /// <summary>
        /// Show <b>SaveFileDialog</b> for renaming or moving current opened file.
        /// </summary>
        public override void RenameOrMoveFile()
		{
            saveFileDialog.FileName = fullName;
            if (saveFileDialog.ShowDialog() == true)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    stream.Dispose();
                    stream.Close();
                    System.IO.File.Move(Path, saveFileDialog.FileName, true);

                    Path = saveFileDialog.FileName;

                    this.isSaved = true;

                    if (OnFileOperation != null)
						OnFileOperation.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// If saving not from opened file show <b>SaveFileDialog</b>.
        /// </summary>
        /// <param name="text"></param>
        public void SaveFile(string text)
		{
			if (Path == "")
			{
				if (saveFileDialog.ShowDialog() == true)
				{
					if ((stream = saveFileDialog.OpenFile()) != null)
					{
						Path = saveFileDialog.FileName;
						stream.Dispose();
						stream.Close();
						WriteToFile(text);
                    }
				}
			}
			else
			{
				WriteToFile(text);
            }
		}

        /// <summary>
        /// If <b>SaveAs</b> isn't false then show <b>SaveFileDialog</b> else call <b>SaveFile(string text)</b>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="SaveAs"></param>
        public void SaveFile(string text, bool SaveAs)
		{
			if (SaveAs == false)
			{
				SaveFile(text);
			}
			else
			{
				if (Path != "")
				{
					saveFileDialog.FileName = fullName;
				}
                if (saveFileDialog.ShowDialog() == true)
				{
					if ((stream = saveFileDialog.OpenFile()) != null)
					{
						Path = saveFileDialog.FileName;
						stream.Dispose();
						stream.Close();
						WriteToFile(text);
                    }
				}
			}
        }

        /// <summary>
        /// Show <b>OpenFileDialog</b> and return <b>text</b> of opened file.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string OpenFile(string text)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                if ((stream = openFileDialog.OpenFile()) != null)
                {
                    Path = openFileDialog.FileName;

                    // Encoding determining
                    Ude.CharsetDetector cDet = new();
                    cDet.Feed(stream);
                    cDet.DataEnd();
                    if (cDet.Charset != null)
                    {
                        encoding = Encoding.GetEncoding(cDet.Charset);
                    }
                    stream.Dispose();
                    stream.Close();
					//

					text = ReadFromFile();
                }
            }
            return text;
        }

        /// <summary>
        /// Method for writing text in file.
        /// </summary>
        /// <param name="text"></param>
        public void WriteToFile(string text)
		{
            if (OnFileOperation != null)
                OnFileOperation.Invoke(this, new EventArgs());

            StreamWriter writer = new(Path);
			writer.Write(text);
			this.isSaved = true;
			writer.Dispose();
			writer.Close();
		}

        /// <summary>
        /// Method for reading text from file with specified encoding.
        /// </summary>
        /// <param name="text"></param>
        public string ReadFromFile()
        {
            if (OnFileOperation != null)
                OnFileOperation.Invoke(this, new EventArgs());

            // Read from file
            StreamReader reader = new(Path, encoding);
            string text = reader.ReadToEnd();
            this.isSaved = true;
            reader.Dispose();
            reader.Close();
            //

            return text;
        }
		//
    }
}