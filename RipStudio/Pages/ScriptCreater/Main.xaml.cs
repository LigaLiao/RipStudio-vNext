using FirstFloor.ModernUI.Windows.Controls;
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
using System.Windows.Shapes;

namespace RipStudio.Pages.ScriptCreater
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();
            ((App)(Application.Current)).avs_Main = this;
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
            VideoTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (AudioTB.Text == string.Empty)
            {
                if (string.IsNullOrWhiteSpace(AudioTB.Text))
                {
                    MediaInfo MI = new MediaInfo();
                    MI.Open(VideoTB.Text);

                    if (MI.Count_Get(StreamKind.Audio) > 0)
                    {
                        AudioTB.Text = VideoTB.Text;
                    }
                    else
                    {
                        string af = "";
                        string s = @"\";
                        int Star = 0;
                        int Count = 0;
                        while (Star != -1)
                        {
                            Star = VideoTB.Text.IndexOf(s, Star);
                            if (Star != -1)
                            {
                                Count++;
                                Star++;
                            }
                        }
                        if (Count > 1)
                        {
                            af = System.IO.Path.GetDirectoryName(VideoTB.Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".flac";
                        }
                        else
                        {
                            af = System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".flac";
                        }


                        if (File.Exists(af))
                        {
                            AudioTB.Text = af;
                        }
                    }

                    MI.Close();
                }
            }
        }

        private void SaveScript_Click(object sender, RoutedEventArgs e)
        {
            if ( !String.IsNullOrWhiteSpace(VideoTB.Text)|| !String.IsNullOrWhiteSpace(AudioTB.Text))
            {
                string Script = GetScript();
                if (Script != null)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(OutputTB.Text, false, Encoding.Default);
                        sw.WriteLine(Script);
                        sw.Close();
                        if (IsLoadingEncoder.IsChecked == true)
                        {
                            if (!String.IsNullOrWhiteSpace(VideoTB.Text))
                            {
                                ((App)(Application.Current)).VPresetAVS = OutputTB.Text;
                            }
                            if (!String.IsNullOrWhiteSpace(AudioTB.Text))
                            {
                                ((App)(Application.Current)).APresetAVS = OutputTB.Text;
                            }
                        }
                        //ModernDialog.ShowMessage("保存成功！", "RipStudio Message", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "AviSynth Error", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                ModernDialog.ShowMessage("没有载入任何一个视频或者音频文件！", "RipStudio Message", MessageBoxButton.OK);
            }
        }
        private string GetScript()
        {
            MediaInfo MI = new MediaInfo();
            MI.Open(VideoTB.Text);
            string avs脚本 = "";
            string avs脚本插件 = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n\r\n";
            if (!string.IsNullOrWhiteSpace(VideoTB.Text))
            {

                if (((App)(Application.Current)).VideoMain == null)
                {
                    avs脚本插件 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                    //MessageBox.Show(MI.Get(StreamKind.Video, 0, "BitDepth"));
                    if (MI.Get(StreamKind.Video, 0, "Format") == "HEVC")
                    {
                        avs脚本 += "LWLibavVideoSource(\"" + VideoTB.Text + "\",format=\"" + MI.Get(StreamKind.Video, 0, "ColorSpace") + MI.Get(StreamKind.Video, 0, "ChromaSubsampling").Replace(":", "") + "P" + "8\")\r\n";//MI.Get(StreamKind.Video, 0, "BitDepth") +
                    }
                    else
                    {
                        if(MI.Get(StreamKind.Video, 0, "ColorSpace")=="RGB")
                        {
                            avs脚本 += "LWLibavVideoSource(\"" + VideoTB.Text + "\",format=\"RGB24\")\r\n";
                        //avs脚本 += "LWLibavVideoSource(source=\"" + VideoTB.Text + "\",dominance=1,format=\"RGB24\")\r\n";
                        }
                        else
                        {
                        avs脚本 += "LWLibavVideoSource(\"" + VideoTB.Text + "\")\r\n";
                        }
                    }       
                }
                else
                {
                    avs脚本插件 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";


                    if (MI.Get(StreamKind.Video, 0, "Format") == "HEVC")
                    {
                        avs脚本 += "LWLibavVideoSource(\"" + VideoTB.Text + "\",format=\"" + MI.Get(StreamKind.Video, 0, "ColorSpace") + MI.Get(StreamKind.Video, 0, "ChromaSubsampling").Replace(":", "") + "P" + "8\")\r\n";//MI.Get(StreamKind.Video, 0, "BitDepth") +
                    }
                    else
                    {
                        if (MI.Get(StreamKind.Video, 0, "ColorSpace") == "RGB")
                        {
                            avs脚本 += "LWLibavVideoSource(\"" + VideoTB.Text + "\",format=\"RGB24\")\r\n";
                            //avs脚本 += "LWLibavVideoSource(source=\"" + VideoTB.Text + "\",dominance=1,format=\"RGB24\")\r\n";
                        }
                        else
                        {
                            avs脚本 += "LWLibavVideoSource(\"" + VideoTB.Text + "\")\r\n";
                        }
                    }
                    if (((App)(Application.Current)).VideoMain.Crop.IsChecked == true)
                    {
                        avs脚本 += "crop(" + ((App)(Application.Current)).VideoMain.Crop1.Text + "," + ((App)(Application.Current)).VideoMain.Crop1.Text + ",-" + ((App)(Application.Current)).VideoMain.Crop3.Text + ",-" + ((App)(Application.Current)).VideoMain.Crop4.Text + ")\r\n";
                    }
                    if (((App)(Application.Current)).VideoMain.AddBorder.IsChecked == true)
                    {
                        avs脚本 += "AddBorders(" + ((App)(Application.Current)).VideoMain.Border1.Text + "," + ((App)(Application.Current)).VideoMain.Border1.Text + ",-" + ((App)(Application.Current)).VideoMain.Border3.Text + ",-" + ((App)(Application.Current)).VideoMain.Border4.Text + ")\r\n";
                    }
                    if (((App)(Application.Current)).VideoMain.Resolution.SelectedIndex!=0)
                    {
                        string[] SArray = ((App)(Application.Current)).VideoMain.Resolution.Text.ToLower().Split('x');
                        avs脚本 += "BlackmanResize(" + SArray[0] + "," + SArray[1] + ")\r\n";
                    }
                    if (((App)(Application.Current)).VideoMain.FPS.SelectedIndex != 0)
                    {
                        if (float.Parse(MI.Get(StreamKind.Video, 0, "FrameRate")) / float.Parse(((App)(Application.Current)).VideoMain.FPS.Text) == 2)
                        {
                            avs脚本 += "ChangeFPS(" + ((App)(Application.Current)).VideoMain.FPS.Text + ")\r\n";
                        }
                        else
                        {
                            avs脚本 += "ConvertFPS(" + ((App)(Application.Current)).VideoMain.FPS.Text + ")\r\n";
                        }
                    }

                    if (float.Parse(((App)(Application.Current)).VideoMain.色度TB.Text) != 0 || float.Parse(((App)(Application.Current)).VideoMain.饱和度TB.Text) != 1 || float.Parse(((App)(Application.Current)).VideoMain.亮度TB.Text) != 0 || float.Parse(((App)(Application.Current)).VideoMain.对比度TB.Text) != 1)
                    {
                        if (MI.Get(StreamKind.Video, 0, "ColorSpace") == "RGB")
                        {
                            avs脚本 += "ConvertToYV24()\r\n";
                            avs脚本 += "Tweak(" + ((App)(Application.Current)).VideoMain.色度TB.Text + "," + ((App)(Application.Current)).VideoMain.饱和度TB.Text + "," + ((App)(Application.Current)).VideoMain.亮度TB.Text + "," + ((App)(Application.Current)).VideoMain.对比度TB.Text + ")\r\n";

                            if (((App)(Application.Current)).VideoMain.ColorSpace.SelectedIndex == 0)
                            {
                                avs脚本 += "ConvertToRGB24()\r\n";
                            }
                            else
                            {
                                avs脚本 += "ConvertTo" + ((App)(Application.Current)).VideoMain.ColorSpace.Text + "()\r\n";
                            }
                        }
                        else
                        {
                            avs脚本 += "Tweak(" + ((App)(Application.Current)).VideoMain.色度TB.Text + "," + ((App)(Application.Current)).VideoMain.饱和度TB.Text + "," + ((App)(Application.Current)).VideoMain.亮度TB.Text + "," + ((App)(Application.Current)).VideoMain.对比度TB.Text + ")\r\n";
                            if (((App)(Application.Current)).VideoMain.ColorSpace.SelectedIndex != 0)
                            {
                                avs脚本 += "ConvertTo" + ((App)(Application.Current)).VideoMain.ColorSpace.Text + "()\r\n";
                            }
                        }
                    }
                    else
                    {
                        if (((App)(Application.Current)).VideoMain.ColorSpace.SelectedIndex != 0)
                        {
                            avs脚本 += "ConvertTo" + ((App)(Application.Current)).VideoMain.ColorSpace.Text + "()\r\n";
                        }
                    }


                    if (((App)(Application.Current)).VideoMain.降噪.IsChecked == true)
                    {
                        avs脚本插件 += "LoadPlugin(\"" + "DeNoise" + ".dll\")\r\n";
                        //avs脚本 += "Denoise()\r\n";
                        //avs脚本 += "DeNoise(clip=false,lx=400,rx=410,ty=500,by=510, show = false)\r\n";
                        
                    }
                    if (((App)(Application.Current)).VideoMain.柔化.IsChecked == true)
                    {
                        avs脚本 += "Blur(0.6)\r\n";
                    }
                    if (((App)(Application.Current)).VideoMain.锐化.IsChecked == true)
                    {
                        avs脚本 += "Sharpen(1)\r\n";
                    }

                }
                #region Subtitle
                if (File.Exists(VideoTB.Text))
                {
                    if (((App)(Application.Current)).VideoSubtitle != null && ((App)(Application.Current)).VideoSubtitle.DG1.Items.Count > 0)
                    {
                        if (((App)(Application.Current)).VideoSubtitle.SubtitleFilter.SelectedIndex == 0)
                        {
                            avs脚本插件 += "LoadPlugin(\"VSFilter.dll\")\r\n";
                        }
                        else
                        {
                            avs脚本插件 += "LoadPlugin(\"VSFilterMod64.dll\")\r\n";
                        }

                        if (((App)(Application.Current)).VideoSubtitle.DG1.Items.Count == 2)
                        {
                            if (File.Exists((((App)(Application.Current)).VideoSubtitle.DG1.Items.GetItemAt(0) as Customer3).File))
                            {
                                avs脚本 += "TextSub(\"" + (((App)(Application.Current)).VideoSubtitle.DG1.Items.GetItemAt(0) as Customer3).File + "\")\r\n";
                            }
                        }
                        else if (((App)(Application.Current)).VideoSubtitle.DG1.Items.Count > 2)
                        {
                            for (int i = 0; i < ((App)(Application.Current)).VideoSubtitle.DG1.Items.Count - 1; i++)
                            {
                                if (File.Exists((((App)(Application.Current)).VideoSubtitle.DG1.Items.GetItemAt(i) as Customer3).File))
                                {
                                    avs脚本 += "TextSub(\"" + (((App)(Application.Current)).VideoSubtitle.DG1.Items.GetItemAt(i) as Customer3).File + "\")\r\n";
                                }
                            }
                        }
                        if (AutoLoadSub.IsChecked == true)
                        {
                            if (File.Exists(System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".ass"))
                            {
                                if (((App)(Application.Current)).VideoSubtitle != null)
                                {
                                    avs脚本插件 += "LoadPlugin(\"" + ((App)(Application.Current)).VideoSubtitle.SubtitleFilter.Text + ".dll\")\r\n";
                                }
                                else
                                {
                                    avs脚本插件 += "LoadPlugin(\"" + "VSFilter" + ".dll\")\r\n";
                                }
                                avs脚本 += "TextSub(\"" + System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".ass" + "\")\r\n";
                            }
                        }
                    }
                    else
                    {
                        if (AutoLoadSub.IsChecked == true)
                        {

                            if (File.Exists(System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".ass"))
                            {
                                if (((App)(Application.Current)).VideoSubtitle != null)
                                {
                                    avs脚本插件 += "LoadPlugin(\"" + ((App)(Application.Current)).VideoSubtitle.SubtitleFilter.Text + ".dll\")\r\n";
                                }
                                else
                                {
                                    avs脚本插件 += "LoadPlugin(\"" + "VSFilter" + ".dll\")\r\n";
                                }
                                avs脚本 += "TextSub(\"" + System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".ass" + "\")\r\n";
                            }
                        }
                    }
                }

                #endregion
                #region Other
                if (((App)(Application.Current)).VideoOther != null)
                {
                    if (((App)(Application.Current)).VideoOther.扫描线效果.IsChecked == true)
                    {
                        avs脚本 += "Scanlines(" + 4 + ")\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.黑白效果.IsChecked == true)
                    {
                        avs脚本 += "GreyScale()\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.倒转.IsChecked == true)
                    {
                        avs脚本 += "FlipVertical()\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.翻转.IsChecked == true)
                    {
                        avs脚本 += "FlipHorizontal()\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.淡入.IsChecked == true && int.Parse(((App)(Application.Current)).VideoOther.淡入帧.Text) != 0)
                    {
                        avs脚本 += "FadeIn(" + ((App)(Application.Current)).VideoOther.淡入帧.Text + ")\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.淡出.IsChecked == true && int.Parse(((App)(Application.Current)).VideoOther.淡出帧.Text) != 0)
                    {
                        avs脚本 += "FadeOut(" + ((App)(Application.Current)).VideoOther.淡出帧.Text + ")\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.显示视频信息.IsChecked == true)
                    {
                        avs脚本 += "info()\r\n";
                    }
                    if (((App)(Application.Current)).VideoOther.画面旋转.IsChecked == true)
                    {
                        if (((App)(Application.Current)).VideoOther.逆转90度.IsChecked == true)
                        {
                            avs脚本 += "TurnLeft()\r\n";
                        }
                        else if (((App)(Application.Current)).VideoOther.旋转180度.IsChecked == true)
                        {
                            avs脚本 += "Turn180()\r\n";
                        }
                        else if (((App)(Application.Current)).VideoOther.顺转90度.IsChecked == true)
                        {
                            avs脚本 += "TurnRight()\r\n";
                        }
                    }
                }
                #endregion
            }

            if (!string.IsNullOrWhiteSpace(AudioTB.Text))
            {
                if (!string.IsNullOrWhiteSpace(VideoTB.Text))
                {
                    avs脚本 += "Video = last\r\n";
                }

                if (((App)(Application.Current)).AudioMain != null)
                {
                    if (!avs脚本插件.Contains("LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n"))
                    {
                        avs脚本插件 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                    }


                    if (!string.IsNullOrWhiteSpace(VideoTB.Text))
                    {
                        avs脚本 += "Audio = LWLibavAudioSource" + "(\"" + AudioTB.Text + "\")\r\n";
                        avs脚本 += "AudioDub(Video, Audio)\r\n";
                    }
                    else
                    {
                        avs脚本 += "LWLibavAudioSource" + "(\"" + AudioTB.Text + "\")\r\n";
                    }



                    if (((App)(Application.Current)).AudioMain.BitsPerSampleCB.SelectedIndex == 0)
                    {
                        
                    }
                    else if (((App)(Application.Current)).AudioMain.BitsPerSampleCB.SelectedIndex == 5)
                    {
                        avs脚本 += "ConvertAudioToFloat()\r\n";
                    }
                    else
                    {
                        avs脚本 += "ConvertAudioTo" + ((App)(Application.Current)).AudioMain.BitsPerSampleCB.Text + "bit()\r\n";
                    }

                    if (((App)(Application.Current)).AudioMain.SampleRateCB.SelectedIndex != 0)
                    {
                        if (((App)(Application.Current)).AudioMain.BitsPerSampleCB.SelectedIndex == 2)
                        {
                            avs脚本 += "ResampleAudio (" + ((App)(Application.Current)).AudioMain.SampleRateCB.Text + ")\r\n";
                        }
                        else if ((((App)(Application.Current)).AudioMain.BitsPerSampleCB.SelectedIndex == 5))
                        {
                            avs脚本插件 += "LoadPlugin(\"" + "Shibatch" + ".dll\")\r\n";
                            avs脚本 += "SSRC(" + ((App)(Application.Current)).AudioMain.SampleRateCB.Text + ")\r\n";
                        }
                        else
                        {
                            try
                            {
                                int.Parse(MI.Get(StreamKind.Audio, 0, "BitDepth"));
                                avs脚本插件 += "LoadPlugin(\"" + "Shibatch" + ".dll\")\r\n";
                                avs脚本 += "ConvertAudioToFloat()\r\n";
                                avs脚本 += "SSRC(" + ((App)(Application.Current)).AudioMain.SampleRateCB.Text + ")\r\n";
                                avs脚本 += "ConvertAudioTo" + MI.Get(StreamKind.Audio, 0, "BitDepth") + "bit()\r\n";
                            }
                            catch
                            {
                                avs脚本插件 += "LoadPlugin(\"" + "Shibatch" + ".dll\")\r\n";
                                avs脚本 += "ConvertAudioToFloat()\r\n";
                                avs脚本 += "SSRC(" + ((App)(Application.Current)).AudioMain.SampleRateCB.Text + ")\r\n";
                            }
                        }
                    }

                    if (int.Parse(((App)(Application.Current)).AudioMain.NormalizeTB.Text) != 100)
                    {
                        avs脚本 += "Normalize(" + ((float)(int.Parse(((App)(Application.Current)).AudioMain.NormalizeTB.Text)) / 100.00).ToString() + ")\r\n";
                    }
                    if (float.Parse(((App)(Application.Current)).AudioMain.Delay.Text) != 0)
                    {
                        avs脚本 += "DelayAudio(" + ((App)(Application.Current)).AudioMain.Delay.Text + ".0/1000.0)\r\n";
                    }
                }
                else
                {
                    if (!avs脚本插件.Contains("LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n"))
                    {
                        avs脚本插件 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                    }

                    if (!string.IsNullOrWhiteSpace(VideoTB.Text))
                    {
                        avs脚本 += "Audio = LWLibavAudioSource(\"" + AudioTB.Text + "\")\r\n";
                        avs脚本 += "AudioDub(Video, Audio)\r\n";
                    }
                    else
                    {
                        avs脚本 += "LWLibavAudioSource(\"" + AudioTB.Text + "\")\r\n";
                    }

                }
            }




            if (((App)(Application.Current)).avs_Cut != null)
            {
                if (string.IsNullOrWhiteSpace(VideoTB.Text) && !string.IsNullOrWhiteSpace(AudioTB.Text))
                {
                    if (((App)(Application.Current)).avs_Cut.DG1.Items.Count == 2)
                    {
                        avs脚本 += "Audio.AudioTrim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(0) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(0) as Customer).EndFrame + ")\r\n";

                    }
                    else if (((App)(Application.Current)).avs_Cut.DG1.Items.Count > 2)
                    {
                        avs脚本 += "Audio.";
                        for (int i = 0; i < ((App)(Application.Current)).avs_Cut.DG1.Items.Count - 1; i++)
                        {
                            if (i > 0)
                            {
                                avs脚本 += "+AudioTrim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).EndFrame + ")";
                            }
                            else if (i + 1 == ((App)(Application.Current)).avs_Cut.DG1.Items.Count - 1)
                            {
                                avs脚本 += "+AudioTrim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).EndFrame + ")\r\n";
                            }

                            else
                            {
                                avs脚本 += "AudioTrim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).EndFrame + ")";
                            }
                        }
                    }
                }
                else
                {
                    if (((App)(Application.Current)).avs_Cut.DG1.Items.Count == 2)
                    {
                        avs脚本 += "Trim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(0) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(0) as Customer).EndFrame + ")\r\n";

                    }
                    else if (((App)(Application.Current)).avs_Cut.DG1.Items.Count > 2)
                    {
                        for (int i = 0; i < ((App)(Application.Current)).avs_Cut.DG1.Items.Count - 1; i++)
                        {
                            if (i > 0)
                            {
                                avs脚本 += "+Trim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).EndFrame + ")";
                            }
                            else if (i + 1 == ((App)(Application.Current)).avs_Cut.DG1.Items.Count - 1)
                            {
                                avs脚本 += "+Trim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).EndFrame + ")\r\n";
                            }
                            else
                            {
                                avs脚本 += "Trim(" + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).StartFrame + "," + (((App)(Application.Current)).avs_Cut.DG1.Items.GetItemAt(i) as Customer).EndFrame + ")";
                            }
                        }
                    }
                }
            }
            MI.Close();
            return avs脚本插件 + "\r\n" + avs脚本;
        }



        private void VideoB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "VideoFile|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                VideoTB.Text = dlg.FileName;
                if (string.IsNullOrWhiteSpace(AudioTB.Text))
                {
                    MediaInfo MI = new MediaInfo();
                    MI.Open(dlg.FileName);

                    if (MI.Count_Get(StreamKind.Audio) > 0)
                    {
                        AudioTB.Text = dlg.FileName;
                    }
                    else
                    {
                        string af = "";
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
                        if (Count > 1)
                        {
                            af = System.IO.Path.GetDirectoryName(dlg.FileName) + @"\" + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + ".flac";
                        }
                        else
                        {
                            af = System.IO.Path.GetDirectoryName(dlg.FileName) + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + ".flac";
                        }


                        if (File.Exists(af))
                        {
                            AudioTB.Text = af;
                        }
                    }

                    MI.Close();
                }
            }
        }

        private void AudioB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "AudioFile|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                AudioTB.Text = dlg.FileName;
            }
        }

        private void CheckScript_Click(object sender, RoutedEventArgs e)
        {
            if (VideoTB.Text != String.Empty || AudioTB.Text != String.Empty)
            {
                new CheckScriptWindow(GetScript(), OutputTB.Text).ShowDialog();
            }
            else
            {
                ModernDialog.ShowMessage("你没有载入任何一个视频或者音频文件。", "RipStudio Message", MessageBoxButton.OK);
            }
        }
        private void PreviewScript_Click(object sender, RoutedEventArgs e)
        {
            if (VideoTB.Text != String.Empty || AudioTB.Text != String.Empty)
            {
                try
                {
                    string Script = GetScript();
                    if (Script != null)
                    {
                        if (((App)(Application.Current)).vp == null)
                        {
                            ((App)(Application.Current)).vp = new VideoPlayr(Script, false);
                            ((App)(Application.Current)).vp.Show();
                        }
                        else
                        {
                            ((App)(Application.Current)).vp.Close();
                            ((App)(Application.Current)).vp = new VideoPlayr(Script, false);
                            ((App)(Application.Current)).vp.Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "AviSynth Error", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("你没有载入任何一个视频或者音频文件！", "RipStudio Message", MessageBoxButton.OK);
            }
        }

        private void OutputB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "AviSynth Script|*.avs"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                OutputTB.Text = dlg.FileName;
            }
        }

        private void VideoTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OutputTB.Text))
            {
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = VideoTB.Text.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    OutputTB.Text = System.IO.Path.GetDirectoryName(VideoTB.Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".avs";
                }
                else
                {
                    OutputTB.Text = System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".avs";
                }
            }
        }

        private void AudioTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OutputTB.Text) && string.IsNullOrWhiteSpace(VideoTB.Text))
            {
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = AudioTB.Text.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    OutputTB.Text = System.IO.Path.GetDirectoryName(AudioTB.Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension(AudioTB.Text) + ".avs";
                }
                else
                {
                    OutputTB.Text = System.IO.Path.GetDirectoryName(AudioTB.Text) + System.IO.Path.GetFileNameWithoutExtension(AudioTB.Text) + ".avs";
                }
            }
        }

        private void AudioTB_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void AudioTB_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void AudioTB_PreviewDrop(object sender, DragEventArgs e)
        {
            ((TextBox)sender).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

    }
}