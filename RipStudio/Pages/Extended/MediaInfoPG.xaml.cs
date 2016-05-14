using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using MediaInfoLib;
//using MediaInfoDotNet.Models;
//using MediaInfoDotNet;

namespace RipStudio.Pages
{
    /// <summary>
    /// Interaction logic for MediaInfo.xaml
    /// </summary>
    public partial class MediaInfoPG : UserControl
    {
        public MediaInfoPG()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop)!=null)
            {
                MediaInfo MI = new MediaInfo();
                MI.Open(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString());
                (sender as TextBox).Text = MI.Inform();
                MI.Close();
            }
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "All File|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                MediaInfo MI = new MediaInfo();
                MI.Open(dlg.FileName);
                MITB.Text = MI.Inform();
                MI.Close();
            }

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(MITB.Text);
        }



    }
}
