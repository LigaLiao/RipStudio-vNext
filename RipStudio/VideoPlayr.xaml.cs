using AvisynthWrapper;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for ModernWindow1.xaml
    /// </summary>
    public partial class VideoPlayr : ModernWindow
    {
        private Avisynth avs;
        private ScriptInfo si;
        private string File;
        private bool IsFile;
        private int framenum=0;
        private float framerate=1;
        public VideoPlayr(string arg, bool isFile)
        {
            InitializeComponent();
            File = arg;
            IsFile = isFile;
            if(IsFile==true)
            {
                this.Title = File;
            }

            avs = new Avisynth(File, isFile);
            si = avs.GetScriptInfo();
            if (si.HasVideo)
            {
                playProgressSlider.Maximum = si.num_frames;
                framerate = (float)si.fps_numerator / (float)si.fps_denominator;
                III.Source = ReadFrameBitmap(avs.GetVideoFrame(0));
            }
            else
            {
                if (avs != null)
                {
                    avs.FreeAvisynth();
                }
                throw new Exception("No Video.");
            }
        }

        private void playProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            framenum = (int)e.NewValue;
            III.Source = ReadFrameBitmap(avs.GetVideoFrame(framenum));
            zhenshu.Text = "当前是第" + (framenum).ToString() + "帧";
            TimeSpan ts = new TimeSpan(0, 0, (int)(framenum / framerate));
            TimeSpan ts2 = new TimeSpan(0, 0, (int)(si.num_frames / framerate));
            zhenshu.Text = "帧位置：" + framenum + "/" + si.num_frames + "帧  时间位置：" + ts.ToString() + "/" + ts2.ToString();
        }
        private BitmapImage ReadFrameBitmap(Bitmap BMP)
        {
            MemoryStream ms = new MemoryStream();
            BitmapImage bitmap = new BitmapImage();
            BMP.RotateFlip(RotateFlipType.Rotate180FlipX);
            BMP.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(ms.GetBuffer());
            bitmap.EndInit();
            ms.Dispose();
            BMP.Dispose();
            GC.Collect();
            return bitmap;
        }


        private void III_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "bmp|*.bmp;";
            if (IsFile)
            {
                dlg.FileName = System.IO.Path.GetFileNameWithoutExtension(File) + "_Screenshot_" + ((int)playProgressSlider.Value).ToString();
            }
            else
            {
                dlg.FileName = "Screenshot_" + ((int)playProgressSlider.Value).ToString();
            }
         
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                    Bitmap BMP = avs.GetVideoFrame((int)playProgressSlider.Value);
                    BMP.RotateFlip(RotateFlipType.Rotate180FlipX);
                    BMP.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                framenum = int.Parse(ZHENTB.Text);
                if (framenum > si.num_frames || framenum < 0)
                {
                    ModernDialog.ShowMessage("你输入的帧数超出了范围！", "RipStudio Message", MessageBoxButton.OK);
                }
                else
                {
                    playProgressSlider.Value = framenum;
                }
            }
            catch
            {
                ModernDialog.ShowMessage("你输入的不是数字或者不全是数字！", "RipStudio Message", MessageBoxButton.OK);
            }

        }



        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.WindowStyle = WindowStyle.None;
            if (avs != null)
            {
                avs.FreeAvisynth();
            }
        }

        private void 缩放_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //int.Parse((sender as TextBox).Text);
                //ScaleTransform scaleTran = new ScaleTransform(8.0, 8.0);
            //if (III != null)
            //{
                (III.RenderTransform as ScaleTransform).ScaleX = (III.RenderTransform as ScaleTransform).ScaleY = int.Parse((sender as TextBox).Text) / 100.0;
            //}


            }
            catch
            {

            }
        }


    }
}
