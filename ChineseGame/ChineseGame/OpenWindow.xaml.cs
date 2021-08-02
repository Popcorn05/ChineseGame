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
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
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
            MainWindow Editor = new MainWindow();
            Editor.Show();
            this.Close();
        }

        //On Load button click
        public void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //Create file dialog object
            OpenFileDialog LoadDialog = new OpenFileDialog();

            //Get default saves location
            var SavesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WorksheetSaves");
            LoadDialog.InitialDirectory = SavesPath; //Set initial directory to saves dir

            //Set to only display worksheet files
            LoadDialog.DefaultExt = ".wsheet";
            LoadDialog.Filter = "Save files (.wsheet)|*.wsheet";

            //Display and load file
            if (LoadDialog.ShowDialog() == true)
            {
                
            }
        }
    }
}
