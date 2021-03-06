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
using System.Text.Json;

namespace ChineseGame
{
    public partial class MainWindow : Window
    {
        //SETUP--------------------------------------------------

        //Declare variables
        //Titles
        private string SheetTitle = ""; //Worksheet title
        private string SheetTitleChinese = ""; //Chinese worksheet title
        //Grid
        private int GridSize = 6; //Size of grid
        private string[,] GridData; //Holds each character of each position in grid
        private bool GridGenned = false; //Whether grid has been generated
        const double GridSizePixels = 275.0; //Grid size in editor in pixels
        private Grid PreviewGrid; //WPF Grid object for generated grid
        private Border GridBorder; //WPF Border object for generated grid
        private List<Border> GridBorders; //List of WPF Border objects for generated grid
        private List<List<Label>> GridLabels; //List of WPF Label objects for the generated grid
        //Word data
        private int WordDataGridSize = 0; //Number of words in worksheet 
        private List<string[]> WordData; //Word data (Chinese, pinyin and english) of worksheet
        private List<TextBox[]> WordDataObjects; //List of WPF Textboxes for word input
        private List<Button> WordDataButtons; //List of WPF Buttons to remove rows
        private List<String> WordChars; //All characters contained within words, used for filling empty spaces during grid generation

        //Constructor
        public MainWindow(bool load, string content = "")
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
            GridLabels = new List<List<Label>>();

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
            WordChars = new List<string>();

            //Word data first row
            AddWordDataRow();

            //If loading load in data
            if (load == true)
            {
                //Deserialize project data
                List<string[]> loadData = JsonSerializer.Deserialize<List<string[]>>(content);

                //Move deserialized data into variables
                SheetTitle = loadData[0][0];
                SheetTitleChinese = loadData[0][1];
                WorksheetTitleInput.Text = SheetTitle;
                WorksheetTitleChineseInput.Text = loadData[0][1];

                GridSize = Int32.Parse(loadData[0][2]);
                GridSlider.Value = GridSize;
                
                //Implement data into editor
                for (int r = 1; r < loadData.Count(); r++)
                {
                    WordData.Add(loadData[r]);
                    for (int w = 0; w < 3; w++)
                    {
                        WordDataObjects[r - 1][w].Text = loadData[r][w];
                    }
                    AddWordDataRow();
                    WordDataButtons[r - 1].IsEnabled = true;
                }
                RemoveWordDataRow(WordDataGrid.RowDefinitions.Count() - 1);
                UpdateGridSize();
            }
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
            GridGenned = false;

            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
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

            //Reset grid genned
            GridGenned = false;

            //Make sure doent go over character limit
            for (int r = 0; r < WordRowNum; r++)
            {
                if (WordDataObjects[r][0].Text.Length > GridSize)
                {
                    WordDataObjects[r][0].Text = WordDataObjects[r][0].Text.Substring(0, GridSize);
                }
            }

            SaveButton.IsEnabled = false;
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

            //Increase word data grid size var
            WordDataGridSize = WordDataObjects.Count - 1;
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

            //Decrease word data grid size var
            WordDataGridSize = WordDataObjects.Count - 1;
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
            SaveWindow Save = new SaveWindow(SheetTitle, SheetTitleChinese, GridSize.ToString(), WordData, GridData, WordDataGridSize.ToString());
            Save.Show();
        }

