using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using TextFile_Lib;

namespace SimplePad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    
    public partial class App : Application
    {
        static internal TextFile textFile = new();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            foreach (var arg in e.Args)
            {
                textFile.Path = arg;
            }
        }
    }
}
