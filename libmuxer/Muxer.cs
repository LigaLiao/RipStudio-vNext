using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libmuxer
{
    public enum MuxerType
    {
        MP4,
        MKV
    }
    public enum DeviceType
    {
        Standard,
        psp,
        ipod,
        OtherAppleDevices
    }

    public class Muxer
    {
        private MuxerType muxertype;
        private string arguments = "";
        private string FileName = "";
        private string chapters = "";
        private bool b;

        public string Arguments
        {
            get { return arguments; }
        }

        public string Chapters
        {
            get { return chapters; }
            set
            {
                if (muxertype == MuxerType.MP4)
                {
                    arguments += (" -chap \"" + value + "\"");
                }
                else
                {
                    arguments += (" --chapters \"" + value + "\"");
                }
                chapters = value;
            }
        }
        public void AddVideo(string filename, DeviceType devicetype, float fps)
        {
            if (muxertype == MuxerType.MP4)
            {

                if (devicetype != DeviceType.Standard)
                {
                    if (devicetype == DeviceType.OtherAppleDevices)
                    {
                        arguments += (" -ipod -brand M4VH");
                    }
                    else
                    {
                        arguments += ("-" + devicetype.ToString());
                    }
                }
                arguments += (" -add \"" + filename);
                if (Path.GetExtension(filename).Contains("265") || Path.GetExtension(filename).Contains("hevc"))
                {
                    arguments += ":FMT=HEVC";
                }
                arguments += ":FPS=" + fps.ToString();
                arguments += "\"";
               
            }
            else
            {
                string Fps = "";
                if (fps > 23 && fps < 24)
                {
                    Fps = "24000/1001";
                }
                else if (fps > 29 && fps < 30)
                {
                    Fps = "30000/1001";
                }
                else if (fps > 59 && fps < 60)
                {
                    Fps = "60000/1001";
                }
                else
                {
                    if (fps.ToString().Contains("."))
                    {
                        Fps = (((int)(fps+0.5)) * 1000).ToString() + "/1001";
                    }
                    else
                    {
                        Fps = (fps * 1000).ToString() + "/1000";
                    }

                }
                arguments += " --engage keep_bitstream_ar_info --default-duration 0:" + Fps + "fps -d \"0\" --no-chapters -A -S \"" + filename + "\"";
            }
        }

        public void AddAudio(string filename)
        {
            if (muxertype == MuxerType.MP4)
            {
                arguments += (" -add \"" + filename + "\"");
            }
            else
            {
                arguments += (" -a 0 --no-chapters -D -S \"" + filename + "\"");
            }
        }
        public void AddSubtitle(string filename)
        {
            if (muxertype == MuxerType.MP4)
            {
                arguments += (" -add \"" + filename + "\"");
            }
            else
            {
                if (b == false)
                {
                    arguments += (" --default-track \"0:yes\" --forced-track \"0:no\" -s 0 -D -A \"" + filename + "\"");
                    b = true;
                }
                else
                {
                    arguments += (" --default-track \"0:no\" --forced-track \"0:no\" -s 0 -D -A \"" + filename + "\"");
                }
            }
        }

        public Muxer(MuxerType Muxertype)
        {
            muxertype = Muxertype;
            if (muxertype == MuxerType.MP4)
            {
                FileName = @"ExternalMuxer\mp4box.exe";
            }
            else
            {
                FileName = @"ExternalMuxer\mkvmerge.exe";
            }
        }



        public string Start(String filename, BackgroundWorker bw)
        {

            String standard = "";
            String Error = "";
            Process p = new Process();
            p.StartInfo.FileName = FileName;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            if (muxertype == MuxerType.MP4)
            {
                p.StartInfo.Arguments = Arguments + " -new \"" + filename + "\"";
            }
            else
            {
                p.StartInfo.Arguments = " -o \"" + filename + "\"" + Arguments;
            }

            //return Arguments;
            p.Start();
            //int start = System.Environment.TickCount;
            if (muxertype == MuxerType.MP4)
            {
                while (((standard = p.StandardError.ReadLine()) != null) && (!bw.CancellationPending))
                {
                    if (standard.Length > 10)
                    {
                        Error = standard;
                    }

                    if (standard.TrimEnd().EndsWith("/100)"))
                    {
                        bw.ReportProgress(int.Parse(standard.Split('(')[1].Substring(0, 2).Trim()));
                    }
                    else
                    {
                        bw.ReportProgress(0);
                    }
                }
            }
            else
            {
                while (((standard = p.StandardOutput.ReadLine()) != null) && (!bw.CancellationPending))
                {

                    //MessageBox.Show(standard);
                    //if (standard.Length > 10)
                    //{
                    //    Error = standard;
                    //}


                    //if (standard.StartsWith("Pro") && standard.Contains("%\r"))
                    //{
                    //    string percentage = standard.Substring(10, standard.IndexOf("%\r", 10) - 10);
                    //    bw.ReportProgress(int.Parse(percentage));
                    //}

                    if (standard.StartsWith("Error:") && standard.Length > 7)
                        Error = standard;
                    else if (standard.Contains("%"))
                    {
                        string percentage = standard.Substring(standard.IndexOf("%")-2, 2);
                        //MessageBox.Show(percentage);
                        bw.ReportProgress(int.Parse(percentage));
                    }
                }
            }

            if (!bw.CancellationPending)
            {
                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    return "END";
                }
                else
                {
                    return Error;
                }
            }
            else
            {
                p.Close();
                p.CancelErrorRead();
                p.CancelOutputRead();
                p.Dispose();
                return "Cancel";
            }

        }
    }
}
