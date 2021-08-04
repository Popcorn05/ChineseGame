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
        const double GridSizePixels = 275.0;
        private Grid PreviewGrid;
        private Border GridBorder;
        private List<Border> GridBorders;
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

            //Add styles
            PreviewGrid.HorizontalAlignment = HorizontalAlignment.Center;
            PreviewGrid.VerticalAlignment = VerticalAlignment.Center;

            //Grid border
            GridBorder = new Border();
            GridBorder.BorderBrush = Brushes.Black;
            GridBorder.BorderThickness = new Thickness(4);
            GridBorder.Width = GridSizePixels;
            GridBorder.Height = GridSizePixels;
            GridBorder.Child = PreviewGrid;

            GridBorders = new List<Border>();

            //Add to body grid
            BodyGrid.Children.Add(GridBorder);
            Grid.SetColumn(GridBorder, 1);
            Grid.SetRow(GridBorder, 2);

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

            GridBorders.Clear();

            //Add rows and columns
            RowDefinition PreviewRow = new RowDefinition();
            PreviewRow.Height = new GridLength(Math.Round(GridSizePixels / GridSize));
            ColumnDefinition PreviewColumn = new ColumnDefinition();
            PreviewColumn.Width = new GridLength(Math.Round(GridSizePixels / GridSize));

            //Loop through to add rows and columns
            for (int p = 0; p < GridSize; p++)
            {
                PreviewGrid.RowDefinitions.Add(new RowDefinition() { Height = PreviewRow.Height });
                PreviewGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = PreviewColumn.Width });
            }

            //Loop through and add borders
            int i = 0;
            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    GridBorders.Add(new Border { BorderBrush = GridBorder.BorderBrush, BorderThickness = new Thickness(2) });
                    PreviewGrid.Children.Add(GridBorders[i]);
                    Grid.SetColumn(GridBorders[i], x);
                    Grid.SetRow(GridBorders[i], y);
                    i++;
                }
            }
        }

        //Update grid size overload for form elements
        public void UpdateSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Change grid size
            GridSize = (int)e.NewValue;
            TestLabel.Content = GridSize;
            //Only run update grid if preview grid exists
            if (PreviewGrid != null)
            {
                UpdateGridSize();
            }
        }
    }
}