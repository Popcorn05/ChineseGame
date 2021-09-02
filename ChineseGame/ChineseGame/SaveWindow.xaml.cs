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
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;
using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace ChineseGame
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        private string SheetTitle = "";
        private string SheetTitleChinese = "";
        private string GridSize = "";
        private string[,] GridData;
        private string WordDataGridSize;
        private List<string[]> WordData;

        private string filePath;
        public SaveWindow(string SheetTitleData, string SheetTitleChineseData, string GridSizeData, List<string[]> WordDataData, string[,] GridDataData, string WordDataGridSizeData)
        {
            InitializeComponent();

            SheetTitle = SheetTitleData;
            SheetTitleChinese = SheetTitleChineseData;
            GridSize = GridSizeData;
            WordData = WordDataData;
            GridData = GridDataData;
            WordDataGridSize = WordDataGridSizeData;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AsProjectRadioButton.IsChecked == true)
            {
                filePath += ".wsheet";

                List<string[]> jsonData = new List<string[]>();

                jsonData.Add( new string[] { SheetTitle, SheetTitleChinese, GridSize });
                for (int r = 0; r < WordData.Count(); r++)
                {
                    jsonData.Add(new string[] { WordData[r][0], WordData[r][1], WordData[r][2] });
                }

                string jsonSaveData = JsonSerializer.Serialize(jsonData.ToArray());
                File.WriteAllText(filePath, jsonSaveData);

                MessageBox.Show("Project Saved");
            } else
            {
                filePath += ".pdf";
                Document outPDF = CreateWorksheetPDF();
                outPDF.Save(filePath);

                MessageBox.Show("PDF Exported");
            }
            this.Close();
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            //File location
            SaveFileDialog SaveDialog = new SaveFileDialog();

            if (SaveDialog.ShowDialog() == true)
            {
                filePath = SaveDialog.FileName;
                SaveButton.IsEnabled = true;
                ChooseFileLocationButton.Content = filePath;
            }
        }

        private Document CreateWorksheetPDF()
        {
            Document outDoc = new Document();

            Aspose.Pdf.Page outPage = outDoc.Pages.Add();

            var engHeader = new TextFragment(SheetTitle);
            var chinHeader = new TextFragment(SheetTitleChinese);

            engHeader.TextState.Font = FontRepository.FindFont("Arial");
            engHeader.TextState.FontSize = 20;
            engHeader.Margin.Bottom = 15;
            engHeader.Margin.Top = 0;
            engHeader.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;

            chinHeader.TextState.Font = FontRepository.FindFont("Arial");
            chinHeader.TextState.FontSize = 20;
            chinHeader.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;

            Aspose.Pdf.Table gridTable = new Aspose.Pdf.Table
            {
                DefaultCellPadding = new MarginInfo(),
                Margin = { Bottom = 30, Top = 30, Left = 20 * (11 - Int16.Parse(GridSize)), Right = 0 },

                //ColumnAdjustment = ColumnAdjustment.AutoFitToContent,
                DefaultColumnWidth = "35",

                DefaultCellTextState =
				{
					Font = FontRepository.FindFont("Arial"),
                    HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center,
					FontSize = 20
				}
			};

            gridTable.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center;

            gridTable.DefaultCellPadding.Top = 7;
            gridTable.DefaultCellPadding.Left = 0;
            gridTable.DefaultCellPadding.Right = 0;
            gridTable.DefaultCellPadding.Bottom = 7;

            gridTable.Border = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f, Aspose.Pdf.Color.FromRgb(System.Drawing.Color.LightGray));
            gridTable.DefaultCellBorder = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f, Aspose.Pdf.Color.FromRgb(System.Drawing.Color.LightGray));

            for (int row_count = 0; row_count < Int16.Parse(GridSize); row_count++)
            {

                Aspose.Pdf.Row gridRow = gridTable.Rows.Add();

                for (int r = 0; r < Int16.Parse(GridSize); r++)
                {
                    gridRow.Cells.Add(GridData[row_count, r]);
                }

            }

            Aspose.Pdf.Table wordTable = new Aspose.Pdf.Table();

            wordTable.Border = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f, Aspose.Pdf.Color.FromRgb(System.Drawing.Color.LightGray));
            wordTable.DefaultCellBorder = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f, Aspose.Pdf.Color.FromRgb(System.Drawing.Color.LightGray));
            wordTable.ColumnWidths = "25%";

            wordTable.DefaultCellPadding = new MarginInfo();
            wordTable.DefaultCellPadding.Top = 5;
            wordTable.DefaultCellPadding.Bottom = 5;
            wordTable.DefaultCellPadding.Left = 3;
            wordTable.DefaultCellTextState.FontSize = 10;

            Aspose.Pdf.Row titleRow = wordTable.Rows.Add();
            titleRow.DefaultCellTextState.FontSize = 12;
            titleRow.Cells.Add("Chinese");
            titleRow.Cells.Add("Pinyin");
            titleRow.Cells.Add("English");
            titleRow.Cells.Add("Count");

            for (int row_count = 0; row_count < Int16.Parse(WordDataGridSize); row_count++)
            {
                // Add row to table
                Aspose.Pdf.Row wordRow = wordTable.Rows.Add();
                // Add table cells
                wordRow.Cells.Add(WordData[row_count][0]);
                wordRow.Cells.Add(WordData[row_count][1]);
                wordRow.Cells.Add(WordData[row_count][2]);
                wordRow.Cells.Add("");
            }

            outPage.Paragraphs.Add(engHeader);
            outPage.Paragraphs.Add(chinHeader);

            outPage.Paragraphs.Add(gridTable);
            outPage.Paragraphs.Add(wordTable);

            return outDoc;
        }
	}
}
