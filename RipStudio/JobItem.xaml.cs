using FirstFloor.ModernUI.Windows.Controls;
using libavs2aac;
using libavs2flac;
using libavs2wav;
using libavs2x264;
using libavs2x265;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public JobItem(string File_In, string File_Out, EncodingType Type, object config,bool IsWait)
        {
            InitializeComponent();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            filein = InTB.Text = File_In;
            fileout = OutTB.Text = File_Out;
            encodingType = Type;
            jobConfig = config;
            this.ToolTip = "编码类型：" + Type.ToString() + "\r\n输入文件：" + File_In + "\r\n输出文件：" + File_Out;
            if (IsWait == false)
            {
                State.Text = "准备";
                StateBool = true;
                backgroundWorker.RunWorkerAsync(config);
            }
            TypeTB.Text = Type.ToString();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch (encodingType)
                {
                    case EncodingType.WAV:
                        WavEncoder wavEncoder = new WavEncoder(filein);
                        e.Result = wavEncoder.Start(fileout, (BackgroundWorker)sender);
                        break;
                    case EncodingType.FLAC:
                        FlacEncoder flacEncoder = new FlacEncoder(filein);
                        e.Result = flacEncoder.Start(fileout, (BackgroundWorker)sender, (FlacEncoderConfig)e.Argument);
                        break;
                    case EncodingType.AAC:
                        AacEncoder aacEncoder = new AacEncoder(filein);
                        e.Result = aacEncoder.Start(fileout, (BackgroundWorker)sender, (AacEncoderConfig)e.Argument);
                        break;
                    case EncodingType.X264:
                        //X264Encoder x264Encoder = new X264Encoder(filein);
                        //e.Result = x264Encoder.Start(fileout, (BackgroundWorker)sender, Levels);
                        break;
                    case EncodingType.X265:
                        //X265Encoder x265Encoder = new X265Encoder(filein);
                        //e.Result = x265Encoder.Start(fileout, (BackgroundWorker)sender, Levels);
                        break;
                }
            }
            catch (Exception ex)
            {
                (sender as BackgroundWorker).ReportProgress(0);
                e.Result = ex.Message;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (State.Text != "编码中")
            {
                State.Text = "编码中";
            }
            PB.Value = e.ProgressPercentage;
            if (e.UserState!=null)
            {
            Speed.Text=((int)e.UserState).ToString()+"X";
            }
           
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                ModernDialog.ShowMessage("已取消", "RipStudio Message", MessageBoxButton.OK);
                StateBool = false;
                State.Text = "取消";
            }
            else if (e.Error != null)
            {
                State.Text = "出错";
                StateBool = false;
                ModernDialog.ShowMessage(e.Error.Message, "RipStudio Message", MessageBoxButton.OK);
            }
            else if (e.Result is string && (e.Result as string).Contains("Error"))
            {
                State.Text = "出错";
                StateBool = false;
                ModernDialog.ShowMessage((e.Result as string), "RipStudio Message", MessageBoxButton.OK);
            }
            else
            {
                State.Text = "完成";
                //ModernDialog.ShowMessage((e.Result as string), "RipStudio Message", MessageBoxButton.OK);
            }
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
            State.Text = "准备";
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
            ModernDialog.ShowMessage("待定", "RipStudio Message", MessageBoxButton.OK);
        }
    }
}
