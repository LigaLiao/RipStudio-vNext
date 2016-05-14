using FirstFloor.ModernUI.Windows.Controls;
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

namespace RipStudio.Pages.Video.H265Config
{
    /// <summary>
    /// Interaction logic for Advanced.xaml
    /// </summary>
    public partial class Advanced : UserControl
    {
        public Advanced()
        {
            InitializeComponent();
            ((App)(Application.Current)).H265Config_Advanced = this;
        }
        private string name=string.Empty;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((App)(Application.Current)).H265ConfigIsMain = false;
            if (((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.SelectedIndex == 0)
            {
                if (PNAME != null)
                {
                    PNAME.Visibility = Visibility.Visible;
                    NAME.Visibility = Visibility.Visible;
                }
                if (name != string.Empty)
                {
                    EncoderSettings.Text = string.Empty;
                    name = string.Empty;
                }
            }
            else
            {
                if (PNAME != null)
                {
                    PNAME.Visibility = Visibility.Hidden;
                    NAME.Visibility = Visibility.Hidden;
                }
                if (name != ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.Text)
                {
                    StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H265\" + ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.Text + ".txt", Encoding.Default);
                    EncoderSettings.Text = sr.ReadToEnd();
                    sr.Close();
                    name = ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.Text;
                }
            }
        }
        //private bool luoji=false;
        private void SAVE_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H265"))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H265");
            }
            if (((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.SelectedIndex==0)
            {
                    if (NAME.Text != string.Empty)
                    {
                        StreamWriter sw = new StreamWriter(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H265\" + NAME.Text + ".txt", false);
                        sw.WriteLine(EncoderSettings.Text);
                        sw.Close();
                        //luoji = false;
                        PNAME.Visibility = Visibility.Hidden;
                        NAME.Visibility = Visibility.Hidden;
                        ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.Items.Add(new ComboBoxItem() { Content = NAME.Text });
                        ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.SelectedIndex = ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.Items.Count-1;

                        ModernDialog.ShowMessage("已保存", "RipStudio Message", MessageBoxButton.OK);
                    }
                    else
                    {
                        ModernDialog.ShowMessage("Please enter a name", "RipStudio Message", MessageBoxButton.OK);
                    }
            }
            else
            {
                StreamWriter sw = new StreamWriter(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\EncoderSettings\H265\" + ((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.Text+ ".txt", false);
                sw.WriteLine(EncoderSettings.Text);
                sw.Close();
                ModernDialog.ShowMessage("已更新", "RipStudio Message", MessageBoxButton.OK);
            }
        }
    }
}
