using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CreateUpdateWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string richText = new TextRange(richTBx_Sheet.Document.ContentStart, richTBx_Sheet.Document.ContentEnd).Text;
                string[] arrText = richText.Split("\r\n");

                if (arrText.Length > 0)
                {
                    Regex regex = new Regex("([А-Яа-я]+\\s\\([0-9\\.\\:\\s]+\\):\\s)(([\\w\\W]+)(publish_([0-9]{2}))\\\\([\\w\\W]+))");
                    Match firstMatch = regex.Match(arrText[0]);

                    using (FileStream fs = new FileStream(firstMatch.Groups[3].Value + "update_" + firstMatch.Groups[5].Value + ".zip", FileMode.Create))
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        foreach (string str in arrText)
                        {
                            Match strMatch = regex.Match(str);
                            if (strMatch.Success)
                            {
                                var entry = zip.CreateEntry(strMatch.Groups[6].Value);
                                using (var entryStream = entry.Open())
                                using (var fileStream = new FileStream(strMatch.Groups[2].Value, FileMode.Open))
                                {
                                    fileStream.CopyTo(entryStream);
                                }
                            }
                        }

                    }
                }

                MessageBox.Show("Файл успешно создан!");
                richTBx_Sheet.Document.Blocks.Clear();
            } 
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
