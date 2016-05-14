using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace RipStudio.Pages.Settings.Content
{
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class SettingsAppearanceViewModel
        : NotifyPropertyChanged
    {
        private Color[] AccentColors = new Color[]{
            Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
            Color.FromRgb(0x60, 0xa9, 0x17),   // green
            Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
            Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
            Color.FromRgb(0xaa, 0x00, 0xff),   // violet
            Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
            Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
            Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
            //Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xfa, 0x68, 0x00),   // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
            Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
            Color.FromRgb(0x6d, 0x87, 0x64),   // olive
            Color.FromRgb(0x64, 0x76, 0x87),   // steel
            Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
            Color.FromRgb(0x87, 0x79, 0x4e),   // taupe

            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            //Color.FromRgb(0xa2, 0x00, 0xff),   // purple   
        };
        
        private LinkCollection themes = new LinkCollection();
        private Link selectedTheme=null;

        private LinkCollection language = new LinkCollection();
        private Link selectedLanguage = null;

        private LinkCollection themeImg = new LinkCollection();
        private Link selectedThemeImg = null;

        public SettingsAppearanceViewModel()
        {
            this.themes.Add(new Link { DisplayName = "Dark", Source = AppearanceManager.DarkThemeSource });
            this.themes.Add(new Link { DisplayName = "Light", Source = AppearanceManager.LightThemeSource });
            
            this.language.Add(new Link { DisplayName = "SimplifiedChinese", Source = new Uri("/RipStudio;component/Language/SimplifiedChinese.xaml", UriKind.Relative) });
            this.language.Add(new Link { DisplayName = "English", Source = new Uri("/RipStudio;component/Language/English.xaml", UriKind.Relative) });

            //this.themeImg.Add(new Link { DisplayName = "No", Source = null});
            //DirectoryInfo TheFolder = new DirectoryInfo(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Background");
            //foreach (FileInfo NextFile in TheFolder.GetFiles())
            //{
            //    if (NextFile.Extension == ".jpg" || NextFile.Extension == ".png" || NextFile.Extension == ".bmp" || NextFile.Extension == ".jpeg")
            //    {
            //        this.themeImg.Add(new Link { DisplayName = NextFile.Name, Source = new Uri( @"Background/" + NextFile.Name,UriKind.Relative) });
            //    }
            //}

            this.selectedLanguage = this.language.FirstOrDefault(l => l.Source.Equals(Properties.Settings.Default.Language_Uri));
            this.selectedTheme = this.themes.FirstOrDefault(l => l.Source.Equals(Properties.Settings.Default.Theme_Uri));
            //this.selectedThemeImg = this.themeImg.FirstOrDefault(l => l.DisplayName.Equals(Properties.Settings.Default.ThemeImg_Name));
        }
        public LinkCollection Language
        {
            get { return this.language; }
        }
        public LinkCollection Themes
        {
            get { return this.themes; }
        }
        public Color[] AccentColor
        {
            get { return this.AccentColors; }
        }
        public LinkCollection ThemeImg
        {
            get { return this.themeImg; }
        }
        public Link SelectedThemeImg
        {
            get { return this.selectedThemeImg; }
            set
            {
                if (value.Source != null && value.Source.OriginalString != string.Empty)
                {
                    Properties.Settings.Default.ThemeImg_Uri = value.Source;
                    Properties.Settings.Default.ThemeImg_Name = value.DisplayName;
                    this.selectedThemeImg = value;
                    ((Application.Current.FindResource("WindowBackgroundContent") as Rectangle).Fill as ImageBrush).ImageSource = new BitmapImage(value.Source);
                    AppearanceManager.Current.ThemeSource = new Uri("/RipStudio;component/Assets/Style.xaml", UriKind.Relative);
                    OnPropertyChanged("selectedThemeImg");
                }
                else
                {
                    Properties.Settings.Default.ThemeImg_Uri = null;
                    Properties.Settings.Default.ThemeImg_Name = "No";
                    this.selectedThemeImg = value;
                    ((Application.Current.FindResource("WindowBackgroundContent") as Rectangle).Fill as ImageBrush).ImageSource = null;
                    AppearanceManager.Current.ThemeSource = new Uri("/RipStudio;component/Assets/Style.xaml", UriKind.Relative);
                    OnPropertyChanged("selectedThemeImg");
                }
            }
        }
        public Link SelectedTheme
        {
            get { return this.selectedTheme; }
            set
            {
                if (!Properties.Settings.Default.Theme_Uri.Equals(value.Source))
                {
               Properties.Settings.Default.Theme_Uri= AppearanceManager.Current.ThemeSource = value.Source;
               Properties.Settings.Default.Theme_DisplayName = value.DisplayName;
               this.selectedTheme=value;
                OnPropertyChanged("SelectedTheme");
                }
            }
        }
        public Link SelectedLanguage
        {
            get { return this.selectedLanguage; }
            set
            {
                 if (!Properties.Settings.Default.Language_Uri.Equals(value.Source))
                {
               Properties.Settings.Default.Language_Uri  = value.Source;
               Properties.Settings.Default.Language_DisplayName = value.DisplayName;
               this.selectedLanguage = value;

               Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Contains("anguage")).Source = Properties.Settings.Default.Language_Uri;

               ((MainWindow)((App)(Application.Current)).MainWindow).TL1.DisplayName = Application.Current.FindResource("TitleLinks_0") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).TL2.DisplayName = Application.Current.FindResource("TitleLinks_1") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).TL3.DisplayName = Application.Current.FindResource("TitleLinks_2") as string;

               ((MainWindow)((App)(Application.Current)).MainWindow).LG1.DisplayName = Application.Current.FindResource("MenuLinkGroups_Welcome") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG2.DisplayName = Application.Current.FindResource("MenuLinkGroups_AviSynth") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG3.DisplayName = Application.Current.FindResource("MenuLinkGroups_Video") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG4.DisplayName = Application.Current.FindResource("MenuLinkGroups_Audio") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG5.DisplayName = Application.Current.FindResource("MenuLinkGroups_Mux") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG6.DisplayName = Application.Current.FindResource("MenuLinkGroups_Extended") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG7.DisplayName = Application.Current.FindResource("TitleLinks_1") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG8.DisplayName = Application.Current.FindResource("TitleLinks_2") as string;

               ((MainWindow)((App)(Application.Current)).MainWindow).LG11.DisplayName = Application.Current.FindResource("Welcome_Link_0") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG21.DisplayName = Application.Current.FindResource("AviSynth_Link_0") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG22.DisplayName = Application.Current.FindResource("AviSynth_Link_1") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG23.DisplayName = Application.Current.FindResource("AviSynth_Link_2") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG24.DisplayName = Application.Current.FindResource("AviSynth_Link_3") as string;
               //((MainWindow)((App)(Application.Current)).MainWindow).LG61.DisplayName = Application.Current.FindResource("Extended_Link_0") as string;
               //((MainWindow)((App)(Application.Current)).MainWindow).LG62.DisplayName = Application.Current.FindResource("Extended_Link_1") as string;
               ((MainWindow)((App)(Application.Current)).MainWindow).LG63.DisplayName = Application.Current.FindResource("Extended_Link_2") as string;
               if (((App)(Application.Current)).SP != null)
               {
                   ((App)(Application.Current)).SP.Link1.DisplayName = Application.Current.FindResource("Settings_Style") as string;
                   ((App)(Application.Current)).SP.Link2.DisplayName = Application.Current.FindResource("Settings_About") as string;
                   //((App)(Application.Current)).SP.Link3.DisplayName = Application.Current.FindResource("Settings_About") as string;
               }
                    
               OnPropertyChanged("SelectedLanguage");
                }
            }
        }

        public Color SelectedAccentColor
        {
            get { return AppearanceManager.Current.AccentColor; }
            set
            {
                if (AppearanceManager.Current.AccentColor != value)
                {
                  Properties.Settings.Default.AccentColor = AppearanceManager.Current.AccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");
                }
            }
        }
        public double Transparent
        {
            get { return Properties.Settings.Default.ThemeImg_Opacity; }
            set
            {
                if (Properties.Settings.Default.ThemeImg_Opacity != value)
                {
                    ((Application.Current.FindResource("WindowBackgroundContent") as Rectangle).Fill as ImageBrush).Opacity = value/100.00;
                    Properties.Settings.Default.ThemeImg_Opacity = (int)value;
                    OnPropertyChanged("Transparent");
                   
                }
            }

        }
    }
}
