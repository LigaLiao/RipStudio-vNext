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
        private  Avisynth avs;
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
            ScriptInfo si = avs.GetScriptInfo();
            playProgressSlider.Maximum = framenum=si.num_frames;
            framerate = (float)si.fps_numerator / (float)si.fps_denominator;
            III.Source = ReadFrameBitmap(avs.GetVideoFrame(0));
        }

        private void playProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int NUM= (int)e.NewValue;
            III.Source = ReadFrameBitmap(avs.GetVideoFrame(NUM));
            zhenshu.Text = "当前是第" + (NUM).ToString()+ "帧";
            TimeSpan ts = new TimeSpan(0,0,(int)(NUM / framerate));
            TimeSpan ts2 = new TimeSpan(0, 0, (int)(framenum / framerate));
            zhenshu.Text = "帧位置：" + NUM + "/" + framenum + "帧  时间位置：" + ts.ToString() + "/" + ts2.ToString();
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
            if (ZHENTB.Text != "")
            {
                    int zhen;
                    zhen = int.Parse(ZHENTB.Text);
                    III.Source = ReadFrameBitmap(avs.GetVideoFrame(zhen));
                    playProgressSlider.Value = zhen;
            }
        }



        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.WindowStyle = WindowStyle.None;
        }
    }
}
