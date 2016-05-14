using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RipStudio.Pages
{
    /// <summary>
    /// Interaction logic for ChapterEditor.xaml
    /// </summary>
    public partial class ChapterEditor : UserControl
    {
        public ObservableCollection<Customer2> data;
        public ChapterEditor()
        {
            InitializeComponent();
            data = new ObservableCollection<Customer2>();
            DG1.DataContext = data;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DG1.SelectedItem != null)
            {
                try { data.Remove((Customer2)DG1.SelectedItem); }
                catch { }
            }
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            if (DG1.SelectedIndex > -1)
            {
                data.Insert(DG1.SelectedIndex, new Customer2());
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (data.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Chapter File";
                dlg.Filter = "Chapter File|*.txt"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    string TEXT = string.Empty;
                    for (int i = 0; i < data.Count; i++)
                    {
                        Customer2 ls = DG1.Items.GetItemAt(i) as Customer2;
                        if (ls.Timecode != string.Empty)
                        {
                            TEXT += "CHAPTER" + (i + 1).ToString("00") + "=" + ls.Timecode + "\r\n";
                            TEXT += "CHAPTER" + (i + 1).ToString("00") + "NAME=" + ls.Name + "\r\n";
                        }
                    }
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(dlg.FileName,false,Encoding.UTF8);
                    SW.Write(TEXT);
                    SW.Flush();
                    SW.Close();
                }

            }
            else
            {
                ModernDialog.ShowMessage("没有输入任何章节信息", "RipStudio Message", MessageBoxButton.OK);
            }

        }
    }
}
