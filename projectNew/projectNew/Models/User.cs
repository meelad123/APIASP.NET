using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projectNew.Models
{
    public class User
    {
        public User()
        {
            Friends = new List<User>();
            Vacations = new List<Vacation>();
        }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public virtual ICollection<User> Friends { get; set; }
        public virtual ICollection<Vacation> Vacations { get; set; }

    }
}