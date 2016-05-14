using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RipStudio
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            LoadSettings();
            InitializeComponent();
        }
        private void LoadSettings()
        {
            //AppearanceManager.Current.ThemeSource = new Uri("/RipStudio;component/Assets/Style.xaml", UriKind.Relative);

            if (AppearanceManager.Current.AccentColor != Properties.Settings.Default.AccentColor)
            {
                AppearanceManager.Current.AccentColor = Properties.Settings.Default.AccentColor;
            }
            if (AppearanceManager.Current.ThemeSource != Properties.Settings.Default.Theme_Uri)
            {
                AppearanceManager.Current.ThemeSource = Properties.Settings.Default.Theme_Uri;
            }
            if (Properties.Settings.Default.Language_Uri != new Uri("/RipStudio;component/Language/SimplifiedChinese.xaml", UriKind.Relative))
            {
                Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Contains("anguage")).Source = Properties.Settings.Default.Language_Uri;
            }

            //if (Properties.Settings.Default.ThemeImg_Uri != null && Properties.Settings.Default.ThemeImg_Name!=null)
            //{
            //    if(System.IO.File.Exists(Properties.Settings.Default.ThemeImg_Uri.ToString()))
            //    {
            //    ((Application.Current.FindResource("WindowBackgroundContent") as Rectangle).Fill as ImageBrush).ImageSource = new BitmapImage(Properties.Settings.Default.ThemeImg_Uri);
            //    }
            //                else
            //{
            //    Properties.Settings.Default.ThemeImg_Uri = null;
            //    Properties.Settings.Default.ThemeImg_Name = "No";
            //}
            //}
            //else
            //{
            //    Properties.Settings.Default.ThemeImg_Uri = null;
            //    Properties.Settings.Default.ThemeImg_Name = "No";
            //}
            //if (Properties.Settings.Default.ThemeImg_Opacity >= 0 && Properties.Settings.Default.ThemeImg_Opacity<=100)
            //{
            ////if (((Application.Current.FindResource("WindowBackgroundContent") as Rectangle).Fill as ImageBrush).Opacity != Properties.Settings.Default.ThemeImg_Opacity / 100.00)
            ////{
            //    ((Application.Current.FindResource("WindowBackgroundContent") as Rectangle).Fill as ImageBrush).Opacity = Properties.Settings.Default.ThemeImg_Opacity / 100.00;
            ////}           
            //}
            //else
            //{
            //    Properties.Settings.Default.ThemeImg_Opacity = 100 ;
            //}
        }
        private void ModernWindow_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.WindowStyle = WindowStyle.None;
        }
    }
}
