using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RipStudio.Pages.ScriptCreater
{
    /// <summary>
    /// Interaction logic for Cut.xaml
    /// </summary>
    public partial class Cut : UserControl
    {
        public ObservableCollection<Customer> cutdata;
        public Cut()
        {
            InitializeComponent();
            ((App)(Application.Current)).avs_Cut = this;
            cutdata = new ObservableCollection<Customer>(); 
            DG1.DataContext = cutdata;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            cutdata.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DG1.SelectedItem != null )
            {
                try { cutdata.Remove((Customer)DG1.SelectedItem); }
                catch { }
            }
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            if(DG1.SelectedIndex>-1)
            {
             cutdata.Insert(DG1.SelectedIndex,new Customer());
            
            }
           
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ModernDialog.ShowMessage(Properties.Settings.Default.Language_Uri.ToString(), "RipStudio Message", MessageBoxButton.OK);
            
        }
    }
}
