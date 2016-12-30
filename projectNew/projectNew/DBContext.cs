using Microsoft.AspNet.Identity.EntityFramework;
using projectNew.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace projectNew
{
    public class DBContext : IdentityDbContext<IdentityUser>
    {
        public DBContext()
            : base("DBConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<DBContext>(null);
        }
        public DbSet<User> MyUsers { get; set; }
        public DbSet<Vacation> MyVacations { get; set; }
        public DbSet<Memory> MyMemories { get; set; }
        public DbSet<Media> MyMedia { get; set; }
        public static DBContext Create()
        {
            return new DBContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Models.User>()
                            .HasMany(x => x.Friends)
                            .WithMany();
        }
    }
}