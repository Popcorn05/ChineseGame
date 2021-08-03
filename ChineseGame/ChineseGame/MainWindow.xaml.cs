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
        const double GridSizePixels = 350.0;
        Grid PreviewGrid;
        //Word data
        private List<string[]> WordData;
        private int MaxWords;

        //Constructor
        public MainWindow()
        {
            //Init window
            InitializeComponent();
            
            //Create start grid
            PreviewGrid = new Grid();

            //Add style
            PreviewGrid.Style = (Style)Resources["PreviewGridStyling"];

            //Add to body grid
            BodyGrid.Children.Add(PreviewGrid);
            Grid.SetColumn(PreviewGrid, 1);
            Grid.SetRow(PreviewGrid, 2);

            //Update grid size
            UpdateGridSize();
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

        //Update Grid Size
        public void UpdateGridSize()
        {
            //Clear current
            PreviewGrid.RowDefinitions.Clear();
            PreviewGrid.ColumnDefinitions.Clear();

            //Add rows and columns
            RowDefinition PreviewRow = new RowDefinition();
            PreviewRow.Height = new GridLength(Math.Round(GridSizePixels / GridSize));
            ColumnDefinition PreviewColumn = new ColumnDefinition();
            PreviewColumn.Width = new GridLength(Math.Round(GridSizePixels / GridSize));

            for (int i = 0; i < GridSize; i++)
            {
                PreviewGrid.RowDefinitions.Add(new RowDefinition() { Height = PreviewRow.Height });
                PreviewGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = PreviewColumn.Width });
            }
        }

        //Update grid size overload for form elements
        public void UpdateSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GridSize = (int)e.NewValue;
        }
    }
}