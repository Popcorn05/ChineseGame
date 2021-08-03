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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChineseGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //SETUP--------------------------------------------------

        //Declare variables
        //Titles
        private string SheetTitle = "";
        private string SheetTitleChinese = "";
        //Grid
        private int GridSize = 6;
        private string[,] GridData;
        private bool GridGenned = false;
        //Word data
        private List<string[]> WordData;
        private int MaxWords;

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        //INPUTS-----------------------------------------------

        //Update titles
        public void UpdateTitles()
        {
            SheetTitle = WorksheetTitleInput.Text;
            SheetTitleChinese = WorksheetTitleChineseInput.Text;
        }
        
        //Update titles overload (for form elements)
        public void UpdateTitles(object sender, TextChangedEventArgs e)
        {
            SheetTitle = WorksheetTitleInput.Text;
            SheetTitleChinese = WorksheetTitleChineseInput.Text;
        }
    }
}