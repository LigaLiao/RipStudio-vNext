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
    /// Interaction logic for Color.xaml
    /// </summary>
    public partial class ColorPG : UserControl
    {
        public ColorPG()
        {
            InitializeComponent();
            ((App)(Application.Current)).VideoColor = this;
        }

        private void 画面旋转_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void 画面旋转_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
