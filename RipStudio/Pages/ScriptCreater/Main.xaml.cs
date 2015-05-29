using FirstFloor.ModernUI.Windows.Controls;
using MediaInfoDotNet;
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
        //private void 预览脚本_Click(object sender, RoutedEventArgs e)
        //{
        //    VideoPlayr vp = new VideoPlayr(@"C:\Users\Liga\Documents\Visual Studio 2013\Projects\avs2yuv\Debug\in.avs");
        //    vp.Show();
        //}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AudioTB.IsEnabled = false;
            AudioB.IsEnabled = false;
            if (VideoTB.Text != string.Empty)
            {
                AudioTB.Text = VideoTB.Text;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AudioTB.IsEnabled = true;
            AudioB.IsEnabled = true;
            AudioTB.Text = string.Empty;
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
            if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) == ".mp4")
            {
                VideoTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (IsSame.IsChecked == true)
                {
                    AudioTB.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                }
            }
            else
            {
                ModernDialog.ShowMessage("拖入的不是所允许的文件。", "RipStudio Message", MessageBoxButton.OK);
            }
        }

        private void SaveScript_Click(object sender, RoutedEventArgs e)
        {
            if (VideoTB.Text != String.Empty || AudioTB.Text != String.Empty)
            {

                string Script = GetScript();
                if (Script != null)
                {
                    try
                    {
                        if (IsSaveToVideoFolder.IsChecked == false)
                        {
                            StreamWriter sw = new StreamWriter(System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".avs", false, Encoding.Default);
                            sw.WriteLine(Script);
                            sw.Close();
                            ModernDialog.ShowMessage("保存成功！", "RipStudio Message", MessageBoxButton.OK);
                        }
                        else
                        {
                            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                            dlg.DefaultExt = ".avs"; // Default file extension
                            dlg.Filter = "AviSynth Script|*.avs"; // Filter files by extension

                            // Show open file dialog box
                            Nullable<bool> result = dlg.ShowDialog();
                            // Process open file dialog box results
                            if (result == true)
                            {
                                // Open document
                                StreamWriter sw = new StreamWriter(dlg.FileName, false, Encoding.Default);
                                sw.WriteLine(Script);
                                sw.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "AviSynth Error", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                ModernDialog.ShowMessage("没有载入视频文件！", "RipStudio Message", MessageBoxButton.OK);
            }
        }
        private string GetScript()
        {
            try
            {
                string avs脚本 = "SetWorkingDir(\"" + System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\AviSynthPlugins\" + "\")\r\n\r\n";
                if (VideoTB.Text != string.Empty)
                {
                    #region VideoMain
                    if (((App)(Application.Current)).VideoMain == null)
                    {
                        avs脚本 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                        avs脚本 += "LWLibavVideoSource(\"" + ((App)(Application.Current)).avs_Main.VideoTB.Text + "\")\r\n";
                    }
                    else
                    {
                        switch (((App)(Application.Current)).VideoMain.Decoder.SelectedIndex)
                        {
                            case 0: avs脚本 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n"; break;
                            case 1: avs脚本 += "LoadPlugin(\"" + "DGDecode" + ".dll\")\r\n"; break;
                            case 2: avs脚本 += "LoadPlugin(\"" + "ffms2" + ".dll\")\r\n"; break;
                        }
                        avs脚本 += ((App)(Application.Current)).VideoMain.Decoder.Text + "(\"" + ((App)(Application.Current)).avs_Main.VideoTB.Text + "\")\r\n";

                        if (((App)(Application.Current)).VideoMain.Crop.IsChecked == true)
                        {
                            avs脚本 += "crop(" + ((App)(Application.Current)).VideoMain.Crop1.Text + "," + ((App)(Application.Current)).VideoMain.Crop1.Text + ",-" + ((App)(Application.Current)).VideoMain.Crop3.Text + ",-" + ((App)(Application.Current)).VideoMain.Crop4.Text + ")\r\n";
                        }
                        if (((App)(Application.Current)).VideoMain.AddBorder.IsChecked == true)
                        {
                            avs脚本 += "AddBorder(" + ((App)(Application.Current)).VideoMain.Border1.Text + "," + ((App)(Application.Current)).VideoMain.Border1.Text + ",-" + ((App)(Application.Current)).VideoMain.Border3.Text + ",-" + ((App)(Application.Current)).VideoMain.Border4.Text + ")\r\n";
                        }
                        if (((App)(Application.Current)).VideoMain.重设分辨率.IsChecked == true)
                        {
                            switch (((App)(Application.Current)).VideoMain.Resolution.SelectedIndex)
                            {
                                case 0: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (7680).ToString() + "," + (4320).ToString() + ")\r\n"; break;
                                case 1: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (3840).ToString() + "," + (2160).ToString() + ")\r\n"; break;
                                case 2: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (1920).ToString() + "," + (1080).ToString() + ")\r\n"; break;
                                case 3: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (1366).ToString() + "," + (768).ToString() + ")\r\n"; break;
                                case 4: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (1280).ToString() + "," + (720).ToString() + ")\r\n"; break;
                                case 5: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (854).ToString() + "," + (480).ToString() + ")\r\n"; break;
                                case 6: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + (800).ToString() + "," + (480).ToString() + ")\r\n"; break;
                                case 7: avs脚本 += ((App)(Application.Current)).VideoMain.重设分辨率滤镜.Text + "(" + ((App)(Application.Current)).VideoMain.AVS_Width.Text + "," + ((App)(Application.Current)).VideoMain.AVS_Height.Text + ")\r\n"; break;
                            }
                        }
                    }
                    #endregion
                    #region Color
                    if (((App)(Application.Current)).VideoColor != null)
                    {
                        if (((App)(Application.Current)).VideoColor.Tweak.IsChecked == true)
                        {
                            avs脚本 += "Tweak(" + ((App)(Application.Current)).VideoColor.Tweak1.Text + "," + ((App)(Application.Current)).VideoColor.Tweak2.Text + "," + ((App)(Application.Current)).VideoColor.Tweak3.Text + "," + ((App)(Application.Current)).VideoColor.Tweak4.Text + ")\r\n";
                        }
                        if (((App)(Application.Current)).VideoColor.Levels.IsChecked == true)
                        {
                            avs脚本 += "Levels(" + ((App)(Application.Current)).VideoColor.Levels1.Text + "," + ((App)(Application.Current)).VideoColor.Levels2.Text + "," + ((App)(Application.Current)).VideoColor.Levels3.Text + "," + ((App)(Application.Current)).VideoColor.Levels4.Text + "," + ((App)(Application.Current)).VideoColor.Levels5.Text + ")\r\n";
                        }
                        if (((App)(Application.Current)).VideoColor.ColorMatrix.IsChecked == true)
                        {
                            if (((App)(Application.Current)).VideoColor.ColorMatrixComboBox1.SelectedIndex != ((App)(Application.Current)).VideoColor.ColorMatrixComboBox2.SelectedIndex)
                            {
                                avs脚本 += "LoadPlugin(\"" + "ColorMatrix" + ".dll\")\r\n"; 
                                avs脚本 += "ColorMatrix(mode=\"" + ((App)(Application.Current)).VideoColor.ColorMatrixComboBox1.Text + "->" + ((App)(Application.Current)).VideoColor.ColorMatrixComboBox2.Text + ")\r\n";
                            }
                        }

                    }
                    #endregion
                    #region TIVTC
                    #endregion
                    #region De
                    if (((App)(Application.Current)).VideoDe != null)
                    {
                        #region Deblocker
                        if (((App)(Application.Current)).VideoDe.Deblocker.IsChecked == true)
                        {
                            if (((App)(Application.Current)).VideoDe.DeblockerComboBox.SelectedIndex == 1)
                            {
                                avs脚本 += "LoadPlugin(\"" + "unblock" + ".dll\")\r\n";
                                avs脚本 += "Unblock(";
                                if (((App)(Application.Current)).VideoDe.Unblock_Chroma.IsChecked == false)
                                {
                                    avs脚本 += "chroma=false,";
                                }
                                if (((App)(Application.Current)).VideoDe.Unblock_Photo.IsChecked == true)
                                {
                                    avs脚本 += "photo=true";
                                    if (((App)(Application.Current)).VideoDe.Unblock_Cartoon.IsChecked == true)
                                    {
                                        avs脚本 += ",cartoon=true";
                                    }
                                }
                                else if (((App)(Application.Current)).VideoDe.Unblock_Cartoon.IsChecked == true)
                                {
                                    avs脚本 += "cartoon=true";
                                }
                                avs脚本 += ")\r\n";
                            }
                            else
                            {
                                if (int.Parse(((App)(Application.Current)).VideoDe.quant1.Text) < 2 && int.Parse(((App)(Application.Current)).VideoDe.quant1.Text) > 40) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.quant2.Text) < 2 && int.Parse(((App)(Application.Current)).VideoDe.quant2.Text) > 40) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.aOff1.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.aOff1.Text) > 10) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.bOff1.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.bOff1.Text) > 10) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.aOff2.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.aOff2.Text) > 10) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.bOff2.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.bOff2.Text) > 10) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.uv.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.uv.Text) > 3) { return null; }
                                avs脚本 += "LoadPlugin(\"" + "mt_masktools-26" + ".dll\")\r\n";
                                avs脚本 += "LoadPlugin(\"" + "DctFilter" + ".dll\")\r\n";
                                avs脚本 += "LoadPlugin(\"" + "deblock" + ".dll\")\r\n";
                                avs脚本 += "Import(\"" + "Deblock_QED.avs" + ".dll\")\r\n";
                                avs脚本 += "Deblock_QED(";
                                avs脚本 += "quant1=" + ((App)(Application.Current)).VideoDe.quant1.Text;
                                avs脚本 += ",quant2=" + ((App)(Application.Current)).VideoDe.quant1.Text;
                                avs脚本 += ",aOff1=" + ((App)(Application.Current)).VideoDe.aOff1.Text;
                                avs脚本 += ",bOff1=" + ((App)(Application.Current)).VideoDe.bOff1.Text;
                                avs脚本 += ",aOff2=" + ((App)(Application.Current)).VideoDe.aOff2.Text;
                                avs脚本 += ",bOff2=" + ((App)(Application.Current)).VideoDe.bOff2.Text;
                                avs脚本 += ",uv=" + ((App)(Application.Current)).VideoDe.uv.Text;
                                avs脚本 += ")\r\n";
                            }
                        #endregion
                            #region DeBand
                            if (((App)(Application.Current)).VideoDe.DeBand.IsChecked == true)
                            {
                                if (((App)(Application.Current)).VideoDe.DeBandComboBox.SelectedIndex == 0)
                                {
                                    if (float.Parse(((App)(Application.Current)).VideoDe.GradFun2D_Threshold.Text) < 0.1 || float.Parse(((App)(Application.Current)).VideoDe.GradFun2D_Threshold.Text) > 10)
                                    {
                                        return null;
                                    }
                                    else if (float.Parse(((App)(Application.Current)).VideoDe.GradFun2D_Threshold.Text) == 1.2)
                                    {
                                        avs脚本 += "LoadPlugin(\"" + "gradfun2db" + ".dll\")\r\n";
                                        avs脚本 += "GradFun2db()\r\n";
                                    }
                                    else
                                    {
                                        avs脚本 += "LoadPlugin(\"" + "gradfun2db" + ".dll\")\r\n";
                                        avs脚本 += "Unblok(thr=";
                                        avs脚本 += ((App)(Application.Current)).VideoDe.GradFun2D_Threshold.Text;
                                        avs脚本 += ")\r\n";
                                    }
                                }
                                else
                                {
                                    if (int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Range.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.quant1.Text) > 31) { return null; }
                                    if (int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Cbthresh.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Cbthresh.Text) > 511) { return null; }
                                    if (int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Cdither.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Cdither.Text) > 511) { return null; }
                                    if (int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Crthresh.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Crthresh.Text) > 511) { return null; }
                                    if (int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Ydither.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Ydither.Text) > 511) { return null; }
                                    if (int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Ythresh.Text) < 1 && int.Parse(((App)(Application.Current)).VideoDe.Flash3k_Ythresh.Text) > 511) { return null; }

                                    avs脚本 += "LoadPlugin(\"" + "flash3kyuu_deband" + ".dll\")\r\n";
                                    avs脚本 += "f3kdb(";
                                    avs脚本 += "range=" + ((App)(Application.Current)).VideoDe.Flash3k_Range.Text;
                                    avs脚本 += ",sample_mode=" + ((App)(Application.Current)).VideoDe.Flash3k_Mode.SelectedIndex.ToString();
                                    avs脚本 += ",dither_algo=" + ((App)(Application.Current)).VideoDe.Flash3k_Precision.SelectedIndex.ToString();
                                    avs脚本 += "grainY=" + ((App)(Application.Current)).VideoDe.Flash3k_Ydither.Text;
                                    avs脚本 += "grainC=" + ((App)(Application.Current)).VideoDe.Flash3k_Cdither.Text;
                                    avs脚本 += ",dynamic_grain=" + ((App)(Application.Current)).VideoDe.Flash3k_DynamicDither.IsChecked.ToString();
                                    avs脚本 += "Y=" + ((App)(Application.Current)).VideoDe.Flash3k_Ythresh.Text;
                                    avs脚本 += "Cb=" + ((App)(Application.Current)).VideoDe.Flash3k_Cbthresh.Text;
                                    avs脚本 += "Cr=" + ((App)(Application.Current)).VideoDe.Flash3k_Crthresh.Text;

                                    avs脚本 += ",blur_first=" + ((App)(Application.Current)).VideoDe.Flash3k_BlurFirst.IsChecked.ToString();
                                    avs脚本 += ",keep_tv_range=" + ((App)(Application.Current)).VideoDe.Flash3k_ForceTVRange.IsChecked.ToString();
                                    avs脚本 += ")\r\n";
                                }
                            }
                        }
                            #endregion
                        #region DeHalo
                        if (((App)(Application.Current)).VideoDe.DeHalo.IsChecked == true)
                        {
                            if (((App)(Application.Current)).VideoDe.DeHaloComboBox.SelectedIndex == 0)
                            {
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_rx.Text) < 1 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_rx.Text) > 3) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_ry.Text) < 1 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_ry.Text) > 3) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_highsens.Text) < 0 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_highsens.Text) > 100) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_lowsens.Text) < 0 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_lowsens.Text) > 100) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_darkstr.Text) < 0 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_darkstr.Text) > 1) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_brightstr.Text) < 0 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_brightstr.Text) > 1) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_ss.Text) < 0.01 || float.Parse(((App)(Application.Current)).VideoDe.DeHaloAlpha_darkstr.Text) > 4) { return null; }

                                avs脚本 += "LoadPlugin(\"" + "mt_masktools-26" + ".dll\")\r\n";
                                avs脚本 += "LoadPlugin(\"" + "RepairSSE2" + ".dll\")\r\n";
                                avs脚本 += "Import(\"" + "Dehalo_alpha_mt" + ".avsi\")\r\n";

                                avs脚本 += "DeHalo_alpha_mt(";
                                avs脚本 += "rx=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_rx.Text;
                                avs脚本 += ",ry=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_ry.Text;
                                avs脚本 += ",darkstr=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_darkstr.Text;
                                avs脚本 += ",brightstr=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_brightstr.Text;
                                avs脚本 += ",darkstr=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_darkstr.Text;
                                avs脚本 += ",brightstr=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_brightstr.Text;
                                avs脚本 += ",highsens=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_highsens.Text;
                                avs脚本 += ",lowsens=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_lowsens.Text;
                                avs脚本 += ",ss=" + ((App)(Application.Current)).VideoDe.DeHaloAlpha_ss.Text;
                                avs脚本 += ")\r\n";

                            }
                            else
                            {
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_rx.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_rx.Text) > 100) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_ry.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_ry.Text) > 100) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_strength.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_strength.Text) > 1000) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_lodamp.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_lodamp.Text) > 20) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_hidamp.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_hidamp.Text) > 20) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_sharpness.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_sharpness.Text) > 1.58) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_tweaker.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_tweaker.Text) > 1) { return null; }
                                if (float.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_sharpness.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_sharpness.Text) > 1.58) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_PPmode.Text) < 0 && int.Parse(((App)(Application.Current)).VideoDe.BlindDeHalo_PPmode.Text) > 100) { return null; }

                                avs脚本 += "LoadPlugin(\"" + "mt_masktools-26" + ".dll\")\r\n";
                                avs脚本 += "LoadPlugin(\"" + "RemoveGrainSSE2" + ".dll\")\r\n";
                                avs脚本 += "LoadPlugin(\"" + "mt_masktools-26" + ".dll\")\r\n";
                                avs脚本 += "Import(\"" + "BlindDeHalo3_mt2" + ".avs\")\r\n";

                                avs脚本 += "BlindDeHalo3(";
                                avs脚本 += "rx=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_rx.Text;
                                avs脚本 += ",ry=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_ry.Text;
                                avs脚本 += ",strength=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_strength.Text;
                                avs脚本 += ",lodamp=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_lodamp.Text;
                                avs脚本 += ",hidamp=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_hidamp.Text;
                                avs脚本 += ",sharpness=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_sharpness.Text;
                                avs脚本 += ",PPmode=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_PPmode.Text;
                                avs脚本 += ",PPlimit=" + ((App)(Application.Current)).VideoDe.BlindDeHalo_PPlimit.Text;
                                avs脚本 += ",interlaced=false))\r\n";
                            }
                        }

                        #endregion
                        #region DeRinging
                        if (((App)(Application.Current)).VideoDe.DeRinging.IsChecked == true)
                        {
                            if (((App)(Application.Current)).VideoDe.DeRingingComboBox.SelectedIndex == 0)
                            {
                                if (int.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_strength.Text) < 0 || int.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_strength.Text) > 32) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_restore.Text) < 0 || float.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_restore.Text) > 128) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_radius.Text) < 1 || float.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_radius.Text) > 2) { return null; }
                                if (int.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_threads.Text) < 0 || float.Parse(((App)(Application.Current)).VideoDe.MosquitoNR_threads.Text) > 32) { return null; }

                                avs脚本 += "LoadPlugin(\"" + "MosquitoNR" + ".dll\")\r\n";

                                avs脚本 += "MosquitoNR(";
                                avs脚本 += "strength=" + ((App)(Application.Current)).VideoDe.MosquitoNR_strength.Text;
                                avs脚本 += ",restore=" + ((App)(Application.Current)).VideoDe.MosquitoNR_restore.Text;
                                avs脚本 += ",radius=" + ((App)(Application.Current)).VideoDe.MosquitoNR_radius.Text;
                                avs脚本 += ",threads=" + ((App)(Application.Current)).VideoDe.MosquitoNR_threads.Text;
                                avs脚本 += ")\r\n";
                            }
                            else
                            {
                                avs脚本 += "LoadPlugin(\"" + "UnDot" + ".dll\")\r\n";

                                avs脚本 += "Undot()\r\n";
                            }
                        }

                        #endregion
                    }
                    #endregion
                    #region Subtitle
                    if (((App)(Application.Current)).VideoSubtitle != null && ((App)(Application.Current)).VideoSubtitle.SubList.Items.Count > 0)
                    {
                        avs脚本 += "LoadPlugin(\"" + ((App)(Application.Current)).VideoSubtitle.SubtitleFilter.Text + ".dll\")\r\n";
                        for (int i = 0; i < ((App)(Application.Current)).VideoSubtitle.SubList.Items.Count; i++)
                        {
                            avs脚本 += "TextSub(\"" + ((TreeViewItem)(((App)(Application.Current)).VideoSubtitle.SubList.Items.GetItemAt(i))).Header.ToString() + "\")\r\n";
                        }
                        if (AutoLoadSub.IsChecked == true)
                        {
                            avs脚本 += "TextSub(\"" + System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".ass" + "\")\r\n";
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
                                    avs脚本 += "LoadPlugin(\"" + ((App)(Application.Current)).VideoSubtitle.SubtitleFilter.Text + ".dll\")\r\n";
                                }
                                else
                                {
                                    avs脚本 += "LoadPlugin(\"" + "xy-VSFilter" + ".dll\")\r\n";
                                }
                                avs脚本 += "TextSub(\"" + System.IO.Path.GetDirectoryName(VideoTB.Text) + System.IO.Path.GetFileNameWithoutExtension(VideoTB.Text) + ".ass" + "\")\r\n";
                            }
                        }
                    }

                    #endregion
                    #region Other
                    if (((App)(Application.Current)).VideoOther != null)
                    {
                        if (((App)(Application.Current)).VideoOther.LOGO.IsChecked == true)
                        {
                            avs脚本 += "Import(\"Logo.avs\")\r\n";
                            avs脚本 += "Logo(\"" + ((App)(Application.Current)).VideoOther.LOGOFile.Text + "\",x=" + ((App)(Application.Current)).VideoOther.Logo_X.Text + ",y=" + ((App)(Application.Current)).VideoOther.Logo_Y.Text;
                            avs脚本 += ",start=" + ((App)(Application.Current)).VideoOther.Logo_Start.Text + ",end=" + ((App)(Application.Current)).VideoOther.Logo_End.Text;
                            avs脚本 += ",I=" + ((App)(Application.Current)).VideoOther.Logo_I.Text + ",O=" + ((App)(Application.Current)).VideoOther.Logo_O.Text;
                            avs脚本 += ",mode=" + ((App)(Application.Current)).VideoOther.Logo_BlendMode.Text;
                            if (((App)(Application.Current)).VideoOther.Logo_Chroma.IsChecked == true && ((App)(Application.Current)).VideoOther.Logo_BlendMode.SelectedIndex < 3)
                            {
                                avs脚本 += ",chr=true";
                            }
                            avs脚本 += ",Wstr" + ((App)(Application.Current)).VideoOther.Logo_Intensity.Text;


                            avs脚本 += ",blur" + ((App)(Application.Current)).VideoOther.Logo_Blur.Text;
                            avs脚本 += ",Opac" + ((App)(Application.Current)).VideoOther.Logo_Opacity.Text;
                            if (((App)(Application.Current)).VideoOther.Logo_MatteCarving.IsChecked == true)
                            {
                                avs脚本 += ",matte=true";
                            }
                            if (System.IO.Path.GetExtension(((App)(Application.Current)).VideoOther.LOGOFile.Text) == ".gif")
                            {
                                avs脚本 += ",anim_gif=true";

                            }
                            avs脚本 += ")\r\n";
                        }



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
                if (AudioTB.Text != string.Empty)
                {
                    if (VideoTB.Text != string.Empty)
                    {
                        avs脚本 += "Video = last\r\n";
                    }
                    //avs脚本 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n";
                    //if (((App)(Application.Current)).AudioMain.DynamicrangeCompression)
                    //{ 

                    //}
                    if (((App)(Application.Current)).AudioMain != null)
                    {
                        if (((App)(Application.Current)).AudioMain.Decoder.SelectedIndex != 0)
                        {
                            switch (((App)(Application.Current)).AudioMain.Decoder.SelectedIndex)
                            {
                                case 3: avs脚本 += "LoadPlugin(\"" + "LSMASHSource" + ".dll\")\r\n"; break;
                                case 2: avs脚本 += "LoadPlugin(\"" + "bass" + ".dll\")\r\n"; break;
                                case 1: avs脚本 += "LoadPlugin(\"" + "ffms2" + ".dll\")\r\n"; break;
                            }
                            avs脚本 += "Audio = " + ((App)(Application.Current)).AudioMain.Decoder.Text + "(\"" + AudioTB.Text + "\")\r\n";
                        }
                        else
                        {
                            avs脚本 += "LoadPlugin(\"" + "ffms2" + ".dll\")\r\n";
                            avs脚本 += "Audio = FFAudioSource(\"" + AudioTB.Text + "\")\r\n";
                        }



                        if (int.Parse(((App)(Application.Current)).AudioMain.Delay.Text) != 0)
                        {
                            avs脚本 += "DelayAudio(" + ((App)(Application.Current)).AudioMain.Delay.Text + ".0/1000.0)\r\n";
                        }
                        if (((App)(Application.Current)).AudioMain.SampleRateCB.SelectedIndex != 0)
                        {
                            switch (((App)(Application.Current)).AudioMain.SampleRateCB.SelectedIndex)
                            {
                                case 1: avs脚本 += "SSRC(8000)\r\n"; break;
                                case 2: avs脚本 += "SSRC(11025)\r\n"; break;
                                case 3: avs脚本 += "SSRC(22050)\r\n"; break;
                                case 4: avs脚本 += "SSRC(32000)\r\n"; break;
                                case 5: avs脚本 += "SSRC(44100)\r\n"; break;
                                case 6: avs脚本 += "SSRC(48000)\r\n"; break;
                                case 7: avs脚本 += "SSRC(96000)\r\n"; break;
                            }
                        }

                        if (((App)(Application.Current)).AudioMain.Normalize.IsChecked == true)
                        {
                            if (int.Parse(((App)(Application.Current)).AudioMain.NormalizeTB.Text) < 0 && int.Parse(((App)(Application.Current)).AudioMain.NormalizeTB.Text) > 100)
                            {
                                return null;
                            }
                            avs脚本 += "Normalize(0." + ((App)(Application.Current)).AudioMain.NormalizeTB.Text + ")\r\n";
                        }
                    }
                    if (VideoTB.Text != string.Empty)
                    {
                        avs脚本 += "AudioDub(Video, Audio)\r\n";
                    }
                }






                if (((App)(Application.Current)).avs_Cut != null)
                {
                    if (VideoTB.Text == string.Empty && AudioTB.Text != string.Empty)
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
                return avs脚本;
            }
            catch (FormatException fe)
            {
                ModernDialog.ShowMessage("有参数格式不正确！", "RipStudio Message", MessageBoxButton.OK);
                return null;
            }
            catch (Exception ee)
            {
                ModernDialog.ShowMessage(ee.Message, "RipStudio Message", MessageBoxButton.OK);
                return null;
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
                        ModernDialog.ShowMessage(GetScript(), "RipStudio Message", MessageBoxButton.OK);
                        VideoPlayr vp = new VideoPlayr(Script, false);
                        vp.Show();
                    }
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "AviSynth Error", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("没有载入视频文件！", "RipStudio Message", MessageBoxButton.OK);
            }

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
                if (IsSame.IsChecked == true)
                {
                    AudioTB.Text = dlg.FileName;
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

    }
}
