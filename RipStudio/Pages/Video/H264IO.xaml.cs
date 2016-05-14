using AvisynthWrapper;
using FirstFloor.ModernUI.Windows.Controls;
using libavs2x264;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace RipStudio.Pages.Video
{
    /// <summary>
    /// Interaction logic for H264Main.xaml
    /// </summary>
    public partial class H264IO : UserControl
    {
        public H264IO()
        {
            InitializeComponent();
            ((App)(Application.Current)).H264IOPAGE = this;
        }
        private static string[] x264_preset_names = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" };
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(In.Text) && !string.IsNullOrWhiteSpace(Out.Text))
            {
                string config = string.Empty;
                if (EncoderSettings_CB.SelectedIndex == 0)
                {
                    if (((App)(Application.Current)).H264ConfigIsMain == false)
                    {
                        if (((App)(Application.Current)).H264Config_Advanced != null)
                        {
                            config = ((App)(Application.Current)).H264Config_Advanced.EncoderSettings.Text;
                        }
                        else
                        {
                            config += "--crf 23 --preset medium";
                        }
                    }
                    else
                    {
                        if (((App)(Application.Current)).H264Config_Main != null)
                        {
                            switch (((App)(Application.Current)).H264Config_Main.EncodingMode.SelectedIndex)
                            {
                                case 0: config += (" --crf " + (int)(((App)(Application.Current)).H264Config_Main.EncodingValue.Value)); break;
                                case 1: config += (" --bitrate " + (int)(((App)(Application.Current)).H264Config_Main.EncodingValue.Value)); break;
                                case 2: config += (" --qp " + (int)(((App)(Application.Current)).H264Config_Main.EncodingValue.Value)); break;
                            }
                            if (((App)(Application.Current)).H264Config_Main.Tune.SelectedIndex != 0)
                            {
                                config += (" --tune " + ((App)(Application.Current)).H264Config_Main.Tune.Text);
                            }
                            config += (" --preset " + x264_preset_names[(int)(((App)(Application.Current)).H264Config_Main.PresetS.Value)]);
                            if (((App)(Application.Current)).H264Config_Main.Level.SelectedIndex != 0)
                            {
                                config += (" --level " + (((App)(Application.Current)).H264Config_Main.Level.SelectedItem as ComboBoxItem).Tag.ToString());
                            }
                            if (((App)(Application.Current)).H264Config_Main.Profile.SelectedIndex != 0)
                            {
                                config += (" --profile " + ((App)(Application.Current)).H264Config_Main.Profile.Text);
                            }
                        }
                        else
                        {
                            config += "--crf 23 --preset medium";
                        }
                    }

                }
                else
                {
                    StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H264" + EncoderSettings_CB.Text + ".txt", Encoding.Default);
                    config = sr.ReadToEnd();
                    sr.Close();
                }

                ((App)(Application.Current)).LV.Items.Add(new JobItem(In.Text, Out.Text, EncodingType.H264, config, Now.IsChecked == true ? false : true));
            }
            else
            {
                ModernDialog.ShowMessage("输入输出有不存在项。", "RipStudio Message", MessageBoxButton.OK);
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
            if (e.Data.GetData(DataFormats.FileDrop) != null)
            {
                if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) == ".avs")
                {
                    try
                    {
                        if (ModernDialog.ShowMessage("是否预览脚本？", "RipStudio Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            new VideoPlayr(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString(), true).Show();
                            (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                        }
                        else
                        {
                            Avisynth AVS = new Avisynth(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString(), true);
                            AVS.FreeAvisynth();
                            (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "Avisynth Error", MessageBoxButton.OK);
                    }

                }
                else
                {
                    In.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                }
            }
        }


        private void InB_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "AviSynth Script|*.avs|Video File|*.*;";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document

                if (System.IO.Path.GetExtension(dlg.FileName).ToLower() == ".avs")
                {
                    try
                    {
                        if (ModernDialog.ShowMessage("是否预览脚本？", "RipStudio Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            new VideoPlayr(dlg.FileName, true).Show();
                            In.Text = dlg.FileName;
                        }
                        else
                        {
                            Avisynth AVS = new Avisynth(dlg.FileName, true);
                            AVS.FreeAvisynth();
                            In.Text = dlg.FileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "Avisynth Error", MessageBoxButton.OK);
                    }
                }
                else
                {
                    In.Text = dlg.FileName;
                }
            }
        }

        private void OutB_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //if (In.Text != string.Empty && File.Exists(In.Text))
            //{
            //    dlg.FileName = System.IO.Path.GetDirectoryName(In.Text) + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_encoded"; // Default file name
            //}
            if (File.Exists(In.Text))
            {
                dlg.FileName = System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_encoded"; // Default file name
            }
            dlg.DefaultExt = ".264"; // Default file extension
            dlg.Filter = "H.264 format|*.264"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                Out.Text = dlg.FileName;
            }
        }


        private void EncoderSettings_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EncoderSettings_CB.SelectedIndex == 0)
            {
                if (EncoderSettings_Delete != null)
                {
                    EncoderSettings_Delete.IsEnabled = false;
                }
            }
            else
            {
                if (EncoderSettings_Delete != null)
                {
                    EncoderSettings_Delete.IsEnabled = true;
                }
            }
        }

        private void EncoderSettings_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H264\" + EncoderSettings_CB.Text + ".txt"))
                {
                    File.Delete(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H264\" + EncoderSettings_CB.Text + ".txt");
                }
                EncoderSettings_CB.Items.RemoveAt(EncoderSettings_CB.SelectedIndex);
                EncoderSettings_CB.SelectedIndex = 0;
            }
            catch (IOException e2)
            {
                ModernDialog.ShowMessage(e2.Message, "RipStudio Message", MessageBoxButton.OK);
            }
        }

        private void EncoderSettings_CB_Initialized(object sender, EventArgs e)
        {

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H264"))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H264");
            }

            DirectoryInfo TheFolder = new DirectoryInfo(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H264");
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Extension == ".txt" || NextFile.Extension == ".TXT")
                {
                    ComboBoxItem wo = new ComboBoxItem();
                    wo.Content = System.IO.Path.GetFileNameWithoutExtension(NextFile.Name);
                    EncoderSettings_CB.Items.Add(wo);
                }
            }

        }
        private string lsAVS = string.Empty;
        private void In_Loaded(object sender, RoutedEventArgs e)
        {
            if (((App)(Application.Current)).VPresetAVS != string.Empty)
            {
                if (((App)(Application.Current)).VPresetAVS != lsAVS)
                {
                    In.Text = lsAVS = ((App)(Application.Current)).VPresetAVS;
                }
            }
        }

        private void In_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Out.Text == string.Empty)
            {
                if (File.Exists(In.Text))
                {
                    string s = @"\";
                    int Star = 0;
                    int Count = 0;
                    while (Star != -1)
                    {
                        Star = In.Text.IndexOf(s, Star);
                        if (Star != -1)
                        {
                            Count++;
                            Star++;
                        }
                    }
                    if (Count > 1)
                    {
                        Out.Text = System.IO.Path.GetDirectoryName(In.Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_encoded.264";
                    }
                    else
                    {
                        Out.Text = System.IO.Path.GetDirectoryName(In.Text) + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_encoded.264";
                    }
                }
            }
        }
    }
}
