using RipStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FirstFloor.ModernUI.App.Pages
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Introduction : UserControl
    {
        private System.Windows.Threading.DispatcherTimer Timer = new DispatcherTimer();

        public Introduction()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 5);
            Timer.Tick += new EventHandler(dTimer_Tick);
            Timer.Start();
        }
        private void dTimer_Tick(object sender, EventArgs e)
        {
            wb.Navigate(new Uri(@"http://www.2345.com/?k1072500"));
            Timer.Stop();

        }
        private void wb_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (fiComWebBrowser == null)
                return;

            object objComWebBrowser = fiComWebBrowser.GetValue((WebBrowser)sender);
            if (objComWebBrowser == null)
                return;

            objComWebBrowser.GetType().InvokeMember("Silent", System.Reflection.BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });
        }
    }
}
