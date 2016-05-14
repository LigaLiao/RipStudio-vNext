using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
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

namespace RipStudio
{
    /// <summary>
    /// Interaction logic for CheckScriptWindow.xaml
    /// </summary>
    public partial class CheckScriptWindow : ModernWindow
    {
        private string FileName="";
        public CheckScriptWindow(string script, string filename)
        {
            InitializeComponent();
            Script.Text = script;
            FileName=filename;
        }

        private void PreviewScript_Click(object sender, RoutedEventArgs e)
        {
            if (Script.Text != String.Empty )
            {
                try
                {
                    if (((App)(Application.Current)).vp == null)
                    {
                        ((App)(Application.Current)).vp = new VideoPlayr(Script.Text, false);
                        ((App)(Application.Current)).vp.Show();
                    }
                    else
                    {
                        ((App)(Application.Current)).vp.Close();
                        ((App)(Application.Current)).vp = new VideoPlayr(Script.Text, false);
                        ((App)(Application.Current)).vp.Show();
                    }
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "AviSynth Error", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("脚本不能空", "RipStudio Message", MessageBoxButton.OK);
            }
        }

        private void SaveScript_Click(object sender, RoutedEventArgs e)
        {
            if (Script.Text != String.Empty)
            {
                    try
                    {
                        if (FileName!=string.Empty)
                        {
                            StreamWriter sw = new StreamWriter(FileName, false, Encoding.Default);
                            sw.WriteLine(Script.Text);
                            sw.Close();
                            if (IsLoadingEncoder.IsChecked == true)
                            {
                                if (!String.IsNullOrWhiteSpace(((App)(Application.Current)).avs_Main.VideoTB.Text))
                                {
                                    ((App)(Application.Current)).VPresetAVS = FileName;
                                }
                                if (!String.IsNullOrWhiteSpace(((App)(Application.Current)).avs_Main.AudioTB.Text))
                                {
                                    ((App)(Application.Current)).APresetAVS = FileName;
                                }

                                //((App)(Application.Current)).PresetAVS = FileName;
                            }
                            ModernDialog.ShowMessage("保存成功！", "RipStudio Message", MessageBoxButton.OK);
                        }
                        else
                        {
                            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                            dlg.DefaultExt = ".avs"; // Default file extension
                            dlg.Filter = "AviSynth Script|*.avs"; // Filter files by extension
                            // Show open file dialog box
                            Nullable<bool> result = dlg.ShowDialog();
                            // Process open file dialog box results
                            if (result == true)
                            {
                                // Open document
                                StreamWriter sw = new StreamWriter(dlg.FileName, false, Encoding.Default);
                                sw.WriteLine(Script.Text);
                                sw.Close();
                                if(IsLoadingEncoder.IsChecked==true)
                                {
                                    if (!String.IsNullOrWhiteSpace(((App)(Application.Current)).avs_Main.VideoTB.Text))
                                    {
                                        ((App)(Application.Current)).VPresetAVS = dlg.FileName;
                                    }
                                    if (!String.IsNullOrWhiteSpace(((App)(Application.Current)).avs_Main.AudioTB.Text))
                                    {
                                        ((App)(Application.Current)).APresetAVS = dlg.FileName;
                                    }
                                    //((App)(Application.Current)).PresetAVS = dlg.FileName;
                                }
                                //ModernDialog.ShowMessage("保存成功！", "RipStudio Message", MessageBoxButton.OK);
                            }
                        }
                    }
                    
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "AviSynth Save Error", MessageBoxButton.OK);
                    }
                }
            
            else
            {
                ModernDialog.ShowMessage("脚本不能空", "RipStudio Message", MessageBoxButton.OK);
            }
        }
    }
}
