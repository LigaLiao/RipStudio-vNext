using FirstFloor.ModernUI.Windows.Controls;
using libavs2flac;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace RipStudio.Pages.Audio
{
    /// <summary>
    /// Interaction logic for FLAC.xaml
    /// </summary>
    public partial class FLAC : UserControl
    {
        public FLAC()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (In.Text != string.Empty || Out.Text != string.Empty)
            {
                if (File.Exists(In.Text) || File.Exists(Out.Text))
                {
                    FlacEncoderConfig EC = new FlacEncoderConfig();
                    EC.Levels = (int)levelsSlider.Value;
                    ((App)(Application.Current)).LV.Items.Add(new JobItem(In.Text, Out.Text, EncodingType.FLAC, EC, Now.IsChecked == true ? false : true));
                }
                else
                {
                    ModernDialog.ShowMessage("输入输出有不存在项。", "RipStudio Message", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("输入输出有未指定项。", "RipStudio Message", MessageBoxButton.OK);
            }
        }

        private void In_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void In_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void In_PreviewDrop(object sender, DragEventArgs e)
        {
            if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) == ".avs")
            {
                (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (Out.Text == string.Empty)
                {
                    Out.Text = System.IO.Path.GetDirectoryName(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) + @"\" + System.IO.Path.GetFileNameWithoutExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) + ".flac";
                }
            }
            else
            {
                ModernDialog.ShowMessage("拖入的不是所允许的文件。", "RipStudio Message", MessageBoxButton.OK);
            }
        }



        private void InB_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "AviSynth Script|*.avs"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                In.Text = dlg.FileName;
            }
        }

        private void OutB_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Out"; // Default file name
            dlg.DefaultExt = ".flac"; // Default file extension
            dlg.Filter = "FreeLosslessAudioCodec AudioFormat|*.flac"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                Out.Text = dlg.FileName;
            }
        }

        private void levelsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            QualityLevel.Text = "Quality Level - " + ((int)e.NewValue).ToString();
        }
    }
}
