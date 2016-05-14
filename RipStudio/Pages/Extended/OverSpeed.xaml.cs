using FirstFloor.ModernUI.Windows.Controls;
using libavs2aac;
using libmuxer;
using MediaInfoLib;
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

namespace RipStudio.Pages.Extended
{
    /// <summary>
    /// Interaction logic for EasyEncode.xaml
    /// </summary>
    public partial class OverSpeed : UserControl
    {
        public OverSpeed()
        {
            InitializeComponent();
        }

        private void VideoTB_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void VideoTB_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void VideoTB_PreviewDrop(object sender, DragEventArgs e)
        {
            if (((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0) != null)
            {
                string Extension=System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString());
                if (Extension == ".h264" || Extension == ".h265" || Extension == ".mp4" || Extension == ".mkv" || Extension == ".avi" || Extension == ".wmv" || Extension == ".rmvb" || Extension == ".jpg" || Extension == ".jpeg" || Extension == ".png")
                {
                    VideoTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                    //AudioTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();


                    if (Extension == ".jpg" || Extension == ".jpeg" || Extension == ".png" || Extension == ".bmp")
                    {
                        //CompressionRatioCB.IsEnabled = false;
                        //SourceType24CB.IsEnabled = false;
                        BitrateTB.IsEnabled = false;
                        _2pass.IsEnabled = false;
                    }
                    else
                    {
                        //CompressionRatioCB.IsEnabled = true;
                        //SourceType24CB.IsEnabled = true;
                        BitrateTB.IsEnabled = true;
                        _2pass.IsEnabled = true;
                        if (AudioTB.Text == string.Empty)
                        {
                            AudioTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                        }
                    }



                }
                else
                {
                    ModernDialog.ShowMessage("拖入的不是所允许的文件。", "RipStudio Message", MessageBoxButton.OK);
                }
            }
        }
        private void VideoB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "Video Files|*.264;*.265;*.mp4;*.mkv;*.avi;*.wmv;*.rmvb" + "|Image Files|*.jpg;*.jpeg;*.png;*.bmp"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                VideoTB.Text = dlg.FileName;

                if (OutTB.Text == string.Empty)
                {
                    string s = @"\";
                    int Star = 0;
                    int Count = 0;
                    while (Star != -1)
                    {
                        Star = dlg.FileName.IndexOf(s, Star);
                        if (Star != -1)
                        {
                            Count++;
                            Star++;
                        }
                    }
                    //ModernDialog.ShowMessage(Count.ToString(), "RipStudio Message", MessageBoxButton.OK);
                    if (Count > 1)
                    {
                        OutTB.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + @"\" + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_muxed.mkv";
                    }
                    else
                    {
                        OutTB.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_muxed.mkv";
                    }
                }

