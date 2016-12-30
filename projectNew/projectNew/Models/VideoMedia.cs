using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectNew.Models
{
    public class VideoMedia:Media
    {
        public string VideoCodec { get; set; }
        public float VideoBitrate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float FrameRate { get; set; }
        public string AudioCodec { get; set; }
        public float AudioBitrate { get; set; }
        public int Channels { get; set; }
        public float SamplingRate { get; set; }
    }
}