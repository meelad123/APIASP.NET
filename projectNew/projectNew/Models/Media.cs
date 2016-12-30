using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectNew.Models
{
    public class Media
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Container { get; set; }


        public virtual int MemoryId { get; set; }
        public virtual Memory Memory { get; set; }
    }
}