using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using libmuxer;
using MediaInfoLib;
//using MediaInfoDotNet.Models;
//using MediaInfoDotNet;
namespace RipStudio.Pages
{
    /// <summary>
    /// Interaction logic for mp4.xaml
    /// </summary>
    public partial class mp4 : UserControl
    {
        public mp4()
        {
            InitializeComponent();
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

        private void In_PreviewDrop0(object sender, DragEventArgs e)
        {
            try
            {
                (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (VideoOutput.Text == string.Empty)
                {
                    VideoOutput.Text = System.IO.Path.GetDirectoryName((sender as TextBox).Text) + System.IO.Path.GetFileNameWithoutExtension((sender as TextBox).Text) + "_muxed.mp4";
                }
            }
            catch
            {

            }
        }
        private void In_PreviewDrop1(object sender, DragEventArgs e)
        {
            try
            {
                string Extension = "";
                for (int i = 0; i < ((System.Array)e.Data.GetData(DataFormats.FileDrop)).Length; i++)
                {
                    //ModernDialog.ShowMessage(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString(), "RipStudio Message", MessageBoxButton.OK);
                    Extension = System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString());
                    if ((sender as TextBox).Text != string.Empty)
                    {
                        (sender as TextBox).Text += (";" + ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString());
                    }
                    else
                    {
                        (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString();
                    }
                }
            }
            catch
            {
                //ModernDialog.ShowMessage(ex.Message, "RipStudio Message", MessageBoxButton.OK);
            }
        }
        private void In_PreviewDrop2(object sender, DragEventArgs e)
        {
            try
            {
                string Extension = "";
                for (int i = 0; i < ((System.Array)e.Data.GetData(DataFormats.FileDrop)).Length; i++)
                {
                    Extension = System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString());
                    if (Extension == ".srt" || Extension == ".idx" || Extension == ".ttxt")
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
        private void In_PreviewDrop3(object sender, DragEventArgs e)
        {
            try
            {
                if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) == ".txt")
                {
                    (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                }
                else
                {
                    ModernDialog.ShowMessage("拖入的不是所允许的文件。", "RipStudio Message", MessageBoxButton.OK);
                }
            }
            catch
            {

            }
        }

        private void InB_Click0(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //dlg.DefaultExt = ".h264"; // Default file extension
            dlg.Filter = "Video format|*.264;*.265;*.m4v"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                if (VideoOutput.Text == string.Empty)
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
                    if (Count > 1)
                    {
                        //Out.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + @"\" + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_out.h264";
                        VideoOutput.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + @"\" + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_muxed.mp4";
                    }
                    else
                    {
                        VideoOutput.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_muxed.mp4";
                        //Out.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_out.h264";
                    }
                }
                VideoInput.Text = dlg.FileName;
            }
        }
        private void InB_Click1(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Audio format|*.aac;*.m4a;*.mp3;*.ac3"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    if (AudioInput.Text != string.Empty)
                    {
                        AudioInput.Text += ";" + dlg.FileNames[i];
                    }
                    else
                    {
                        AudioInput.Text = dlg.FileNames[i];
                    }
                }

            }
        }
        private void InB_Click2(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Subtitle format|*.srt;*.idx;*.ttxt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    if (SubtitleInput.Text != string.Empty)
                    {
                        SubtitleInput.Text += ";" + dlg.FileNames[i];
                    }
                    else
                    {
                        SubtitleInput.Text = dlg.FileNames[i];
                    }
                }

            }
        }
        private void InB_Click3(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //dlg.DefaultExt = ".h264"; // Default file extension
            dlg.Filter = "Chapters File|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                ChaptersFile.Text = dlg.FileName;
            }
        }
        private void OutB_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            if (VideoOutput.Text != string.Empty)
            {
                dlg.FileName = System.IO.Path.GetDirectoryName(VideoOutput.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoOutput.Text) + "_muxer";
            }
            dlg.Filter = "MP4 format|*.mp4"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                VideoOutput.Text = dlg.FileName;
            }
        }


        private bool isNumberic(string message)
        {
            try
            {
                Convert.ToDouble(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(VideoInput.Text) || !string.IsNullOrEmpty(VideoOutput.Text) )
                {
                    if (File.Exists(VideoInput.Text) || File.Exists(VideoOutput.Text))
                    {

                        libmuxer.Muxer muxer = new libmuxer.Muxer(MuxerType.MP4);
                        DeviceType dy = DeviceType.Standard;
                        switch (DeviceTypeCB.SelectedIndex)
                        {
                            case 0:
                                dy = DeviceType.Standard;
                                break;
                            case 1:
                                dy = DeviceType.psp;
                                break;
                            case 2:
                                dy = DeviceType.ipod;
                                break;
                            case 3:
                                dy = DeviceType.OtherAppleDevices;
                                break;
                        }

                        if (FPS.SelectedIndex != 0)
                        {
                            if (isNumberic(FPS.Text))
                            {
                                muxer.AddVideo(VideoInput.Text, dy, float.Parse(FPS.Text));
                            }
                            else
                            {
                                ModernDialog.ShowMessage("FPS不正确！", "RipStudio Message", MessageBoxButton.OK);
                            }

                        }
                        else
                        {
                            try
                            {
                                MediaInfo MI = new MediaInfo();
                                MI.Open(VideoInput.Text);
                                float fps = 0;
                                if (float.TryParse(MI.Get(StreamKind.Video, 0, "FrameRate"), out fps) == true) //判断是否可以转换为整型
                                {
                                    muxer.AddVideo(VideoInput.Text, DeviceType.Standard, fps);
                                }
                                else
                                {
                                    muxer.AddVideo(VideoInput.Text, DeviceType.Standard, 23.976f);
                                }
                                MI.Close();
                            }
                            catch
                            {
                                muxer.AddVideo(VideoInput.Text, dy, 23.976f);
                            }
                        }


                        string[] AArray = AudioInput.Text.Split(';');
                        if (AArray[0] != string.Empty)
                        {
                            foreach (string i in AArray)
                            {
                                muxer.AddAudio(i.ToString());
                            }
                        }

                        string[] SArray = SubtitleInput.Text.Split(';');
                        if (SArray[0] != string.Empty)
                        {
                            foreach (string i in SArray)
                            {
                                muxer.AddSubtitle(i.ToString());
                            }
                        }
                        if (ChaptersFile.Text != "")
                        {
                            muxer.Chapters = ChaptersFile.Text;
                        }

                        ((App)(Application.Current)).LV.Items.Add(new JobItem(VideoInput.Text, VideoOutput.Text, EncodingType.MP4, muxer, Now.IsChecked == true ? false : true));
                    }
                    else
                    {
                        ModernDialog.ShowMessage("视频输入输出有不存在项。", "RipStudio Message", MessageBoxButton.OK);
                    }
                }
                else
                {
                    ModernDialog.ShowMessage("视频输入输出有未指定项。", "RipStudio Message", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }
        //private string lsAVS = string.Empty;
        //private void VideoInput_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (((App)(Application.Current)).PresetAVS != string.Empty)
        //    {
        //        if (((App)(Application.Current)).PresetAVS != lsAVS)
        //        {
        //            VideoInput.Text = lsAVS = ((App)(Application.Current)).PresetAVS;
        //        }
        //    }
        //}

        private void VideoInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(VideoOutput.Text))
            {
                if (File.Exists(VideoInput.Text))
                {
                    string s = @"\";
                    int Star = 0;
                    int Count = 0;
                    while (Star != -1)
                    {
                        Star = VideoInput.Text.IndexOf(s, Star);
                        if (Star != -1)
                        {
                            Count++;
                            Star++;
                        }
                    }
                    if (Count > 1)
                    {
                        VideoOutput.Text = System.IO.Path.GetDirectoryName(VideoInput.Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension(VideoInput.Text) + "_muxed.mp4";
                    }
                    else
                    {
                        VideoOutput.Text = System.IO.Path.GetDirectoryName(VideoInput.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoInput.Text) + "_muxed.mp4";
                    }
                }
            }
        }
    }
}