                if (Path.GetExtension(VideoTB.Text) == ".jpg" || Path.GetExtension(VideoTB.Text) == ".jpeg" || Path.GetExtension(VideoTB.Text) == ".png"||Path.GetExtension(VideoTB.Text) == ".bmp")
                {
                    //CompressionRatioCB.IsEnabled = false;
                    //SourceType24CB.IsEnabled = false;
                    BitrateTB.IsEnabled = false;
                    _2pass.IsEnabled = false;
                }
                else
                {
                    //CompressionRatioCB.IsEnabled = true;
                    //SourceType24CB.IsEnabled = true;
                    BitrateTB.IsEnabled = true;
                    _2pass.IsEnabled = true;
                    if (AudioTB.Text == string.Empty)
                    {
                        AudioTB.Text = dlg.FileName;
                    }
                }



            }
        }

        private void AudioB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "AudioFile|*.*"; 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                AudioTB.Text = dlg.FileName;
            }
        }

        private void SubB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "SubFile|*.ass;*.ssa;*.srt"; // Filter files by extension
            dlg.Multiselect = true;
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                //SubTB.Text = dlg.FileName;
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    if (SubTB.Text != string.Empty)
                    {
                        SubTB.Text += ";" + dlg.FileNames[i];
                    }
                    else
                    {
                        SubTB.Text = dlg.FileNames[i];
                    }
                }
            }
        }

        private void OutB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "MKV File|*.mkv|MP4 File|*.mp4;"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                OutTB.Text = dlg.FileName;
            }
        }

        private void AudioTB_PreviewDrop(object sender, DragEventArgs e)
        {
            if (((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0) != null)
            {
                if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) == ".mp3")
                {
                    VideoTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();


                    if (Path.GetExtension(VideoTB.Text) == ".jpg" || Path.GetExtension(VideoTB.Text) == ".jpeg" || Path.GetExtension(VideoTB.Text) == ".png"||Path.GetExtension(VideoTB.Text) == ".bmp")
                    {
                        //CompressionRatioCB.IsEnabled = false;
                        //SourceType24CB.IsEnabled = false;
                        BitrateTB.IsEnabled = false;
                        _2pass.IsEnabled = false;
                    }
                    else
                    {
                        //CompressionRatioCB.IsEnabled = true;
                        //SourceType24CB.IsEnabled = true;
                        BitrateTB.IsEnabled = true;
                        _2pass.IsEnabled = true;
                        if (AudioTB.Text == string.Empty)
                        {
                            AudioTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                        }
                    }
                }
                else
                {
                    ModernDialog.ShowMessage("拖入的不是所允许的文件。", "RipStudio Message", MessageBoxButton.OK);
                }
            }
        }

        private void SubTB_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                string Extension = "";
                for (int i = 0; i < ((System.Array)e.Data.GetData(DataFormats.FileDrop)).Length; i++)
                {
                    Extension = System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString());
                    if (Extension == ".ass" || Extension == ".ssa" || Extension == ".srt")
                    {
                        if ((sender as TextBox).Text != string.Empty)
                        {
                            (sender as TextBox).Text += (";" + ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString());
                        }
                        else
                        {
                            (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString();
                        }
                    }
                    else
                    {
                        ModernDialog.ShowMessage("拖入的不是所允许的文件。", "RipStudio Message", MessageBoxButton.OK);
                    }
                }
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OutTB.Text != string.Empty)
            {
                //ModernDialog.ShowMessage(GetScript(), "RipStudio Message", MessageBoxButton.OK);
                //ModernDialog.ShowMessage(GetVideoSettings(), "RipStudio Message", MessageBoxButton.OK);
                try
                {
                  EncodingType AET=EncodingType.AAC;
                  //EncodingType VET = EncodingType.H264;
                  MuxerType MT = MuxerType.MP4;
                    switch (AudioCodingCB.SelectedIndex)
                    {
                        case 0: AET=EncodingType.AAC; break;
                        case 1: AET = EncodingType.FLAC; break;
                        case 2: AET = EncodingType.WAV; break;
                    }
                    if (System.IO.Path.GetExtension(OutTB.Text).ToLower() == "mp4")
                    {
                        MT=MuxerType.MP4;
                    }
                    else
                    {
                        MT = MuxerType.MKV;
                    }



                    AutoEncoder ae = new AutoEncoder(GetScript(), OutTB.Text, VideoCodingCB.SelectedIndex == 0 ? EncodingType.H264 : EncodingType.H265, AET,MT, GetVideoSettings(), GetAudioSettings()); 
                    ((App)(Application.Current)).LV.Items.Add(new JobItem(VideoTB.Text, OutTB.Text, EncodingType.AUTO, ae, Now.IsChecked == true ? false : true));
                }
                catch (Exception EX)
                {
                    ModernDialog.ShowMessage(EX.Message , "RipStudio Message", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("没有指定输出文件", "RipStudio Message", MessageBoxButton.OK);
            }


        }
        private static string[] preset_names = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" };
        private string GetScript()
        {
            
            string Script = string.Empty;
            Script += "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n\r\n";
            if (File.Exists(VideoTB.Text))
            {
                if (Path.GetExtension(VideoTB.Text) == ".jpg" || Path.GetExtension(VideoTB.Text) == ".jpeg" || Path.GetExtension(VideoTB.Text) == ".png" || Path.GetExtension(VideoTB.Text) == ".bmp")
                {
                    Script += "LoadPlugin(\"ImageSeq.dll\")\r\n";
                    Script += "LoadPlugin(\"LSMASHSource.dll\")\r\n";
                    Script += "LoadPlugin(\"" + "Shibatch" + ".dll\")\r\n";
                    if (File.Exists(AudioTB.Text))
                    {

                        Script += "Audio = LWLibavAudioSource(\"" + AudioTB.Text + "\")\r\n";
                        if (FrameRate.SelectedIndex == 0)
                        {
                            Script += "Video = ImageReader(\"" + VideoTB.Text + "\", fps=10, start=1, end=ceil(10*AudioLengthF(audio)/AudioRate(audio)))\r\n";
                        }
                        else
                        {
                            Script += "Video = ImageReader(\"" + VideoTB.Text + "\", fps=" + FrameRate.Text + ", start=1, end=ceil(" + FrameRate.Text + "*AudioLengthF(audio)/AudioRate(audio)))\r\n";
                        }

                        Script += "AudioDub(Video, Audio)\r\n";
                        try
                        {
                            if (AudioCodingCB.SelectedIndex == 0)
                            {
                                MediaInfo MI2 = new MediaInfo();
                                MI2.Open(AudioTB.Text);
                                if (int.Parse(MI2.Get(StreamKind.Audio, 0, "SamplingRate")) > 96000)
                                {
                                    Script += "ConvertAudioToFloat()\r\n";
                                    Script += "SSRC(96000)\r\n";
                                    Script += "ConvertAudioTo" + MI2.Get(StreamKind.Audio, 0, "BitDepth") + "bit()\r\n";
                                }
                                MI2.Close();
                            }
                        }
                        catch
                        {

                        }
                            if(ResolutionCB.SelectedIndex!=0)
                            {
                                string[] SArray2 = ResolutionCB.Text.Split('x');
                                Script += "BlackmanResize(" + SArray2[0] + "," + SArray2[1] + ")\r\n"; 
                                //if (SourceType24CB.SelectedIndex != 0)
                                //{
                                //    string[] SArray2 = ResolutionCB.Text.Split('x');
                                //Script += "BilinearResize(" + SArray2[0] + "," + SArray2[1] + ")\r\n"; 
                                //}
                                //else
                                //{
                                //    string[] SArray2 = ResolutionCB.Text.Split('x');
                                //    Script += "LanczosResize(" + SArray2[0] + "," + SArray2[1] + ")\r\n"; 
                                //}
                            }
                        string[] SArray = SubTB.Text.Split(';');
                        if (SArray[0] != string.Empty)
                        {
                            Script += "LoadPlugin(\"VSFilter.dll\")\r\n";
                            foreach (string i in SArray)
                            {
                                Script += "TextSub(\"" + i.ToString() + "\")\r\n";
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("No Audio!");
                    }
                }
                else
                {
                    MediaInfo MI = new MediaInfo();
                    MI.Open(VideoTB.Text);
                    Script += "LoadPlugin(\"LSMASHSource.dll\")\r\n";
                    Script += "LoadPlugin(\"" + "Shibatch" + ".dll\")\r\n";
                    //Script += "LWLibavVideoSource(\"" + VideoTB.Text + "\")\r\n";
                    if (MI.Get(StreamKind.Video, 0, "Format") == "HEVC")
                    {
                        Script += "LWLibavVideoSource(\"" + VideoTB.Text + "\",format=\"" + MI.Get(StreamKind.Video, 0, "ColorSpace") + MI.Get(StreamKind.Video, 0, "ChromaSubsampling").Replace(":", "") + "P" + "8\")\r\n";//MI.Get(StreamKind.Video, 0, "BitDepth") +
                    }
                    else
                    {
                        if (MI.Get(StreamKind.Video, 0, "ColorSpace") == "RGB")
                        {
                            Script += "LWLibavVideoSource(\"" + VideoTB.Text + "\",format=\"RGB24\")\r\n";
                            //Script += "LWLibavVideoSource(source=\"" + VideoTB.Text + "\",dominance=1,format=\"RGB24\")\r\n";
                        }
                        else
                        {
                            Script += "LWLibavVideoSource(\"" + VideoTB.Text + "\")\r\n";
                        }
                    }

                    if (ResolutionCB.SelectedIndex != 0)
                    {
                        string[] SArray2 = ResolutionCB.Text.Split('x');
                        Script += "BlackmanResize(" + SArray2[0] + "," + SArray2[1] + ")\r\n"; 

                    }
                    //Script +="\r\n";

                   if( FrameRate.SelectedIndex!=0)
                   {
                       if (float.Parse(MI.Get(StreamKind.Video, 0, "FrameRate")) / float.Parse(FrameRate.Text) == 2)
                       {
                           Script += "ChangeFPS(" + FrameRate.Text + ")\r\n";
                       }
                       else
                       {
                           Script += "ConvertFPS(" + FrameRate.Text + ")\r\n";
                       }
                   }
                    


                    string[] SArray = SubTB.Text.Split(';');
                    if (SArray[0] != string.Empty)
                    {
                        Script += "LoadPlugin(\"VSFilter.dll\")\r\n";
                        foreach (string i in SArray)
                        {
                            Script += "TextSub(\"" + i.ToString() + "\")\r\n";
                        }
                    }

                    Script += "Video = last\r\n";
                    if (File.Exists(AudioTB.Text))
                    {
                        Script += "Audio=LWLibavAudioSource(\"" + AudioTB.Text + "\")\r\n";
                        Script += "AudioDub(Video, Audio)\r\n";


                        try
                        {
                            if (AudioCodingCB.SelectedIndex == 0)
                            {
                                MediaInfo MI2 = new MediaInfo();
                                MI2.Open(AudioTB.Text);
                                if (int.Parse(MI2.Get(StreamKind.Audio, 0, "SamplingRate")) > 96000)
                                {
                                    Script += "ConvertAudioToFloat()\r\n";
                                    Script += "SSRC(96000)\r\n";
                                    Script += "ConvertAudioTo" + MI2.Get(StreamKind.Audio, 0, "BitDepth") + "bit()\r\n";
                                }
                                MI2.Close();
                            }
                        }
                        catch
                        {
                        
                        }
                    }
                    MI.Close();
                }
            }
            else
            {
                throw new Exception("No Video!");
            }
            return Script;
        }
        private string GetVideoSettings()
        {
            string Settings = string.Empty;
            if (VideoCodingCB.SelectedIndex == 0)
            {
                if (Path.GetExtension(VideoTB.Text) == ".jpg")
                {
                    Settings += " --crf 23 ";
                    Settings += " --profile high";
                    Settings += " --tune stillimage";
                }
                else
                {
                    Settings += " --bitrate " + BitrateTB.Text;
                    Settings += " --preset " + preset_names[9];
                    //Settings += " --preset " + preset_names[CompressionRatioCB.SelectedIndex];
                    //Settings += " --profile high";
                    //if (SourceType24CB.SelectedIndex != 0)
                    //{
                    //    if (SourceType24CB.SelectedIndex == 1)
                    //    {
                    //        Settings += " --tune animation";
                    //    }
                    //    else
                    //    {
                    //        Settings += " --tune film";
                    //    }
                    //}
                    if (_2pass.IsChecked == true)
                    {
                        Settings += " --pass 2";
                    }
                }
            }
            else
            {
                if (Path.GetExtension(VideoTB.Text) == ".jpg")
                {
                    Settings += " --crf 28 ";
                    //Settings += " --profile main";

                }
                else
                {
                    Settings += " --bitrate " + BitrateTB.Text;
                    Settings += " --preset " + preset_names[9];
                    //Settings += " --profile main";
                    if (_2pass.IsChecked == true)
                    {
                        Settings += " --pass 2";
                    }
                }
            }
 
            return Settings;
        }
        private AacEncoderConfig GetAudioSettings()
        {
            AacEncoderConfig EC = new AacEncoderConfig();
            EC.Bitrate = int.Parse(AudioBitrateTB.Text) * 1024;
            EC.Afterburner = 1;
            if (int.Parse(AudioBitrateTB.Text) < 64)
            {
                EC.AOT = 29;
            }
            else if (int.Parse(AudioBitrateTB.Text) < 128)
            {
                EC.AOT = 5;
            }
            else
            {
                EC.AOT = 2;
            }
            return EC;
        }

        private void AudioCodingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AudioBitrateTB != null && AudioBitrateTBTB != null)
            if ((sender as ComboBox).SelectedIndex == 0)
            {
                AudioBitrateTB.Visibility = AudioBitrateTBTB.Visibility = Visibility.Visible;
            }
            else
            {
                AudioBitrateTB.Visibility = AudioBitrateTBTB.Visibility = Visibility.Hidden;
            }

        }


    }
}
