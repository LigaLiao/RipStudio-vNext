using FirstFloor.ModernUI.Windows.Controls;
using libavs2aac;
using libavs2flac;
using libavs2wav;
using libavs2x264;
using libavs2x265;
using MediaInfoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

namespace RipStudio
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class JobItem : ListViewItem
    {
        public bool StateBool = false;
        private BackgroundWorker backgroundWorker;
        private string filein = "";
        private string fileout = "";
        private EncodingType encodingType;
        private object jobConfig;
        private System.Diagnostics.Stopwatch sw=new System.Diagnostics.Stopwatch();

        private WavEncoder wavEncoder=null;
        private FlacEncoder flacEncoder = null;
        private AacEncoder aacEncoder = null;
        private X264Encoder x264Encoder = null;
        private X265Encoder x265Encoder = null;


        public JobItem(string File_In, string File_Out, EncodingType Type, object config,bool IsWait)
        {
            InitializeComponent();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            filein = InTB.Text = File_In;
            fileout = OutTB.Text = File_Out;
            encodingType = Type;
            jobConfig = config;

            switch(encodingType)
            {
                case EncodingType.H264:
                case EncodingType.H265:
                     Speed.Text = "码率/速度/已用时间/剩余时间";
                     Speed.ToolTip = "码率/速度/已用时间/剩余时间";
                break;
                case EncodingType.AAC:
                case EncodingType.FLAC:
                case EncodingType.WAV:
                case EncodingType.AUTO:
                       Speed.Text = "速度/已用时间/剩余时间";
                       Speed.ToolTip = "速度/已用时间/剩余时间";
                break;
                case EncodingType.MP4:
                case EncodingType.MKV:
                     Speed.Text = "已用时间";
                     Speed.ToolTip = "已用时间";
                break;
            }

            IOSP.ToolTip = "输入文件：" + File_In + "\r\n输出文件：" + File_Out;
            if (IsWait == false)
            {
                State.Text = "处理中";
                StateBool = true;
                backgroundWorker.RunWorkerAsync(config);
            }
            TypeTB.Text = Type.ToString();
        }


        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            sw.Start();
            try
            {
                switch (encodingType)
                {
                    case EncodingType.WAV:
                        if (System.IO.Path.GetExtension(filein).ToLower() != ".avs")
                        {
                            string avs = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n" + "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                            avs += "LWLibavAudioSource(\"" + filein + "\")\r\n";
                            wavEncoder = new WavEncoder(avs, false);
                        }
                        else
                        {
                            wavEncoder = new WavEncoder(filein, true);
                        }
                        e.Result = wavEncoder.Start(fileout, (BackgroundWorker)sender);
                        break;
                    case EncodingType.FLAC:
                        if (System.IO.Path.GetExtension(filein).ToLower() != ".avs")
                        {
                            string avs = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n" + "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                            avs += "LWLibavAudioSource(\"" + filein + "\")\r\n";
                            flacEncoder = new FlacEncoder(avs, false);
                        }
                        else
                        {
                            flacEncoder = new FlacEncoder(filein, true);
                        }
                        //flacEncoder = new FlacEncoder(filein, isfile);
                        e.Result = flacEncoder.Start(fileout, (BackgroundWorker)sender, (FlacEncoderConfig)e.Argument);
                        break;
                    case EncodingType.AAC:
                        if (System.IO.Path.GetExtension(filein).ToLower() != ".avs")
                        {
                            string avs = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n" + "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                            avs += "LWLibavAudioSource(\"" + filein + "\")\r\n";
                            aacEncoder = new AacEncoder(avs, false);
                        }
                        else
                        {
                            aacEncoder = new AacEncoder(filein, true);
                        }
                        e.Result = aacEncoder.Start(fileout, (BackgroundWorker)sender, (AacEncoderConfig)e.Argument);
                        break;
                    case EncodingType.H264:
                        if (System.IO.Path.GetExtension(filein).ToLower() != ".avs")
                        {
                            MediaInfo MI = new MediaInfo();
                            MI.Open(filein);
                            string avs = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n" + "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                            if (MI.Get(StreamKind.Video, 0, "Format") == "HEVC")
                            {
                                avs += "LWLibavVideoSource(\"" + filein + "\",format=\"" + MI.Get(StreamKind.Video, 0, "ColorSpace") + MI.Get(StreamKind.Video, 0, "ChromaSubsampling").Replace(":", "") + "P" + "8\")\r\n";//MI.Get(StreamKind.Video, 0, "BitDepth") +
                            }
                            else
                            {
                                if (MI.Get(StreamKind.Video, 0, "ColorSpace") == "RGB")
                                {
                                    avs += "LWLibavVideoSource(\"" + filein + "\",format=\"RGB24\")\r\n";
                                }
                                else
                                {
                                    avs += "LWLibavVideoSource(\"" + filein + "\")\r\n";
                                }
                            }
                            MI.Close();
                            x264Encoder = new X264Encoder(avs, false);
                        }
                        else
                        {
                            x264Encoder = new X264Encoder(filein, true);
                        }
                        
                        e.Result = x264Encoder.Start(fileout, (BackgroundWorker)sender, (string)e.Argument);
                        break;
                    case EncodingType.H265:
                        if (System.IO.Path.GetExtension(filein).ToLower() != ".avs")
                        {
                            MediaInfo MI = new MediaInfo();
                            MI.Open(filein);
                            string avs = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n" + "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                            if (MI.Get(StreamKind.Video, 0, "Format") == "HEVC")
                            {
                                avs += "LWLibavVideoSource(\"" + filein + "\",format=\"" + MI.Get(StreamKind.Video, 0, "ColorSpace") + MI.Get(StreamKind.Video, 0, "ChromaSubsampling").Replace(":", "") + "P" + "8\")\r\n";//MI.Get(StreamKind.Video, 0, "BitDepth") +
                            }
                            else
                            {
                                if (MI.Get(StreamKind.Video, 0, "ColorSpace") == "RGB")
                                {
                                    avs += "LWLibavVideoSource(\"" + filein + "\",format=\"RGB24\")\r\n";
                                }
                                else
                                {
                                    avs += "LWLibavVideoSource(\"" + filein + "\")\r\n";
                                }
                            }
                            MI.Close();
                            x265Encoder = new X265Encoder(avs, false);
                        }
                        else
                        {
                            x265Encoder = new X265Encoder(filein, true);
                        }
                        e.Result = x265Encoder.Start(fileout, (BackgroundWorker)sender, (string)e.Argument);
                        break;
                    case EncodingType.MP4:
                    case EncodingType.MKV:
                        e.Result = ((libmuxer.Muxer)e.Argument).Start(fileout, (BackgroundWorker)sender);
                        break;
                    case EncodingType.AUTO:
                        e.Result = ((AutoEncoder)e.Argument).Start((BackgroundWorker)sender);
                        break;
                }
            }
            catch (Exception ex)
            {
                (sender as BackgroundWorker).ReportProgress(0);
                e.Result = ex.Message;
            }
        }
        //double db=0;
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (State.Text != "进行中")
            {
                State.Text = "进行中";
            }
            PB.Value = e.ProgressPercentage;
            if (e.UserState != null)
            {
                switch (encodingType)
                {
                    case EncodingType.H264:
                    case EncodingType.H265:
                        Speed.Text = (!double.IsNaN(((double[])e.UserState)[2]) ? ((double[])e.UserState)[2].ToString("0.00") : "0") + @"/" + ((double[])e.UserState)[0].ToString("0.00") + "FPS / " + sw.Elapsed.ToString(@"hh\:mm\:ss") + @" / " + new TimeSpan(0, 0, ((double[])e.UserState)[1] == double.PositiveInfinity ? 0 : ((int)(((double[])e.UserState)[1]))).ToString(@"hh\:mm\:ss");
                        break;
                    case EncodingType.AAC:
                    case EncodingType.FLAC:
                    case EncodingType.WAV:
                        Speed.Text = ((double[])e.UserState)[0].ToString("0.00") + "X / " + sw.Elapsed.ToString(@"hh\:mm\:ss") + " / " + new TimeSpan(0, 0, (int)(((double[])e.UserState)[1]) - int.Parse(sw.Elapsed.ToString(@"ss"))).ToString(@"hh\:mm\:ss");
                        //Speed.Text = ((double[])e.UserState)[2].ToString();
                        break;
                    case EncodingType.MP4:
                    case EncodingType.MKV:
                        Speed.Text = sw.Elapsed.ToString(@"hh\:mm\:ss");
                        break;
                    case EncodingType.AUTO:
                        if (e.UserState is double[])
                        {
                            if (((double[])e.UserState).Length == 3)
                            {
                                Speed.Text = "视频编码中 - " + ((double[])e.UserState)[0].ToString("0.00") + "FPS / " + sw.Elapsed.ToString(@"hh\:mm\:ss") + @" / " + new TimeSpan(0, 0, ((double[])e.UserState)[1] == double.PositiveInfinity ? 0 : ((int)(((double[])e.UserState)[1]))).ToString(@"hh\:mm\:ss");
                            }
                            else
                            {
                                Speed.Text ="音频编码中 - "+ ((double[])e.UserState)[0].ToString("0.00") + "X / " + sw.Elapsed.ToString(@"hh\:mm\:ss") + " / " + new TimeSpan(0, 0, (int)(((double[])e.UserState)[1]) - int.Parse(sw.Elapsed.ToString(@"ss"))).ToString(@"hh\:mm\:ss");
                            }
                        }
                        else
                        {
                            Speed.Text = "混流中 - " + sw.Elapsed.ToString(@"hh\:mm\:ss");
                        }
                        break;
                }
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is string && (e.Result as string) == "END")
            {
                Speed.Text = "耗时：" + sw.Elapsed.ToString(@"hh\:mm\:ss");
                State.Text = "完成";
                PB.Value = 100;
            }
            else if (e.Result is string && (e.Result as string)=="Cancel")
            {
                 StateBool = false;
                State.Text = "停止";
                PB.Value = 0;
                switch (encodingType)
                {
                    case EncodingType.H264:
                    case EncodingType.H265:
                        Speed.Text = "码率/速度/已用时间/剩余时间";
                        break;
                    case EncodingType.AAC:
                    case EncodingType.FLAC:
                    case EncodingType.WAV:
                    case EncodingType.AUTO:
                        Speed.Text = "速度/已用时间/剩余时间";
                        break;
                    case EncodingType.MP4:
                    case EncodingType.MKV:
                        Speed.Text = "已用时间";
                        break;
                }
                //ModernDialog.ShowMessage("已停止任务", "RipStudio Message", MessageBoxButton.OK);
            }
            else if (e.Error != null)
            {
                State.Text = "出错";
                StateBool = false;
                PB.Value = 0;
                switch (encodingType)
                {
                    case EncodingType.H264:
                    case EncodingType.H265:
                        Speed.Text = "码率/速度/已用时间/剩余时间";
                        break;
                    case EncodingType.AAC:
                    case EncodingType.FLAC:
                    case EncodingType.WAV:
                        Speed.Text = "速度/已用时间/剩余时间";
                        break;
                    case EncodingType.MP4:
                    case EncodingType.MKV:
                        Speed.Text = "已用时间";
                        break;
                }
                ModernDialog.ShowMessage(e.Error.Message, "RipStudio Message", MessageBoxButton.OK);
            }
            else
            {
                State.Text = "出错";
                StateBool = false;
                PB.Value = 0;
                switch (encodingType)
                {
                    case EncodingType.H264:
                    case EncodingType.H265:
                        Speed.Text = "码率/速度/已用时间/剩余时间";
                        break;
                    case EncodingType.AAC:
                    case EncodingType.FLAC:
                    case EncodingType.WAV:
                        Speed.Text = "速度/已用时间/剩余时间";
                        break;
                    case EncodingType.MP4:
                    case EncodingType.MKV:
                        Speed.Text = "已用时间";
                        break;
                }
                ModernDialog.ShowMessage((e.Result as string), "RipStudio Message", MessageBoxButton.OK);
            }
            //switch (encodingType)
            //{
            //    case EncodingType.WAV:
            //        wavEncoder.Close();
            //        break;
            //    case EncodingType.FLAC:
            //        flacEncoder.Close();
            //        break;
            //    case EncodingType.AAC:
            //        aacEncoder.Close();
            //        break;
            //    case EncodingType.H264:
            //        x264Encoder.Close();
            //        break;
            //    case EncodingType.H265:
            //        x265Encoder.Close();
            //        break;
            //    //case EncodingType.MP4:
            //    //case EncodingType.MKV:
            //    //    e.Result = ((libmuxer.Muxer)e.Argument).Start(fileout, (BackgroundWorker)sender);
            //    //    break;
            //    //case EncodingType.AUTO:
            //    //    e.Result = ((AutoEncoder)e.Argument).Start((BackgroundWorker)sender);
            //    //    break;
            //}
            sw.Stop();
        }

        private void ListViewItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (State.Text != "完成")
            {
                if (StateBool == false)
                {
                    ((MenuItem)(((JobItem)sender).ContextMenu.Items.GetItemAt(0))).IsEnabled = true;
                    ((MenuItem)(((JobItem)sender).ContextMenu.Items.GetItemAt(1))).IsEnabled = false;
                }
                else
                {
                    ((MenuItem)(((JobItem)sender).ContextMenu.Items.GetItemAt(0))).IsEnabled = false;
                    ((MenuItem)(((JobItem)sender).ContextMenu.Items.GetItemAt(1))).IsEnabled = true;
                }
            }
            else
            {
                ((MenuItem)(((JobItem)sender).ContextMenu.Items.GetItemAt(0))).IsEnabled = false;
                ((MenuItem)(((JobItem)sender).ContextMenu.Items.GetItemAt(1))).IsEnabled = false;
            }
        }

        private void 开始_Click(object sender, RoutedEventArgs e)
        {
            State.Text = "处理中";
            StateBool = true;
            backgroundWorker.RunWorkerAsync(jobConfig);
        }

        private void 停止_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void 删除_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
            ((App)(Application.Current)).LV.Items.Remove(this);
        }

        private void 打开输出文件夹_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(fileout))
            {
                System.Diagnostics.Process.Start("explorer",string.Format("/Select, {0}", fileout));
            }
            else
            {
                System.Diagnostics.Process.Start("explorer",string.Format("/N, {0}",System.IO.Path.GetDirectoryName(fileout)));
            }
        }

        private void 打开输入的脚本_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer",filein);
        }

        private void 查看任务信息_Click(object sender, RoutedEventArgs e)
        {
            switch (encodingType)
            {
                case EncodingType.H264:
                case EncodingType.H265:
                    ModernDialog.ShowMessage((string)jobConfig, "Configuration", MessageBoxButton.OK);
                    break;
                case EncodingType.AAC:
                case EncodingType.FLAC:
                    string lingshi="";
                    foreach (System.Reflection.PropertyInfo p in jobConfig.GetType().GetProperties())
                    {
                        lingshi += p.Name + ":" + p.GetValue(jobConfig)+"\r\n";
                    }
                    ModernDialog.ShowMessage(lingshi, "Configuration", MessageBoxButton.OK);
                    break;
                case EncodingType.WAV: 
                    ModernDialog.ShowMessage("WAV是没有参数可选的", "Configuration", MessageBoxButton.OK); 
                    break;
                case EncodingType.MP4:
                case EncodingType.MKV:
                    ModernDialog.ShowMessage(((libmuxer.Muxer)jobConfig).Arguments, "Configuration", MessageBoxButton.OK); 
                    break;
                case EncodingType.AUTO:
                    ModernDialog.ShowMessage(((libmuxer.Muxer)jobConfig).Arguments, "Configuration", MessageBoxButton.OK);
                    break;
            }


        }
    }
}
