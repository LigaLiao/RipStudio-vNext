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

namespace RipStudio.Pages.Video
{
    /// <summary>
    /// Interaction logic for H265ConfigTabPage.xaml
    /// </summary>
    public partial class H265ConfigTabPage : UserControl
    {
        public H265ConfigTabPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (((App)(Application.Current)).H265IOPAGE.EncoderSettings_CB.SelectedIndex != 0)
            {
                MTab.SelectedSource = new Uri("Pages/Video/H265Settings/Advanced.xaml", UriKind.Relative);
            }
        }
    }
}
