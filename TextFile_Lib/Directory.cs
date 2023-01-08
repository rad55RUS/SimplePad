using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFile_Lib
{
    /// <summary>
    /// Provides next public string fields and properties: <b>Path</b>, <b>name</b>, <b>extension</b>, <b>fullname</b>(name + extension) and <b>folder</b>. This is an abstract class.
    /// </summary>
    public abstract class Directory
	{
		// Private fields
        private string path = "";
        //

        // Public fields
        public string ?fullName;
		public string ?extension;
		public string ?name;
		public string ?folder;
		//

		// Properties
		public string Path 
		{ 
			get
			{
				return path;
			}
			set
			{
				path = value;
				extension = "";
				name = "";
				folder = "";
				for (int i = path.Length - 1; i >= 0; i--)
				{
					if (path[i] == '.')
					{
						i--;
						while (path[i] != '\\')
						{
							name = path[i] + name;
							i--;
						}
						i--;
						fullName = name + '.' + extension;
						while (i >= 0)
						{
							folder = path[i] + folder;
							i--;
						}
						Debug.Print(folder);
						break;
					}
					extension = path[i] + extension;
				}
			}
		}
		//
	}
}