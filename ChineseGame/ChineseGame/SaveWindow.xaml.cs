﻿using System;
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
        private List<string[]> WordData;

        private string filePath;
        public SaveWindow(string SheetTitleData, string SheetTitleChineseData, string GridSizeData, List<string[]> WordDataData, string[,] GridDataData)
        {
            InitializeComponent();

            SheetTitle = SheetTitleData;
            SheetTitleChinese = SheetTitleChineseData;
            GridSize = GridSizeData;
            WordData = WordDataData;
            GridData = GridDataData;
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

            outPage.Paragraphs.Add(new TextFragment("Hello World"));

            return outDoc;
        }
	}
}
