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

namespace RipStudio.Pages.ScriptCreater.VideoContent
{
    /// <summary>
    /// Interaction logic for Subtitle.xaml
    /// </summary>
    public partial class Subtitle : UserControl
    {
        public Subtitle()
        {
            InitializeComponent();
            ((App)(Application.Current)).VideoSubtitle = this;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ass"; // Default file extension
            dlg.Filter = "Advanced SubStation Alpha|*.ass"; // Filter files by extension
            dlg.Multiselect = true;
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    TreeViewItem TVI = new TreeViewItem();
                    TVI.Header= dlg.FileNames[i];
                    SubList.Items.Add(TVI);
                }
                SubList.IsExpanded = true;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            SubList.Items.Remove(SubListTV.SelectedItem);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SubList.Items.Clear();
        }
    }
}
