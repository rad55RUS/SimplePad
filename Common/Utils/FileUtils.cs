using System.Collections.Generic;
using System.IO;

namespace SimplePad.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<string> GetFiles(string directory, bool subfolders, List<string> extensions)
        {
            List<string> files = new();

            foreach (string file in Directory.GetFiles(directory))
            {
                if (extensions.Contains(Path.GetExtension(file)))
                {
                    files.Add(file);
                }
            }

            if (subfolders)
            {
                foreach (string subdirectory in Directory.GetDirectories(directory))
                {
                    files.AddRange(GetFiles(subdirectory, subfolders, extensions));
                }
            }

            return files;
        }
    }
}