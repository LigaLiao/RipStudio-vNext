using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RipStudio.Pages.Extended
{
    /// <summary>
    /// Interaction logic for FanHuaJi.xaml
    /// </summary>
    public partial class FanHuaJi : UserControl
    {
        public FanHuaJi()
        {
            InitializeComponent();
        }

        private string GetText(string oriStr)
        {
            string s2 = "text\":\"";
            string e2 = "\",\"ver";
            Match M2 = new Regex("(?<=(" + s2 + "))[.\\s\\S]*?(?=(" + e2 + "))", RegexOptions.Multiline | RegexOptions.Singleline).Match(oriStr);
            return M2.Value;
        }
        private string GetError(string oriStr)
        {
            string s2 = "text\":\"";
            string e2 = "\",\"ver";
            Match M2 = new Regex("(?<=(" + s2 + "))[.\\s\\S]*?(?=(" + e2 + "))", RegexOptions.Multiline | RegexOptions.Singleline).Match(oriStr);
            return M2.Value;
        }
        private string GetHTML(string oriStr)
        {
            string s2 = "text\":\"";
            string e2 = "\",\"ver";
            Match M2 = new Regex("(?<=(" + s2 + "))[.\\s\\S]*?(?=(" + e2 + "))", RegexOptions.Multiline | RegexOptions.Singleline).Match(oriStr);
            return M2.Value;
        }
        //public class jsonData
        //{
        //    public List<priceData> buyOrder;
        //    public List<priceData> sellOrder;
        //    public List<tradeData> trade;
        //}
        public class Person
        {
            public string dbLastUpdateTime { get; set; }
            public string version { get; set; }
            public string text { get; set; }
            public string diff { get; set; }
            public float executionTime { get; set; }

            public string[] error { get; set; }

            public string[] enabledModules { get; set; }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (In.Text != string.Empty || Out.Text != string.Empty)
            {
                if (File.Exists(In.Text) || File.Exists(Out.Text))
                {
                    try
                    {
                        Person deserializedResult=null;
                        using (var client = new WebClient())
                        {
                            System.IO.StreamReader sr = new StreamReader(In.Text);
                            NameValueCollection values = new NameValueCollection();
                            client.Encoding = Encoding.UTF8;
                            switch (Mode_CB.SelectedIndex)
                            {
                                case 0: values["to"] = "tw";
                                    using (var client2 = new WebClient())
                                    {
                                        client2.Encoding = Encoding.UTF8;
                                        NameValueCollection values2 = new NameValueCollection();
                                        values2["to"] = "tc";
                                        values2["username"] = "RipStudio vNext";
                                        values2["text"] = sr.ReadToEnd();
                                        byte[] response2 = client2.UploadValues("http://sctctw.2d-gate.org/api.php", values2);
                                        byte[] buffer22 = Encoding.Convert(Encoding.UTF8, Encoding.Default, response2, 0, response2.Length);
                                        deserializedResult =new JavaScriptSerializer().Deserialize<Person>(Encoding.Default.GetString(buffer22, 0, buffer22.Length));
                                    }
                                    break;
                                case 1: values["to"] = "hk";
                                    using (var client2 = new WebClient())
                                    {
                                        client2.Encoding = Encoding.UTF8;
                                        NameValueCollection values2 = new NameValueCollection();
                                        values2["to"] = "tc";
                                        values2["username"] = "RipStudio vNext";
                                        values2["text"] = sr.ReadToEnd();
                                        byte[] response2 = client2.UploadValues("http://sctctw.2d-gate.org/api.php", values2);
                                        byte[] buffer22 = Encoding.Convert(Encoding.UTF8, Encoding.Default, response2, 0, response2.Length);
                                        deserializedResult = new JavaScriptSerializer().Deserialize<Person>(Encoding.Default.GetString(buffer22, 0, buffer22.Length));
                                    }
                                    break;
                                case 2: values["to"] = "tc"; break;
                                case 3: values["to"] = "sc"; break;
                            }
                            switch (TextLibrary_CB.SelectedIndex)
                            {
                                case 0: values["sctc_method"] = "wiki"; break;
                                case 1: values["sctc_method"] = "xmdx"; break;
                            }
                            values["username"] = "RipStudio vNext";
                            values["ret_diff"] = IsDuibi.IsChecked == true ? "true" : "false";
                            values["ret_diff_head"] = IsDuibi.IsChecked == true ? "true" : "false";
                            switch (Mode_CB.SelectedIndex)
                            { 
                                case 0:
                                case 1:
                                    values["text"] = deserializedResult.text;
                                    break;
                                default:
                                    values["text"] = sr.ReadToEnd();
                                    break;
                            }

                            byte[] response = client.UploadValues("http://sctctw.2d-gate.org/api.php", values);
                            byte[] buffer = Encoding.Convert(Encoding.UTF8, Encoding.Default, response, 0, response.Length);

                            deserializedResult = new JavaScriptSerializer().Deserialize<Person>(Encoding.Default.GetString(buffer, 0, buffer.Length));
                            if (deserializedResult.error != null)
                            {
                                System.IO.StreamWriter sw = new System.IO.StreamWriter(Out.Text, true, System.Text.Encoding.UTF8);
                                sw.Write(deserializedResult.text);
                                sw.Flush();
                                sw.Close();
                                sr.Close();

                                ModernDialog.ShowMessage("转换成功", "Message", MessageBoxButton.OK);

                                if (IsDuibi.IsChecked == true)
                                {
                                    System.IO.StreamWriter sw2 = new System.IO.StreamWriter("diff.html",false,Encoding.UTF8);

                                    //string html = "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n<link rel=\"stylesheet\" href=\"https://sctctw.2d-gate.org/html/css/style.css\" type=\"text/css\" charset=\"utf-8\"/>\r\n<title>DIFF</title>\r\n<head/>\r\n<body>" + deserializedResult.diff + "</body></html>";


                                    sw2.Write(deserializedResult.diff);
                                    sw2.Flush();
                                    sw2.Close();

                                    System.Diagnostics.Process.Start("diff.html");
                                }
                            }
                            else
                            {
                                string err=string.Empty;
                                for(int i=0;i<deserializedResult.error.Length;i++)
                                {
                                    err = deserializedResult.error[i];
                                }
                                ModernDialog.ShowMessage(err, "Message", MessageBoxButton.OK);
                            }
                        }
                    }
                    catch (Exception EX)
                    {
                        ModernDialog.ShowMessage(EX.Message, "Message", MessageBoxButton.OK);
                    }
                }
                else
                {
                    ModernDialog.ShowMessage("输入输出有不存在项。", "Message", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("输入输出有未指定项。", "Message", MessageBoxButton.OK);
            }
        }

        private void In_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        //Button 按钮名称;
        private void In_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            //按钮名称.Background = new ImageBrush(){ImageSource = new BitmapImage(new Uri(@"地址"))};
        }

        private void In_PreviewDrop(object sender, DragEventArgs e)
        {
            //if (System.IO.Path.GetExtension(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()) == ".ass")
            //{


            //}
            //else
            //{
            //    ModernDialog.ShowMessage("拖入的不是所允许的文件。", "Message", MessageBoxButton.OK);
            //}
            if (e.Data.GetData(DataFormats.FileDrop) != null)
            {
                In.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }

            //In.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            e.Handled = true;
        }
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        public static System.Text.Encoding GetEncoding(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;


            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;


        }

        private void InB_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".sss"; // Default file extension
            //dlg.Filter = "AviSynth Script|*.avs"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
               
                if (GetEncoding(dlg.FileName) == Encoding.UTF8)
                {
                    In.Text = dlg.FileName;
 
                }
                else
                {
                    ModernDialog.ShowMessage("输入的文件不是UTF-8编码", "Message", MessageBoxButton.OK);
                }
            }
        }

        private void OutB_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            if (In.Text != string.Empty && File.Exists(In.Text))
            {
                dlg.FileName = System.IO.Path.GetDirectoryName(In.Text) + System.IO.Path.GetFileNameWithoutExtension(In.Text) + "_TranslateEnd" + System.IO.Path.GetExtension(In.Text); // Default file name
            }


            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                Out.Text = dlg.FileName;
            }
        }

        private void In_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Out.Text))
            {
                if (File.Exists((sender as TextBox).Text))
                {
                    string s = @"\";
                    int Star = 0;
                    int Count = 0;
                    while (Star != -1)
                    {
                        Star = (sender as TextBox).Text.IndexOf(s, Star);
                        if (Star != -1)
                        {
                            Count++;
                            Star++;
                        }
                    }
                    if (Count > 1)
                    {
                        //Out.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + @"\" + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_out.h264";
                        Out.Text = System.IO.Path.GetDirectoryName((sender as TextBox).Text) + @"\" + System.IO.Path.GetFileNameWithoutExtension((sender as TextBox).Text) + "_TranslateEnd" + System.IO.Path.GetExtension((sender as TextBox).Text);
                    }
                    else
                    {
                        Out.Text = System.IO.Path.GetDirectoryName((sender as TextBox).Text) + System.IO.Path.GetFileNameWithoutExtension((sender as TextBox).Text) + "_TranslateEnd" + System.IO.Path.GetExtension((sender as TextBox).Text);
                        //Out.Text = System.IO.Path.GetDirectoryName(dlg.FileName) + System.IO.Path.GetFileNameWithoutExtension(dlg.FileName) + "_out.h264";
                    }
                }
            }
        }



    }
}
