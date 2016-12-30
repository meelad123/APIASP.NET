using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace projectNew.Models
{
    public class Memory
    {
        public int MemoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Time{ get; set; }
        public string Place { get; set; }
        public Position Position { get; set; }

        public virtual ICollection<Media> Media { get; set; }

        public virtual int VacationId { get; set; }
        public virtual Vacation Vacation { get; set; }
    }
}