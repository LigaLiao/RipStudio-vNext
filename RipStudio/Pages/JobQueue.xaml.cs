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

namespace RipStudio.Pages
{
    /// <summary>
    /// Interaction logic for JobQueue.xaml
    /// </summary>
    public partial class JobQueue : UserControl
    {
        //private ObservableCollection<Customer> cutdata;
        public JobQueue()
        {
            InitializeComponent();
            JQG.Children.Add(((App)(Application.Current)).LV);
            //((App)(Application.Current)).LV
            //((App)(Application.Current)).LV.Items.Add(new JobItem("fafafaffffffffffffffffffffffffffffffffff", "E:\\暗杀教室01生肉2.mp4", EncodingType.WAV, null,true));
            //((App)(Application.Current)).LV.Items.Add(new JobItem("fafafaffffffffffffffffffffffffffffffffff", "fffff卧槽fff卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽卧槽ffffafafafaf", EncodingType.WAV, null, true));
        }


    }
}
