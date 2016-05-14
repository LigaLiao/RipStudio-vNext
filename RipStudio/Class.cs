using AvisynthWrapper;
using FirstFloor.ModernUI.Windows.Controls;
using libavs2aac;
using libavs2flac;
using libavs2wav;
using libavs2x264;
using libavs2x265;
using libmuxer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RipStudio
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool re = (bool)value;
            if (re)
            {
                return "男";
            }
            else
            {
                return "女";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (strValue == "男")
            {
                return true;
            }
            if (strValue == "女")
            {
                return false;
            }
            return DependencyProperty.UnsetValue;
        }
    }


    public enum EncodingType
    {
        H264,
        H265,
        AAC,
        FLAC,
        WAV,
        MP4,
        MKV,
        AUTO
    }
    public class Customer
    {
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
    }
    public class Customer2
    {
        string timecode="00:00:00.000";
        string name = "Chapter Name";
        public string Timecode 
        {
            get {return timecode; }
            set { timecode = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
    public class Customer3
    {
        public string File { get; set; }
    }
    //public class Customer2
    //{
    //    public string File { get; set; }                //e.Result = x264Encoder.Start(fileout, (BackgroundWorker)sender, (string)e.Argument);
    //}

    public class AutoEncoder
    {
        private X264Encoder x264Encoder;
        private X265Encoder x265Encoder;
        private AacEncoder aacEncoder;
        private FlacEncoder flacEncoder;
        private WavEncoder wavEncoder;
        private Muxer muxer;

        private string videofile = string.Empty;
        private string audiofile = string.Empty;
        private string muxfile = string.Empty;

        private string videoConfig = string.Empty;
        private object audioConfig = string.Empty;

        private EncodingType videoType;
        private EncodingType audioType;


        //private int job=0;
        public AutoEncoder(string script, string output, EncodingType videotype, EncodingType audiotype, MuxerType muxType, string videoconfig, object audioconfig)
        {
            videoConfig = videoconfig;
            audioConfig = audioconfig;
            videoType = videotype;
            audioType = audiotype;
            if (videotype == EncodingType.H264)
            {
                x264Encoder = new X264Encoder(script, false);
                //videofile  = Path.GetDirectoryName(output) +@"\"+ Path.GetFileNameWithoutExtension(output) + "_wfm.h264";
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = output.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    videofile = Path.GetDirectoryName(output) + @"\" + Path.GetFileNameWithoutExtension(output) + "_wfm.h264";
                }
                else
                {
                    videofile = Path.GetDirectoryName(output)  + Path.GetFileNameWithoutExtension(output) + "_wfm.h264";
                }
            }
            else
            {
                x265Encoder = new X265Encoder(script, false);
                //videofile = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.h265";
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = output.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    videofile = Path.GetDirectoryName(output) + @"\" + Path.GetFileNameWithoutExtension(output) + "_wfm.h265";
                }
                else
                {
                    videofile = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.h265";
                }
            }

            if (audiotype == EncodingType.AAC)
            {
                aacEncoder = new AacEncoder(script, false);
                //audiofile  = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.aac";
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = output.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    audiofile = Path.GetDirectoryName(output) + @"\" + Path.GetFileNameWithoutExtension(output) + "_wfm.aac";
                }
                else
                {
                    audiofile = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.aac";
                }
            }
            else if (audiotype == EncodingType.FLAC)
            {
                flacEncoder = new FlacEncoder(script, false);
                //audiofile  = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.flac";
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = output.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    audiofile = Path.GetDirectoryName(output) + @"\" + Path.GetFileNameWithoutExtension(output) + "_wfm.flac";
                }
                else
                {
                    audiofile = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.flac";
                }
            }
            else
            {
                wavEncoder = new WavEncoder(script, false);
                //audiofile  = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.flac";
                string s = @"\";
                int Star = 0;
                int Count = 0;
                while (Star != -1)
                {
                    Star = output.IndexOf(s, Star);
                    if (Star != -1)
                    {
                        Count++;
                        Star++;
                    }
                }
                if (Count > 1)
                {
                    audiofile = Path.GetDirectoryName(output) + @"\" + Path.GetFileNameWithoutExtension(output) + "_wfm.wav";
                }
                else
                {
                    audiofile = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_wfm.wav";
                }
            }

            Avisynth avs= new Avisynth(script,false);
            ScriptInfo si = avs.GetScriptInfo();
            avs.FreeAvisynth();

            //ModernDialog.ShowMessage(((float)(si.fps_numerator / si.fps_denominator)).ToString(), "RipStudio Message", MessageBoxButton.OK);
            muxer = new Muxer(muxType);
            muxer.AddVideo(videofile, DeviceType.Standard, (float)(si.fps_numerator / si.fps_denominator));
            muxer.AddAudio(audiofile);
            string s2 = @"\";
            int Star2 = 0;
            int Count2 = 0;
            while (Star2 != -1)
            {
                Star2 = output.IndexOf(s2, Star2);
                if (Star2 != -1)
                {
                    Count2++;
                    Star2++;
                }
            }
            if (Count2 > 1)
            {
                muxfile = Path.GetDirectoryName(output) +@"\"+ Path.GetFileNameWithoutExtension(output) + "." + muxType.ToString().ToLower();
            }
            else
            {
                muxfile = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "." + muxType.ToString().ToLower();
            }
            //muxfile  = Path.GetDirectoryName(output) + Path.GetFileNameWithoutExtension(output) + "_muxed." + muxType.ToString().ToLower();
        }


        public string Start(BackgroundWorker bw)
        {
            string returnstr = string.Empty;
            if (videoType == EncodingType.H264)
            {
                //job = 0;
                returnstr = x264Encoder.Start(videofile, bw, videoConfig);
                if (returnstr == "END")
                {
                    if (audioType == EncodingType.AAC)
                    {
                        //job = 1;
                        returnstr = aacEncoder.Start(audiofile, bw, (AacEncoderConfig)audioConfig);
                        if (returnstr == "END")
                        {
                            //job = 2;
                            return muxer.Start(muxfile, bw);
                        }
                        else
                        {
                            return returnstr;
                        }
                    }
                    else
                    {
                        //job = 1;
                        returnstr = flacEncoder.Start(audiofile, bw, (FlacEncoderConfig)audioConfig);
                        if (returnstr == "END")
                        {
                            //job = 2;
                            return muxer.Start(muxfile, bw);
                        }
                        else
                        {
                            return returnstr;
                        }
                    }
                }
                else
                {
                    return returnstr;
                }
            }
            else
            {
                //job = 0;
                returnstr = x265Encoder.Start(videofile, bw, videoConfig);
                if (returnstr == "END")
                {
                    if (audioType == EncodingType.AAC)
                    {
                        //job = 1;
                        returnstr = aacEncoder.Start(audiofile, bw, (AacEncoderConfig)audioConfig);
                        if (returnstr == "END")
                        {
                            //job = 2;
                            return muxer.Start(muxfile, bw);
                        }
                        else
                        {
                            return returnstr;
                        }
                    }
                    else
                    {
                        //job = 1;
                        returnstr = flacEncoder.Start(audiofile, bw, (FlacEncoderConfig)audioConfig);
                        if (returnstr == "END")
                        {
                            //job = 2;
                            return muxer.Start(muxfile, bw);
                        }
                        else
                        {
                            return returnstr;
                        }
                    }
                }
                else
                {
                    return returnstr;
                }
            }
        }
    }




}
