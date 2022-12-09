using static System.Net.Mime.MediaTypeNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using System.Runtime;
using Microsoft.Win32;
using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;
using Microsoft.VisualBasic.FileIO;

namespace TextFile_Lib
{
	/// <summary>
	/// Provides necessary members for developing file type classes. This is an abstract class.
	/// </summary>
	public abstract class File : Directory
	{
		internal readonly SaveFileDialog saveFileDialog = new();
		internal readonly OpenFileDialog openFileDialog = new();

		internal Stream? stream;

		public bool isSaved = true;

		public File() : base()
		{
			saveFileDialog.RestoreDirectory = true;
			openFileDialog.RestoreDirectory = true;
		}

        public void DeleteFile()
        {
            FileSystem.DeleteFile(Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);

            Path = "";

            this.isSaved = true;
        }

        public abstract void RenameOrMoveFile();
    }
}