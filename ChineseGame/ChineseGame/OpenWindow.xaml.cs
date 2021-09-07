using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace ChineseGame
{
    public partial class OpenWindow : Window
    {
        //Window constructor
        public OpenWindow()
        {
            InitializeComponent();
        }

        //On New button click
        public void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //Create and show new editor window with false load argument, close this window
            MainWindow Editor = new MainWindow(false);
            Editor.Show();
            this.Close();
        }

        //On Load button click
        public void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //Create file dialog object
            OpenFileDialog LoadDialog = new OpenFileDialog();

            //Set to only display worksheet files
            LoadDialog.Filter = "Save files (.wsheet)|*.wsheet";

            //Init variable for holding loaded file in plaintext
            string content = "";

            //Display and load file
            if (LoadDialog.ShowDialog() == true)
            {
                string fileName = LoadDialog.FileName; //Get filename
                Stream fileStream = LoadDialog.OpenFile(); //Create filestream and open file
                using (StreamReader reader = new StreamReader(fileStream)) //Using filestream reader use opened file to read into content variable
                {
                    content = reader.ReadToEnd();
                }

                //Create show new editor window with load argument true and pass content, then close this window
                MainWindow Editor = new MainWindow(true, content);
                Editor.Show();
                this.Close();
            }
        }
    }
}
