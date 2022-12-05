using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFile_Lib
{
    /// <summary>
    /// Provides necessary methods for developing text file classes
    /// </summary>
    public interface ITextFilePublicMethods
    {
        public void RenameOrMoveFile();
        public void SaveFile(string text);
        public void SaveFile(string text, bool SaveAs);
        public string OpenFile(string text, ref Encoding encoding);
        public string OpenFile(string text);
    }
}