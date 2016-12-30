using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectNew.Models
{
    public class SoundMedia:Media
    {
        public float Duration { get; set; }
        public string Codec { get; set; }
        public float Bitrate { get; set; }
        public int Channels { get; set; }
        public float SamplingRate { get; set; }
    }
}