using FirstFloor.ModernUI.Windows.Controls;
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
using libavs2aac;
using AvisynthWrapper;
namespace RipStudio.Pages.Audio
{
    /// <summary>
    /// Interaction logic for AAC.xaml
    /// </summary>
    public partial class AAC : UserControl
    {
        public AAC()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(In.Text) && !string.IsNullOrWhiteSpace(Out.Text))
            {
                //ModernDialog.ShowMessage((Afterburner.IsChecked == true ? 1 : 0).ToString(), "RipStudio Message", MessageBoxButton.OK);
                AacEncoderConfig EC = new AacEncoderConfig();
                EC.Afterburner = Afterburner.IsChecked == true ? 1 : 0;
                if (ConstantBitrate.IsChecked == true)
                {
                    EC.Bitrate = (int)BitrateValue.Value * 1024;
                }
                else
                {
                    EC.VBR = (int)BitrateValue.Value;
                }
                switch (Profile.SelectedIndex)
                {
                    case 0:
                        if (ConstantBitrate.IsChecked == true)
                        {
                            if ((int)BitrateValue.Value < 64)
                            {
                                EC.AOT = 29;
                            }
                            else if ((int)BitrateValue.Value < 128)
                            {
                                EC.AOT = 5;
                            }
                            else
                            {
                                EC.AOT = 2;
                            }
                        }
                        else
                        {
                            if ((int)BitrateValue.Value == 1)
                            {
                                EC.AOT = 29;
                            }
                            else if ((int)BitrateValue.Value == 2)
                            {
                                EC.AOT = 5;
                            }
                            else
                            {
                                EC.AOT = 2;
                            }
                        }
                        break;
                    case 1: EC.AOT = 2; break;
                    case 2: EC.AOT = 5; break;
                    case 3: EC.AOT = 29; break;
                }
                ((App)(Application.Current)).LV.Items.Add(new JobItem(In.Text, Out.Text, EncodingType.AAC, EC, Now.IsChecked == true ? false : true));
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
                if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()).ToLower() == ".avs")
                {
                    try
                    {
                        Avisynth avs = new Avisynth(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString(), true);
                        ScriptInfo woc = avs.GetScriptInfo();
                        if (woc.HasAudio == true)
                        {
                            (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                        }
                        else
                        {
                            ModernDialog.ShowMessage("没有音频轨道！", "Avisynth Error", MessageBoxButton.OK);
                        }
                        avs.FreeAvisynth();
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
            dlg.Filter = "AviSynth Script|*.avs|Audio File|*.*;";

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
                        Avisynth avs = new Avisynth(dlg.FileName, true);
                        ScriptInfo woc = avs.GetScriptInfo();
                        if (woc.HasAudio == true)
                        {
                            In.Text = dlg.FileName;
                        }
                        else
                        {
                            ModernDialog.ShowMessage("没有音频轨道！", "Avisynth Error", MessageBoxButton.OK);
                        }
                        avs.FreeAvisynth();
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
            dlg.FileName = System.IO.Path.GetDirectoryName(In.Text) + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_out";// Default file name
            dlg.DefaultExt = ".aac"; // Default file extension
            dlg.Filter = "AdvancedAudioCoding audio format|*.aac"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                Out.Text = dlg.FileName;
            }
        }

        private void ConstantBitrate_Checked(object sender, RoutedEventArgs e)
        {
            if (BitrateValue != null)
            {
                this.BitrateValue.Minimum = 32;
                this.BitrateValue.Maximum = 384;
                this.BitrateValue.Value = 192;
                VariableBitrate.Content = "Variable Bitrate";
            }
        }

        private void VariableBitrate_Checked(object sender, RoutedEventArgs e)
        {
            if (VariableBitrate != null)
            {
                this.BitrateValue.Minimum = 1;
                this.BitrateValue.Maximum = 5;
                this.BitrateValue.Value = 3;
                ConstantBitrate.Content = "Constant Bitrate";
            }
        }

        private void BitrateValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ConstantBitrate.IsChecked == true)
            {
                ConstantBitrate.Content = "Constant Bitrate-" + ((int)e.NewValue).ToString() + "(Kbps)";
            }
            else
            {
                VariableBitrate.Content = "Variable Bitrate-" + ((int)e.NewValue).ToString() + "(Quality)";
            }
        }
        private string lsAVS = string.Empty;
        private void In_Loaded(object sender, RoutedEventArgs e)
        {
            if (((App)(Application.Current)).APresetAVS != string.Empty)
            {
                if (((App)(Application.Current)).APresetAVS != lsAVS)
                {
                    In.Text = lsAVS = ((App)(Application.Current)).APresetAVS;
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
                        Out.Text = System.IO.Path.GetDirectoryName(In.Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_encoded.aac";
                    }
                    else
                    {
                        Out.Text = System.IO.Path.GetDirectoryName(In.Text) + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_encoded.aac";
                    }
                }
            }
        }
    }
}
