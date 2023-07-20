using IdentityModule.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityModule.Database
{
    public class IdentityDataContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public IdentityDataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<long>>().HasKey(u => new { u.UserId, u.RoleId });
            modelBuilder.Entity<IdentityUserLogin<long>>().HasKey(u => new { u.UserId, u.ProviderKey });
            modelBuilder.Entity<IdentityUserToken<long>>().HasKey(u => new { u.UserId, u.Value });
            modelBuilder.Entity<IdentityRole<long>>().ToTable("Roles");
        }

        public new DbSet<User> Users { get; set; }
        public new DbSet<IdentityRole<long>> Roles { get; set; }
    }
}
