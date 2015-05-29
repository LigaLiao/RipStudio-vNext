using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for De.xaml
    /// </summary>
    public partial class De : UserControl
    {
        public De()
        {
            InitializeComponent();
            ((App)(Application.Current)).VideoDe = this;
        }

        private void DeblockerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeblockQED != null)
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                {
                    DeblockQED.Visibility = Visibility.Visible;
                    Unblock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DeblockQED.Visibility = Visibility.Collapsed;
                    Unblock.Visibility = Visibility.Visible;
                }
            }
        }

        private void DeBandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GradFun2D != null)
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                {
                    GradFun2D.Visibility = Visibility.Visible;
                    Flash3k1.Visibility = Flash3k2.Visibility = Flash3k3.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GradFun2D.Visibility = Visibility.Collapsed;
                    Flash3k1.Visibility = Flash3k2.Visibility = Flash3k3.Visibility = Visibility.Visible;
                }
            }
        }

        private void DeHaloComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DeHaloAlpha != null)
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                {
                    DeHaloAlpha.Visibility = Visibility.Visible;
                    BlindDeHalo.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DeHaloAlpha.Visibility = Visibility.Collapsed;
                    BlindDeHalo.Visibility = Visibility.Visible;
                }
            }
        }

        private void DeRinginComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MosquitoNR != null)
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                {
                    MosquitoNR.Visibility = Visibility.Visible;
                    HQDering.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MosquitoNR.Visibility = Visibility.Collapsed;
                    HQDering.Visibility = Visibility.Visible;
                }
            }
        }

        private void DeGrainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Removegrain != null)
            {
                switch ((sender as ComboBox).SelectedIndex)
                {
                    case 0: Removegrain.Visibility = Visibility.Visible;
                        Spresso.Visibility = STPresso.Visibility = STemporalDegrain.Visibility = DeGrainMedian.Visibility = MDegrain.Visibility = Visibility.Collapsed;
                        break;
                    case 1: Spresso.Visibility = Visibility.Visible;
                        Removegrain.Visibility = STPresso.Visibility = STemporalDegrain.Visibility = DeGrainMedian.Visibility = MDegrain.Visibility = Visibility.Collapsed;
                        break;
                    case 2: STPresso.Visibility = Visibility.Visible;
                        Spresso.Visibility = Removegrain.Visibility = STemporalDegrain.Visibility = DeGrainMedian.Visibility = MDegrain.Visibility = Visibility.Collapsed;
                        break;
                    case 3: STemporalDegrain.Visibility = Visibility.Visible;
                        Spresso.Visibility = STPresso.Visibility = Removegrain.Visibility = DeGrainMedian.Visibility = MDegrain.Visibility = Visibility.Collapsed;
                        break;
                    case 4: DeGrainMedian.Visibility = Visibility.Visible;
                        Spresso.Visibility = STPresso.Visibility = STemporalDegrain.Visibility = Removegrain.Visibility = MDegrain.Visibility = Visibility.Collapsed;
                        break;
                    case 5: MDegrain.Visibility = Visibility.Visible;
                        Spresso.Visibility = STPresso.Visibility = STemporalDegrain.Visibility = DeGrainMedian.Visibility = Removegrain.Visibility = Visibility.Collapsed;
                        break;
                }

            }
        }

        private void DeRingingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MosquitoNR != null)
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                {
                    MosquitoNR.Visibility = Visibility.Visible;
                }
                else
                {
                    MosquitoNR.Visibility = Visibility.Collapsed;
                }
            }
        }


    }
}
