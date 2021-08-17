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

namespace ChineseGame
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public string SheetTitle = "";
        public string SheetTitleChinese = "";
        public string GridSize = "";
        public List<string[]> WordData;
        public SaveWindow(string SheetTitleData, string SheetTitleChineseData, string GridSizeData, List<string[]> WordDataData)
        {
            InitializeComponent();

            SheetTitle = SheetTitleData;
            SheetTitleChinese = SheetTitleData;
            GridSize = GridSizeData;
            WordData = WordDataData;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
		{          
            string[] saveData = new string[] {SheetTitle, SheetTitleChinese, GridSize};

            string jsonSaveData = JsonSerializer.Serialize(saveData);
            string jsonWordData = JsonSerializer.Serialize(WordData);
            File.WriteAllText(@"C:\Users\FerdDan\OneDrive - Donvale Christian College\Desktop\path.json", jsonSaveData);
            File.AppendAllText(@"C:\Users\FerdDan\OneDrive - Donvale Christian College\Desktop\path.json", jsonWordData);
        }
	}
}
