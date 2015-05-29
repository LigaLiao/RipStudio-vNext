using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RipStudio
{
    public enum EncodingType
    {
        X264,
        X265,
        AAC,
        FLAC,
        WAV
    }
    public class Customer
    {
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
    }
}