        //Generate button click event
        public void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            if (WordDataObjects[0][0].Text != "")
            {
                Generate();
            }
        }

        //Generate grid
        public void Generate()
        {
            //Clear old
            WordData.Clear();
            GridData = new string[GridSize, GridSize];
            UpdateGridSize();
            GridLabels.Clear();
            WordChars.Clear();

            //Get word data from table
            for (int r = 0; r < WordDataGrid.RowDefinitions.Count()-1; r++)
            {
                WordData.Add(new string[3]);
                for (int i = 0; i < 3; i++)
                {
                    WordData[r][i] = WordDataObjects[r][i].Text;
                }
            }

            //Place words in grid
            List<int> IncludedWords = new List<int>();
            int Loop = 0; //Which loop through
            Random Chance = new Random();
            while (Loop < 4)
            {
                for (int w = 0; w < WordData.Count(); w++)
                {
                    if (Chance.Next() > (Loop * 25))
                    {
                        //Split word in string array
                        string[] CurWordSplit = new string[WordData[w][0].Length];
                        for (int l = 0; l < WordData[w][0].Length; l++)
                        {
                            CurWordSplit[l] = WordData[w][0].Substring(l, 1);
                            WordChars.Add(CurWordSplit[l]);
                        }
                        bool WordAdded = false;
                        for (int y = 0; y < GridSize && WordAdded == false; y = Chance.Next(GridSize+1))
                        {
                            for (int x = 0; x < GridSize && WordAdded == false; x = Chance.Next(GridSize+1))
                            {
                                if (GridData[y, x] == null)
                                {
                                    if (CurWordSplit.Length > 1)
                                    {
                                        int dir = Chance.Next(4); //Direction variable goes up for check in order n,s,e,w
                                        while (dir < 4 && WordAdded == false)
                                        {
                                            //Define thingos for increment dir on word check
                                            int xx = x;
                                            int yy = y;
                                            while (true) //Yes I know this is dangerous, but it works
                                            {
                                                //Increment based on direction
                                                switch (dir)
                                                {
                                                    case 0:
                                                        yy -= 1;
                                                        break;
                                                    case 1:
                                                        yy += 1;
                                                        break;
                                                    case 2:
                                                        xx += 1;
                                                        break;
                                                    case 3:
                                                        xx -= 1;
                                                        break;
                                                }
                                                //If out of bounds new dir
                                                if (xx < 0 || xx >= GridSize || yy < 0 || yy >= GridSize)
                                                {
                                                    dir += 1;
                                                    break;
                                                }
                                                //Check if space for this run
                                                if (GridData[yy, xx] == null)
                                                {
                                                    //If end of word
                                                    if (Math.Abs(x - xx) == CurWordSplit.Length || Math.Abs(y - yy) == CurWordSplit.Length - 1)
                                                    {
                                                        if (Chance.Next(100) > (75))
                                                        {
                                                            //Add index of word added (will have to tally multiples if answer output ends up existing)
                                                            IncludedWords.Add(w);
                                                            //Add to data
                                                            xx = x;
                                                            yy = y;
                                                            for (int i = 0; i < CurWordSplit.Length; i++)
                                                            {
                                                                GridData[yy, xx] = CurWordSplit[i];
                                                                switch (dir)
                                                                {
                                                                    case 0:
                                                                        yy -= 1;
                                                                        break;
                                                                    case 1:
                                                                        yy += 1;
                                                                        break;
                                                                    case 2:
                                                                        xx += 1;
                                                                        break;
                                                                    case 3:
                                                                        xx -= 1;
                                                                        break;
                                                                }
                                                            }
                                                            //Stop checking directions
                                                            WordAdded = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dir += 1;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else //If current word only 1 long
                                    {
                                        if (Chance.Next(100) > (55))
                                        {
                                            IncludedWords.Add(w);
                                            GridData[y, x] = CurWordSplit[0];
                                            WordAdded = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Make sure each word is in once
                if (Loop == 0)
                {
                    for (int w = 0; w < WordData.Count(); w++)
                    {
                        //Check if word has been included
                        bool WordIncluded = false;
                        for (int j = 0; j < IncludedWords.Count(); j++)
                        {
                            if (IncludedWords[j] == w)
                            {
                                WordIncluded = true;
                            }
                        }

                        //If not word added then make sure is added
                        //This is completely copy pasted but for loops slightly altered
                        //so it goes through every possible location left-right, top-bottom, not random
                        if (WordIncluded == false)
                        {
                            //Split word in string array
                            string[] CurWordSplit = new string[WordData[w][0].Length];
                            for (int l = 0; l < WordData[w][0].Length; l++)
                            {
                                CurWordSplit[l] = WordData[w][0].Substring(l, 1);
                                WordChars.Add(CurWordSplit[l]);
                            }
                            bool WordAdded = false;
                            for (int y = 0; y < GridSize && WordAdded == false; y++)
                            {
                                for (int x = 0; x < GridSize && WordAdded == false; x++)
                                {
                                    if (GridData[y, x] == null)
                                    {
                                        if (CurWordSplit.Length > 1)
                                        {
                                            int dir = Chance.Next(4); //Direction variable goes up for check in order n,s,e,w
                                            while (dir < 4 && WordAdded == false)
                                            {
                                                //Define thingos for increment dir on word check
                                                int xx = x;
                                                int yy = y;
                                                while (true)
                                                {
                                                    //Increment based on direction
                                                    switch (dir)
                                                    {
                                                        case 0:
                                                            yy -= 1;
                                                            break;
                                                        case 1:
                                                            yy += 1;
                                                            break;
                                                        case 2:
                                                            xx += 1;
                                                            break;
                                                        case 3:
                                                            xx -= 1;
                                                            break;
                                                    }
                                                    //If out of bounds new dir
                                                    if (xx < 0 || xx >= GridSize || yy < 0 || yy >= GridSize)
                                                    {
                                                        dir += 1;
                                                        break;
                                                    }
                                                    //Check if space for this run
                                                    if (GridData[yy, xx] == null)
                                                    {
                                                        //If end of word
                                                        if (Math.Abs(x - xx) == CurWordSplit.Length || Math.Abs(y - yy) == CurWordSplit.Length - 1)
                                                        {
                                                            if (Chance.Next(100) > (75))
                                                            {
                                                                //IncludedWords.Add(w);
                                                                //Add to data
                                                                xx = x;
                                                                yy = y;
                                                                for (int i = 0; i < CurWordSplit.Length; i++)
                                                                {
                                                                    GridData[yy, xx] = CurWordSplit[i];
                                                                    switch (dir)
                                                                    {
                                                                        case 0:
                                                                            yy -= 1;
                                                                            break;
                                                                        case 1:
                                                                            yy += 1;
                                                                            break;
                                                                        case 2:
                                                                            xx += 1;
                                                                            break;
                                                                        case 3:
                                                                            xx -= 1;
                                                                            break;
                                                                    }
                                                                }
                                                                //Stop checking directions
                                                                WordAdded = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dir += 1;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else //If current word only 1 long
                                        {
                                            if (Chance.Next(100) > (55))
                                            {
                                                //IncludedWords.Add(w);
                                                GridData[y, x] = CurWordSplit[0];
                                                WordAdded = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Increment number of loops
                Loop++;
            }
            //Fill blank spots
            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    if (GridData[y,x] == null)
                    {
                        GridData[y, x] = WordChars[Chance.Next(WordChars.Count())];
                    }
                }
            }

            //Display
            for (int y = 0; y < GridSize; y++)
            {
                GridLabels.Add(new List<Label>());
                for (int x = 0; x < GridSize; x++)
                {
                    GridLabels[y].Add(new Label { Content = GridData[y, x] });
                    GridLabels[y][x].HorizontalAlignment = HorizontalAlignment.Center;
                    GridLabels[y][x].VerticalAlignment = VerticalAlignment.Center;
                    GridLabels[y][x].FontFamily = new FontFamily("Microsoft YaHei");
                    GridLabels[y][x].FontSize = GridLabels[y][x].FontSize + 2*(10 - GridSize);
                    PreviewGrid.Children.Add(GridLabels[y][x]);
                    Grid.SetColumn(GridLabels[y][x], x);
                    Grid.SetRow(GridLabels[y][x], y);
                }
            }
            GridGenned = true;
            SaveButton.IsEnabled = true;
        }
    }
}