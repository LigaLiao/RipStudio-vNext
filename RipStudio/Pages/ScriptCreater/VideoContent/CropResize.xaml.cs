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

namespace RipStudio.Pages.ScriptCreater.VideoContent
{
    /// <summary>
    /// Interaction logic for Crop_Resize.xaml
    /// </summary>
    public partial class CropResize : UserControl
    {
        public CropResize()
        {
            InitializeComponent();
            ((App)(Application.Current)).VideoMain = this;
        }

        //private void Resolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (CustomResolution != null)
        //    {
        //        if ((sender as ComboBox).SelectedIndex == 7)
        //        {
                   
        //        }
        //        else
        //        {
                   
        //        }
        //    }
        //}

        private void CustomResolutionItem_Selected(object sender, RoutedEventArgs e)
        {
            CustomResolution.Visibility = Visibility.Visible;
        }

        private void CustomResolutionItem_Unselected(object sender, RoutedEventArgs e)
        {
            CustomResolution.Visibility = Visibility.Hidden;
        }
    }
}
