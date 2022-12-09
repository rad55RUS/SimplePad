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

namespace TextFile_Lib
{
	/// <summary>
	/// Represents text file with necessary methods for integration into an application.
	/// </summary>
	public sealed class TextFile : File, ITextFilePublicMethods
	{
		// Events
		#region
		public delegate void OnFileOperationHandler(object sender, EventArgs e);
		public event OnFileOperationHandler ?OnFileOperation;
        #endregion

		// Methods
		/// <summary>
		/// Constructor
		/// </summary>
        public TextFile() : base()
		{
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
		/// Show <b>OpenFileDialog</b> and return <b>text</b> and <b>encoding</b> of opened file.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public string OpenFile(string text, ref Encoding encoding)
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

                    text = ReadFromFile(text, encoding);
                }
			}
            return text;
		}

        /// <summary>
        /// Show <b>OpenFileDialog</b> and return <b>text</b> of opened file.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string OpenFile(string text)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                if ((stream = openFileDialog.OpenFile()) != null)
                {
					Encoding encoding = Encoding.UTF8;
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

					text = ReadFromFile(text, encoding);
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
        /// Method for reading text from file.
        /// </summary>
        /// <param name="text"></param>
        public string ReadFromFile(string text, Encoding encoding)
        {
            if (OnFileOperation != null)
                OnFileOperation.Invoke(this, new EventArgs());

            // Read from file
            StreamReader reader = new(Path, encoding);
            text = reader.ReadToEnd();
            this.isSaved = true;
            reader.Dispose();
            reader.Close();
            //

            return text;
        }
		//
    }
}