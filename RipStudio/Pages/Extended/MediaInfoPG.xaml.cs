using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using MediaInfoLib;
using MediaInfoDotNet.Models;
using MediaInfoDotNet;

namespace RipStudio.Pages
{
    /// <summary>
    /// Interaction logic for MediaInfo.xaml
    /// </summary>
    public partial class MediaInfoPG : UserControl
    {
        public MediaInfoPG()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
      private  int spnum=0;
      private int ypnum = 0;
      private int zmnum = 0;
      private int qtnum = 0;
        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            MFTV.Items.Clear();
             spnum = 0;
            ypnum = 0;
            zmnum = 0;
            qtnum = 0;
            MediaFile MF = new MediaFile(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString());
            (sender as TextBox).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (MF.Video.Count > 0)
            {
                TreeViewItem tvi1 = new TreeViewItem() { Header = "视频信息" };
                MFTV.Items.Add(tvi1);
                foreach (var aud in MF.Video.Values)
                {

                    TreeViewItem tvi2 = new TreeViewItem();
                    tvi2.Header = "视频轨道" + spnum.ToString();
                    tvi1.Items.Add(tvi2);

                    TreeViewItem tvi3 = new TreeViewItem();
                    tvi3.Header = "id : "+aud.miGetString("ID");
                    tvi2.Items.Add(tvi3);
                    foreach (PropertyInfo p in aud.GetType().GetProperties())
                    {
                            TreeViewItem tvi = new TreeViewItem();
                            tvi.Header = p.Name + " : " + p.GetValue(aud, null);
                            tvi2.Items.Add(tvi);
                    }
                    spnum++;
                }
            }
            if (MF.Audio.Count > 0)
            {
                TreeViewItem tvi1 = new TreeViewItem() { Header = "音频信息" };
                MFTV.Items.Add(tvi1);
                foreach (var aud in MF.Audio.Values)
                {

                    TreeViewItem tvi2 = new TreeViewItem();
                    tvi2.Header = "音频轨道" + ypnum.ToString();
                    tvi1.Items.Add(tvi2);

                    TreeViewItem tvi3 = new TreeViewItem();
                    tvi3.Header = "id : " + aud.miGetString("ID");
                    tvi2.Items.Add(tvi3);
                    foreach (PropertyInfo p in aud.GetType().GetProperties())
                    {
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = p.Name + " : " + p.GetValue(aud, null);
                        tvi2.Items.Add(tvi);
                    }
                    ypnum++;
                }
            }
            if (MF.Text.Count > 0)
            {
                TreeViewItem tvi1 = new TreeViewItem() { Header = "字幕信息" };
                MFTV.Items.Add(tvi1);
                foreach (var aud in MF.Text.Values)
                {

                    TreeViewItem tvi2 = new TreeViewItem();
                    tvi2.Header = "字幕轨道" + ypnum.ToString();
                    tvi1.Items.Add(tvi2);

                    TreeViewItem tvi3 = new TreeViewItem();
                    tvi3.Header = "id : " + aud.miGetString("ID");
                    tvi2.Items.Add(tvi3);
                    foreach (PropertyInfo p in aud.GetType().GetProperties())
                    {
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = p.Name + " : " + p.GetValue(aud, null);
                        tvi2.Items.Add(tvi);
                    }
                    zmnum++;
                }
            }
            if (MF.Other.Count > 0)
            {
                TreeViewItem tvi1 = new TreeViewItem() { Header = "其他信息" };
                MFTV.Items.Add(tvi1);
                foreach (var aud in MF.Other.Values)
                {

                    TreeViewItem tvi2 = new TreeViewItem();
                    tvi2.Header = "其他轨道" + ypnum.ToString();
                    tvi1.Items.Add(tvi2);

                    TreeViewItem tvi3 = new TreeViewItem();
                    tvi3.Header = "id : " + aud.miGetString("ID");
                    tvi2.Items.Add(tvi3);
                    foreach (PropertyInfo p in aud.GetType().GetProperties())
                    {
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = p.Name + " : " + p.GetValue(aud, null);
                        tvi2.Items.Add(tvi);
                    }
                    qtnum++;
                }
            }
            e.Handled = true;
        }



    }
}
