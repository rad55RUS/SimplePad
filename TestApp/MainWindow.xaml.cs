using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextFile_Lib;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextFile_1to2Divided ?textFile_1To2Divided;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void divideTextFile_Click(object sender, RoutedEventArgs e)
        {
            TextFile_1to2Divided temp_textFile_1To2Divided = new TextFile_1to2Divided();
            if (temp_textFile_1To2Divided.container != null)
            {
                textFile_1To2Divided = temp_textFile_1To2Divided;

                firstFilePath_Label.Content = textFile_1To2Divided.path1;
                secondFilePath_Label.Content = textFile_1To2Divided.path2;

                first_TextBox.Text = textFile_1To2Divided.text1;
                second_TextBox.Text = textFile_1To2Divided.text2;

                first_TextBox.IsReadOnly = false;
                second_TextBox.IsReadOnly = false;

                divideTextFile_Button.Content = "Перезапустить";
                saveFirstFile_Button.Visibility = Visibility.Visible;
                saveSecondFile_Button.Visibility = Visibility.Visible;

                textFile_1To2Divided = temp_textFile_1To2Divided;
            }
        }

        private void saveFirstFile_Click(object sender, RoutedEventArgs e)
        {
            textFile_1To2Divided.SaveText(first_TextBox.Text, 1);
        }

        private void saveSecondFile_Click(object sender, RoutedEventArgs e)
        {
            textFile_1To2Divided.SaveText(second_TextBox.Text, 2);
        }
    }
}