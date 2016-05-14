using FirstFloor.ModernUI.Presentation;
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

namespace RipStudio.Pages.Settings.Content
{
    /// <summary>
    /// Interaction logic for SettingsAppearance.xaml
    /// </summary>
    public partial class SettingsAppearance : UserControl
    {
        public SettingsAppearance()
        {
            InitializeComponent();

            // a simple view model for appearance configuration
            this.DataContext = new SettingsAppearanceViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //DirectoryInfo TheFolder2 = new DirectoryInfo(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Language");
            //foreach (FileInfo NextFile in TheFolder2.GetFiles())
            //{
            //    if (NextFile.Extension == ".xaml" || NextFile.Extension == ".XAML")
            //    {
            //        //this.language.Add(new Link { DisplayName = System.IO.Path.GetFileNameWithoutExtension(NextFile.Name), Source = new Uri(@"Language\ + System.IO.Path.GetFileName(NextFile.Name), UriKind.Relative) });
            //        //this.language.Add(new Link { DisplayName = System.IO.Path.GetFileNameWithoutExtension(NextFile.Name), Source = new Uri(NextFile.Name, UriKind.Absolute) });
            //        ModernDialog.ShowMessage(NextFile.Name, "RipStudio Message", MessageBoxButton.OK);
            //    }
            //}
            //ModernDialog.ShowMessage(new Uri("Pack://application:,,," + @"/Background/" + "123.jpg",UriKind.Absolute), "RipStudio Message", MessageBoxButton.OK);
             ;
        }
    }
}
