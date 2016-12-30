using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectNew.Models
{
    public class Vacation
    {
        public int VacationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual ICollection<Memory> Memories { get; set; }
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}