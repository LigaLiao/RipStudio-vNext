using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
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

namespace RipStudio
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }
        private void LoadSettings()
        {
            try
            {
                if (AppearanceManager.Current.AccentColor != Properties.Settings.Default.AccentColor)
                {
                    AppearanceManager.Current.AccentColor = Properties.Settings.Default.AccentColor;
                }
                if (AppearanceManager.Current.ThemeSource != Properties.Settings.Default.Theme_Uri)
                {
                    AppearanceManager.Current.ThemeSource = Properties.Settings.Default.Theme_Uri;
                }
                if (Properties.Settings.Default.Language_Uri != new Uri("/RipStudio;component/Language/SimplifiedChinese.xaml"))
                {
                    //ResourceDictionary resourceDictionary = new ResourceDictionary();
                    //resourceDictionary.Source = Properties.Settings.Default.Language_Uri;
                    ////Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                    //Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Contains("anguage")).Source = Properties.Settings.Default.Language_Uri;
                }
            }
            catch
            {
                Properties.Settings.Default.Reset();
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
                    //ResourceDictionary resourceDictionary = new ResourceDictionary();
                    //resourceDictionary.Source = Properties.Settings.Default.Language_Uri;
                    //Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                    //Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Contains("anguage")).Source = Properties.Settings.Default.Language_Uri;
                }
            }

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
