using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFile_Lib
{
    public class TextFile_x2Container
    {
        // Fields
        public TextFile textFile1;
        public TextFile textFile2;
        //

        // Methods
        public TextFile_x2Container(TextFile _textFile1, TextFile _textFile2)
        {
            textFile1 = _textFile1;
            textFile2 = _textFile2;
        }
        //
    }
}