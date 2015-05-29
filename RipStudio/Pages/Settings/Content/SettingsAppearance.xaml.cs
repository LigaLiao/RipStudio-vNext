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
            ModernDialog.ShowMessage(Properties.Settings.Default.Language_Uri.ToString(), "RipStudio Message", MessageBoxButton.OK);
            
        }
    }
}
