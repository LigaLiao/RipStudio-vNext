using System;
using System.Collections.Generic;
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

namespace RipStudio.Pages.Video.H264Config
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();
            ((App)(Application.Current)).H264Config_Main = this;
        }
        string[] x264_preset_names = { "UltraFast", "SuperFast", "VeryFast", "Faster", "Fast", "Medium", "Slow", "Slower", "VerySlow", "Placebo" };
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PresetValue.Text = x264_preset_names[((int)e.NewValue)];
        }

        private void EncodingValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EncodingValueTB.Text = ((int)e.NewValue).ToString();
        }

        private void EncodingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //switch ((sender as ComboBox).SelectedIndex)
            //{
            //    case 1:
            //        EncodingModeTB.Text = "Quality:  ";
            //        //EncodingValueTB.Text = (23).ToString();
            //        EncodingValue.Minimum = 0;
            //        EncodingValue.Maximum = 51;
            //        EncodingValue.Value = 23;
            //        break;
            //    case 0:
            //        EncodingModeTB.Text = "Quantizer:  ";
            //        //EncodingValueTB.Text = (23).ToString();
            //        EncodingValue.Minimum = 0;
            //        EncodingValue.Maximum = 69;
            //        EncodingValue.Value = 23;
            //        break;
            //    case 2:
            //        EncodingModeTB.Text = "Bitrate:  ";
            //        //EncodingValueTB.Text = (1024).ToString();
            //        EncodingValue.Minimum = 512;
            //        EncodingValue.Maximum = 4096;
            //        EncodingValue.Value = 1024;
            //        break;
            //}
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    EncodingModeTB.Text = "Ratefactor:  ";
                    //EncodingValueTB.Text = (23).ToString();
                    EncodingValue.Minimum = 0;
                    EncodingValue.Maximum = 51;
                    EncodingValue.Value = 23;
                    break;
                case 1:
                    EncodingModeTB.Text = "Bitrate:  ";
                    //EncodingValueTB.Text = (1024).ToString();
                    EncodingValue.Minimum = 512;
                    EncodingValue.Maximum = 4096;
                    EncodingValue.Value = 1024;
                    break;
                case 2:
                    EncodingModeTB.Text = "Quantizer:  ";
                    //EncodingValueTB.Text = (23).ToString();
                    EncodingValue.Minimum = 0;
                    EncodingValue.Maximum = 69;
                    EncodingValue.Value = 23;
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((App)(Application.Current)).H264ConfigIsMain = true;
            if (((App)(Application.Current)).H264IOPAGE.EncoderSettings_CB.SelectedIndex == 0)
            {
                if (MainGrid != null)
                {
                    MainGrid.IsEnabled = true;
                }
            }
            else
            {
                if (MainGrid != null)
                {
                    MainGrid.IsEnabled = false;
                }
            }
        }
    }
}
