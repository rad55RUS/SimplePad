using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace TextFile_Lib
{
    public class TextFile_1to2Divided
    {
        // Private fields
        public readonly TextFile_x2Container ?container;
        //

        // Public fields
        /// Paths
        public string path1 = "";
        public string path2 = "";
        ///
        /// Texts
        public string text1 = "";
        public string text2 = "";
        ///
        //

        // Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public TextFile_1to2Divided()
        {
            TextFile textFile1 = new TextFile();
            TextFile textFile2 = new TextFile();

            string fullText = "";

            fullText = textFile1.OpenFile(fullText);

            if (fullText.Length > 1)
            {
                for (int i = 0; i < fullText.Length; i++)
                {
                    if (i < fullText.Length / 2)
                    {
                        text1 += fullText[i];
                    }
                    else
                    {
                        text2 += fullText[i];
                    }
                }
                textFile1.SaveFile(text1);
                textFile2.saveFileDialog.FileName = textFile1.name + "2." + textFile1.extension;

                textFile2.SaveFile(text2);
                path1 = textFile1.Path;
                path2 = textFile2.Path;

                container = new TextFile_x2Container(textFile1, textFile2);
            }
        }

        /// <summary>
        /// Save text into certain file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Number"></param>
        public void SaveText(string text, int num)
        {
            if (container != null)
                if (num == 1)
                {
                    text1 = text;
                    container.textFile1.SaveFile(text1);
                }
                else
                {
                    text2 = text;
                    container.textFile2.SaveFile(text2);
                }
        }
        //
    }
}