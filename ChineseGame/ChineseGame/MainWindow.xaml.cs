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
        private List<TextBox[]> WordDataObjects;
        private List<Button> WordDataButtons;
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
            PreviewGrid.Background = Brushes.White;

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

            //Instantiate word data lists
            WordData = new List<string[]>();
            WordDataObjects = new List<TextBox[]>();
            WordDataButtons = new List<Button>();

            //Word data first row
            AddWordDataRow();
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
            PreviewGrid.Children.Clear();
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
            TestLabel.Content = "Grid Size: " + GridSize.ToString();
            //Only run update grid if preview grid exists
            if (PreviewGrid != null)
            {
                UpdateGridSize();
            }
        }

        //Update word data grid
        public void WordDataUpdate(object sender, TextChangedEventArgs e)
        {
            //Get current row num
            int WordRowNum = WordDataObjects.Count;

            //If any textbox is filled in bottom row add row
            if (WordDataObjects[WordRowNum-1][0].Text != "" || WordDataObjects[WordRowNum-1][1].Text != "" || WordDataObjects[WordRowNum-1][2].Text != "")
            {
                AddWordDataRow();
                WordDataButtons[WordRowNum - 1].IsEnabled = true;
            }
        }

        //Add row to word data grid
        public void AddWordDataRow()
        {
            //Add new row def
            WordDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40.0) });

            //Add row to objects list
            WordDataObjects.Add(new TextBox[] { new TextBox(), new TextBox(), new TextBox() });

            //Get current row num (w/ new row)
            int WordRowNum = WordDataObjects.Count;

            //Loop through and format textboxes
            for (int i = 0; i < 3; i++)
            {
                WordDataObjects[WordRowNum - 1][i].TextChanged += new TextChangedEventHandler(WordDataUpdate);
                WordDataGrid.Children.Add(WordDataObjects[WordRowNum-1][i]);
                Grid.SetColumn(WordDataObjects[WordRowNum-1][i], i);
                Grid.SetRow(WordDataObjects[WordRowNum-1][i],WordRowNum-1);
            }

            //Add button to list and grid and format
            WordDataButtons.Add(new Button() { Content = "X" });
            WordDataButtons[WordRowNum - 1].Click += new RoutedEventHandler(WordDataButtonClick);
            WordDataGrid.Children.Add(WordDataButtons[WordRowNum-1]);
            Grid.SetColumn(WordDataButtons[WordRowNum-1], 3);
            Grid.SetRow(WordDataButtons[WordRowNum-1], WordRowNum-1);

            //Always disable at first
            WordDataButtons[WordRowNum - 1].IsEnabled = false;
        }

        //Remove row from word data grid
        public void RemoveWordDataRow(int RowNum)
        {
            //Remove objects as grid children
            for (int i = 0; i < 3; i++)
            {
                WordDataGrid.Children.Remove(WordDataObjects[RowNum][i]);
            }
            WordDataGrid.Children.Remove(WordDataButtons[RowNum]);

            //Remove row
            WordDataGrid.RowDefinitions.RemoveAt(RowNum);

            //Remove objects from respective lists
            WordDataObjects.RemoveAt(RowNum);
            WordDataButtons.RemoveAt(RowNum);

            //Update grid positions
            for (int r = 0; r < WordDataObjects.Count(); r++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Grid.SetRow(WordDataObjects[r][i], r);
                }
                Grid.SetRow(WordDataButtons[r], r);
            }
        }

        //Event handler for word data gridrow remove button
        public void WordDataButtonClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < WordDataButtons.Count(); i++)
            {
                if (WordDataButtons[i] == sender)
                {
                    RemoveWordDataRow(i);
                }
            }
        }

        //Open save window
        public void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveWindow Save = new SaveWindow();
            Save.Show();
        }
    }
}