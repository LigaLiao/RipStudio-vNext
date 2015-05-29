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
    /// Interaction logic for Else.xaml
    /// </summary>
    public partial class Other : UserControl
    {
        public Other()
        {
            InitializeComponent();
            ((App)(Application.Current)).VideoOther = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".avs"; // Default file extension
            dlg.Filter = "LogoFile|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                LOGOFile.Text = dlg.FileName;
            }
        }
    }
}
