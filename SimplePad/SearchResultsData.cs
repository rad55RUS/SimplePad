using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePad
{
    internal class SearchInFilesArgs
    {
        internal string desiredString;
        internal string ?replaceString;
        internal string directory;
        internal bool anyCase;
        internal bool subfolders;

        internal SearchInFilesArgs(string _desiredString, string _directory, bool _anyCase, bool _subfolders)
        {
            desiredString = _desiredString;
            directory = _directory;
            anyCase = _anyCase;
            subfolders = _subfolders;
        }

        internal SearchInFilesArgs(string _desiredString, string _replaceString, string _directory, bool _anyCase, bool _subfolders)
        {
            desiredString = _desiredString;
            replaceString = _replaceString;
            directory = _directory;
            anyCase = _anyCase;
            subfolders = _subfolders;
        }
    }
}